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
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;

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
            _mySqlOptionsExtension = Dependencies.ContextOptions.FindExtension<MySqlOptionsExtension>();
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
            var connectionStringBuilder = new MySqlConnectionStringBuilder(ConnectionString)
            {
                Database = "",
                Pooling = false
            };

            var optionsBuilder = new DbContextOptionsBuilder()
                .UseMySql(AddConnectionStringOptions(connectionStringBuilder).ConnectionString, options => options.CommandTimeout(CommandTimeout));

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
                SetServerVersion();

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
                SetServerVersion();

                if (_mySqlOptionsExtension.UpdateSqlModeOnOpen && _mySqlOptionsExtension.NoBackslashEscapes)
                {
                    await AppendToSqlModeAsync(NoBackslashEscapes);
                }
            }

            return result;
        }

        private void SetServerVersion()
        {
            var connectionInfo = (MySqlConnectionInfo)_serviceProvider.GetRequiredService<IMySqlConnectionInfo>();
            var options = _serviceProvider.GetRequiredService<IMySqlOptions>();

            if (options.ServerVersion.IsDefault)
            {
                var connection = (MySqlConnection)DbConnection;
                connectionInfo.ServerVersion = new ServerVersion(connection.ServerVersion);
            }
            else
            {
                connectionInfo.ServerVersion = options.ServerVersion;
            }
        }

        public virtual void AppendToSqlMode(string mode)
            => Dependencies.CurrentContext.Context?.Database.ExecuteSqlRaw(@"SET SESSION sql_mode = CONCAT(@@sql_mode, ',', @p0)", new MySqlParameter("@p0", mode));

        public virtual Task AppendToSqlModeAsync(string mode)
            => Dependencies.CurrentContext.Context?.Database.ExecuteSqlRawAsync(@"SET SESSION sql_mode = CONCAT(@@sql_mode, ',', @p0)", new MySqlParameter("@p0", mode));
    }
}
