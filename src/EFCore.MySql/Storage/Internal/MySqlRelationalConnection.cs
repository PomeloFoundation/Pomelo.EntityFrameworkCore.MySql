// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using MySqlConnector;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Pomelo.EntityFrameworkCore.MySql.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Storage.Internal
{
    public class MySqlRelationalConnection : RelationalConnection, IMySqlRelationalConnection
    {
        private const string NoBackslashEscapes = "NO_BACKSLASH_ESCAPES";

        private readonly MySqlOptionsExtension _mySqlOptionsExtension;

        // ReSharper disable once VirtualMemberCallInConstructor
        public MySqlRelationalConnection(
            [NotNull] RelationalConnectionDependencies dependencies)
            : base(dependencies)
        {
            _mySqlOptionsExtension = Dependencies.ContextOptions.FindExtension<MySqlOptionsExtension>() ?? new MySqlOptionsExtension();
        }

        private bool IsMasterConnection { get; set; }

        protected override DbConnection CreateDbConnection()
            => new MySqlConnection(AddConnectionStringOptions(new MySqlConnectionStringBuilder(ConnectionString)).ConnectionString);

        public virtual IMySqlRelationalConnection CreateMasterConnection()
        {
            var relationalOptions = RelationalOptionsExtension.Extract(Dependencies.ContextOptions);
            var connection = (MySqlConnection)relationalOptions.Connection;
            var connectionString = connection?.ConnectionString ?? relationalOptions.ConnectionString;

            // Add master connection specific options.
            var csb = new MySqlConnectionStringBuilder(connectionString)
            {
                Database = string.Empty,
                Pooling = false
            };

            csb = AddConnectionStringOptions(csb);

            // Apply modified connection string.
            relationalOptions = connection is null
                ? relationalOptions.WithConnectionString(csb.ConnectionString)
                : relationalOptions.WithConnection(connection.CloneWith(csb.ConnectionString));

            var optionsBuilder = new DbContextOptionsBuilder();
            var optionsBuilderInfrastructure = (IDbContextOptionsBuilderInfrastructure)optionsBuilder;

            optionsBuilderInfrastructure.AddOrUpdateExtension(relationalOptions);

            return new MySqlRelationalConnection(Dependencies with { ContextOptions = optionsBuilder.Options })
            {
                IsMasterConnection = true
            };
        }

        private MySqlConnectionStringBuilder AddConnectionStringOptions(MySqlConnectionStringBuilder builder)
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
