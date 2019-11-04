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
using MySql.Data.MySqlClient;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Pomelo.EntityFrameworkCore.MySql.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Storage.Internal
{
    public class MySqlRelationalConnection : RelationalConnection, IMySqlRelationalConnection
    {
        private const string NoBackslashEscapes = "NO_BACKSLASH_ESCAPES";

        private readonly IServiceProvider _serviceProvider;
        private readonly MySqlOptionsExtension _mySqlOptionsExtension;

        // ReSharper disable once VirtualMemberCallInConstructor
        public MySqlRelationalConnection(
            [NotNull] RelationalConnectionDependencies dependencies,
            IServiceProvider serviceProvider)
            : base(dependencies)
        {
            _serviceProvider = serviceProvider;
            _mySqlOptionsExtension = Dependencies.ContextOptions.FindExtension<MySqlOptionsExtension>() ?? new MySqlOptionsExtension();
        }

        private bool IsMasterConnection { get; set; }

        protected override DbConnection CreateDbConnection()
            => new MySqlConnection(AddConnectionStringOptions(new MySqlConnectionStringBuilder(ConnectionString)).ConnectionString);

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual IMySqlRelationalConnection CreateMasterConnection()
        {
            var relationalOptions = RelationalOptionsExtension.Extract(Dependencies.ContextOptions);

            MySqlConnection connection;
            string connectionString;

            // Make sure to get the original connection string including security sensitive information.
            if (relationalOptions.Connection == null)
            {
                connection = null;
                connectionString = ConnectionString;
            }
            else
            {
                connection = ((MySqlConnection)DbConnection).Clone();
                connectionString = connection.ConnectionString;
            }

            // Add master connection specific options.
            var csb = new MySqlConnectionStringBuilder(connectionString) {
                Database = string.Empty,
                Pooling = false
            };

            csb = AddConnectionStringOptions(csb);

            // Apply modified connection string.
            if (connection == null)
            {
                relationalOptions = relationalOptions.WithConnectionString(csb.ConnectionString);
            }
            else
            {
                connection.ConnectionString = csb.ConnectionString;
                relationalOptions = relationalOptions.WithConnection(connection);
            }

            var optionsBuilder = new DbContextOptionsBuilder();
            var optionsBuilderInfrastructure = (IDbContextOptionsBuilderInfrastructure)optionsBuilder;

            optionsBuilderInfrastructure.AddOrUpdateExtension(relationalOptions);
            optionsBuilderInfrastructure.AddOrUpdateExtension(new MySqlOptionsExtension(_mySqlOptionsExtension));

            return new MySqlRelationalConnection(Dependencies.With(optionsBuilder.Options), _serviceProvider)
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
        public override bool IsMultipleActiveResultSetsEnabled => false;

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
                MySqlConnectionInfo.SetServerVersion((MySqlConnection)DbConnection, _serviceProvider);

                if (_mySqlOptionsExtension.UpdateSqlModeOnOpen && _mySqlOptionsExtension.NoBackslashEscapes)
                {
                    AppendToSqlMode(NoBackslashEscapes);
                }
            }

            return result;
        }

        public override async Task<bool> OpenAsync(CancellationToken cancellationToken, bool errorsExpected = false)
        {
            var result = await base.OpenAsync(cancellationToken, errorsExpected);

            if (result)
            {
                MySqlConnectionInfo.SetServerVersion((MySqlConnection)DbConnection, _serviceProvider);

                if (_mySqlOptionsExtension.UpdateSqlModeOnOpen && _mySqlOptionsExtension.NoBackslashEscapes)
                {
                    await AppendToSqlModeAsync(NoBackslashEscapes);
                }
            }

            return result;
        }

        public virtual void AppendToSqlMode(string mode)
            => Dependencies.CurrentContext.Context?.Database.ExecuteSqlRaw(@"SET SESSION sql_mode = CONCAT(@@sql_mode, ',', @p0)", new MySqlParameter("@p0", mode));

        public virtual Task AppendToSqlModeAsync(string mode)
            => Dependencies.CurrentContext.Context?.Database.ExecuteSqlRawAsync(@"SET SESSION sql_mode = CONCAT(@@sql_mode, ',', @p0)", new MySqlParameter("@p0", mode));
    }
}
