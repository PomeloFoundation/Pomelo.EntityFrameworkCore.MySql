// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using MySqlConnector;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Storage.Internal
{
    public class MySqlRelationalConnection : RelationalConnection, IMySqlRelationalConnection
    {
        private const string NoBackslashEscapes = "NO_BACKSLASH_ESCAPES";

        private readonly MySqlOptionsExtension _mySqlOptionsExtension;
        private DbDataSource _dataSource;

        // ReSharper disable once VirtualMemberCallInConstructor
        public MySqlRelationalConnection(RelationalConnectionDependencies dependencies, IMySqlOptions mySqlSingletonOptions)
            : this(dependencies, mySqlSingletonOptions.DataSource)
        {
            _mySqlOptionsExtension = Dependencies.ContextOptions.FindExtension<MySqlOptionsExtension>() ?? new MySqlOptionsExtension();
            _dataSource = mySqlSingletonOptions.DataSource;
        }

        public MySqlRelationalConnection(RelationalConnectionDependencies dependencies, DbDataSource dataSource)
            : base(dependencies)
        {
            _mySqlOptionsExtension = Dependencies.ContextOptions.FindExtension<MySqlOptionsExtension>() ?? new MySqlOptionsExtension();
            _dataSource = dataSource;
        }

        // TODO: Remove, because we don't use it anywhere.
        private bool IsMasterConnection { get; set; }

        protected override DbConnection CreateDbConnection()
            => _dataSource is not null
                ? _dataSource.CreateConnection()
                : new MySqlConnection(AddConnectionStringOptions(new MySqlConnectionStringBuilder(ConnectionString!)).ConnectionString);

        public override string ConnectionString
        {
            get => _dataSource is null ? base.ConnectionString : _dataSource.ConnectionString;
            set
            {
                base.ConnectionString = value;

                _dataSource = null;
            }
        }

        [AllowNull]
        public new virtual MySqlConnection DbConnection
        {
            get => (MySqlConnection)base.DbConnection;
            set
            {
                base.DbConnection = value;

                _dataSource = null;
            }
        }

        public virtual DbDataSource DbDataSource
        {
            get => _dataSource;
            set
            {
                DbConnection = null;
                ConnectionString = null;
                _dataSource = value;
            }
        }

        public virtual IMySqlRelationalConnection CreateMasterConnection()
        {
            if (Dependencies.ContextOptions.FindExtension<MySqlOptionsExtension>() is not { } mySqlOptions)
            {
                throw new InvalidOperationException($"{nameof(MySqlOptionsExtension)} not found in {nameof(CreateMasterConnection)}");
            }

            // Add master connection specific options.
            var csb = new MySqlConnectionStringBuilder(ConnectionString!)
            {
                Database = string.Empty,
                Pooling = false,
            };

            csb = AddConnectionStringOptions(csb);

            var masterConnectionString = csb.ConnectionString;

            // Apply modified connection string.
            var masterMySqlOptions = _dataSource is not null
                ? mySqlOptions.WithConnection(((MySqlConnection)CreateDbConnection()).CloneWith(masterConnectionString))
                : mySqlOptions.Connection is null
                    ? mySqlOptions.WithConnectionString(masterConnectionString)
                    : mySqlOptions.WithConnection(DbConnection.CloneWith(masterConnectionString));

            var optionsBuilder = new DbContextOptionsBuilder();
            var optionsBuilderInfrastructure = (IDbContextOptionsBuilderInfrastructure)optionsBuilder;

            optionsBuilderInfrastructure.AddOrUpdateExtension(masterMySqlOptions);

            return new MySqlRelationalConnection(Dependencies with { ContextOptions = optionsBuilder.Options }, dataSource: null)
            {
                IsMasterConnection = true
            };
        }

        protected virtual MySqlConnectionStringBuilder AddConnectionStringOptions(MySqlConnectionStringBuilder builder)
        {
            if (CommandTimeout != null)
            {
                builder.DefaultCommandTimeout = (uint)CommandTimeout.Value;
            }

            if (_mySqlOptionsExtension.NoBackslashEscapes)
            {
                builder.NoBackslashEscapes = true;
            }

            var boolHandling = _mySqlOptionsExtension.DefaultDataTypeMappings?.ClrBoolean;
            switch (boolHandling)
            {
                case null:
                    // Just keep using whatever is already defined in the connection string.
                    break;

                case MySqlBooleanType.Default:
                case MySqlBooleanType.TinyInt1:
                    builder.TreatTinyAsBoolean = true;
                    break;

                case MySqlBooleanType.None:
                case MySqlBooleanType.Bit1:
                    builder.TreatTinyAsBoolean = false;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            return builder;
        }

        protected override bool SupportsAmbientTransactions => true;

        // CHECK: Is this obsolete or has it been moved somewhere else?
        // public override bool IsMultipleActiveResultSetsEnabled => false;

        public override void EnlistTransaction(Transaction transaction)
        {
            try
            {
                base.EnlistTransaction(transaction);
            }
            catch (MySqlException e)
            {
                if (e.Message == "Already enlisted in a Transaction.")
                {
                    // Return expected exception type.
                    throw new InvalidOperationException(e.Message, e);
                }

                throw;
            }
        }

        public override bool Open(bool errorsExpected = false)
        {
            var result = base.Open(errorsExpected);

            if (result)
            {
                if (_mySqlOptionsExtension.UpdateSqlModeOnOpen && _mySqlOptionsExtension.NoBackslashEscapes)
                {
                    AddSqlMode(NoBackslashEscapes);
                }
            }

            return result;
        }

        public override async Task<bool> OpenAsync(CancellationToken cancellationToken, bool errorsExpected = false)
        {
            var result = await base.OpenAsync(cancellationToken, errorsExpected)
                .ConfigureAwait(false);

            if (result)
            {
                if (_mySqlOptionsExtension.UpdateSqlModeOnOpen && _mySqlOptionsExtension.NoBackslashEscapes)
                {
                    await AddSqlModeAsync(NoBackslashEscapes)
                        .ConfigureAwait(false);
                }
            }

            return result;
        }

        public virtual void AddSqlMode(string mode)
            => Dependencies.CurrentContext.Context?.Database.ExecuteSqlInterpolated($@"SET SESSION sql_mode = CONCAT(@@sql_mode, ',', {mode});");

        public virtual Task AddSqlModeAsync(string mode, CancellationToken cancellationToken = default)
            => Dependencies.CurrentContext.Context?.Database.ExecuteSqlInterpolatedAsync($@"SET SESSION sql_mode = CONCAT(@@sql_mode, ',', {mode});", cancellationToken);

        public virtual void RemoveSqlMode(string mode)
            => Dependencies.CurrentContext.Context?.Database.ExecuteSqlInterpolated($@"SET SESSION sql_mode = REPLACE(@@sql_mode, {mode}, '');");

        public virtual void RemoveSqlModeAsync(string mode, CancellationToken cancellationToken = default)
            => Dependencies.CurrentContext.Context?.Database.ExecuteSqlInterpolatedAsync($@"SET SESSION sql_mode = REPLACE(@@sql_mode, {mode}, '');", cancellationToken);

    }
}
