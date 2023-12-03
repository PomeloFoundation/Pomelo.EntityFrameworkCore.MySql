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
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Storage.Internal
{
    public class MySqlRelationalConnection : RelationalConnection, IMySqlRelationalConnection
    {
        private readonly IMySqlConnectionStringOptionsValidator _mySqlConnectionStringOptionsValidator;
        private const string NoBackslashEscapes = "NO_BACKSLASH_ESCAPES";

        private readonly MySqlOptionsExtension _mySqlOptionsExtension;
        private DbDataSource _dataSource;

        public MySqlRelationalConnection(
            RelationalConnectionDependencies dependencies,
            IMySqlConnectionStringOptionsValidator mySqlConnectionStringOptionsValidator,
            IMySqlOptions mySqlSingletonOptions)
            : this(
                dependencies,
                mySqlConnectionStringOptionsValidator,
                GetEffectiveDataSource(mySqlSingletonOptions, dependencies.ContextOptions))
        {
        }

        public MySqlRelationalConnection(
            RelationalConnectionDependencies dependencies,
            IMySqlConnectionStringOptionsValidator mySqlConnectionStringOptionsValidator,
            DbDataSource dataSource)
            : base(dependencies)
        {
            _mySqlOptionsExtension = dependencies.ContextOptions.FindExtension<MySqlOptionsExtension>() ??
                                     new MySqlOptionsExtension();
            _mySqlConnectionStringOptionsValidator = mySqlConnectionStringOptionsValidator;

            if (dataSource is not null)
            {
                _mySqlConnectionStringOptionsValidator.EnsureMandatoryOptions(dataSource);

                base.SetDbConnection(null, false);
                base.ConnectionString = null;

                _dataSource = dataSource;
            }
            else if (base.ConnectionString is { } connectionString)
            {
                // This branch works for both: connections and connection strings, because base.ConnectionString handles both cases
                // appropriately.
                if (_mySqlConnectionStringOptionsValidator.EnsureMandatoryOptions(ref connectionString))
                {
                    try
                    {
                        base.ConnectionString = connectionString;
                    }
                    catch (Exception e)
                    {
                        _mySqlConnectionStringOptionsValidator.ThrowException(e);
                    }
                }
            }
        }

        /// <summary>
        /// We allow users to either explicitly set a DbDataSource using our `MySqlOptionsExtensions` or by adding it as a service via DI
        /// (`ApplicationServiceProvider`).
        /// We don't set a DI injected service to the `MySqlOption.DbDataSource` property, because it might get cached by the service
        /// collection cache, since no relevant property might have changed in the `MySqlOptionsExtension` instance. If we would create
        /// a similar DbContext instance with a different service collection later, EF Core would provide us with the *same* `MySqlOptions`
        /// instance (that was cached before) and we would use the old `DbDataSource` instance that we retrieved from the old
        /// `ApplicationServiceProvider`.
        /// Therefore, we check the `IMySqlOptions.DbDataSource` property and the current `ApplicationServiceProvider` at the time we
        /// actually need the instance.
        /// </summary>
        protected static DbDataSource GetEffectiveDataSource(IMySqlOptions mySqlSingletonOptions, IDbContextOptions contextOptions)
            => mySqlSingletonOptions.DataSource ??
               contextOptions.FindExtension<CoreOptionsExtension>()?.ApplicationServiceProvider?.GetService<MySqlDataSource>();

        // TODO: Remove, because we don't use it anywhere.
        private bool IsMasterConnection { get; set; }

        protected override DbConnection CreateDbConnection()
            => _dataSource is not null
                ? _dataSource.CreateConnection()
                : new MySqlConnection(AddConnectionStringOptions(new MySqlConnectionStringBuilder(ConnectionString!)).ConnectionString);

        public override string ConnectionString
        {
            get => _dataSource is null
                ? base.ConnectionString
                : _dataSource.ConnectionString;
            set
            {
                _mySqlConnectionStringOptionsValidator.EnsureMandatoryOptions(ref value);
                base.ConnectionString = value;

                _dataSource = null;
            }
        }

        public override void SetDbConnection(DbConnection value, bool contextOwnsConnection)
        {
            _mySqlConnectionStringOptionsValidator.EnsureMandatoryOptions(value);

            base.SetDbConnection(value, contextOwnsConnection);
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
                _mySqlConnectionStringOptionsValidator.EnsureMandatoryOptions(value);

                if (value is not null)
                {
                    DbConnection = null;
                    ConnectionString = null;
                }

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

            return new MySqlRelationalConnection(
                Dependencies with { ContextOptions = optionsBuilder.Options },
                _mySqlConnectionStringOptionsValidator,
                dataSource: null)
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
