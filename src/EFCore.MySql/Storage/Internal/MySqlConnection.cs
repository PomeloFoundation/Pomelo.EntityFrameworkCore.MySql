// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using MySql.Data.MySqlClient;

namespace Pomelo.EntityFrameworkCore.MySql.Storage.Internal
{
    public class MySqlConnection : RelationalConnection, IMySqlConnection
    {
        public MySqlConnection([NotNull] RelationalConnectionDependencies dependencies)
            : base(dependencies)
        {
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        protected override DbConnection CreateDbConnection() => new global::MySql.Data.MySqlClient.MySqlConnection(base.ConnectionString);

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual IMySqlConnection CreateMasterConnection()
        {
            var csb = new MySqlConnectionStringBuilder(ConnectionString)
            {
                Database = "",
                Pooling = false
            };

            var contextOptions = new DbContextOptionsBuilder()
                .UseMySql(csb.ConnectionString)
                .Options;

            return new MySqlConnection(Dependencies.With(contextOptions));
        }

        protected override bool SupportsAmbientTransactions => false;

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public override bool IsMultipleActiveResultSetsEnabled => false;

        public override async Task<IDbContextTransaction> BeginTransactionAsync(
            System.Data.IsolationLevel isolationLevel,
            CancellationToken cancellationToken = default)
        {
            await OpenAsync(cancellationToken).ConfigureAwait(false);

            if (CurrentTransaction != null)
            {
                throw new InvalidOperationException(RelationalStrings.TransactionAlreadyStarted);
            }

            if (Transaction.Current != null)
            {
                throw new InvalidOperationException(RelationalStrings.ConflictingAmbientTransaction);
            }

            if (EnlistedTransaction != null)
            {
                throw new InvalidOperationException(RelationalStrings.ConflictingEnlistedTransaction);
            }

            return await BeginTransactionWithNoPreconditionsAsync(isolationLevel, cancellationToken).ConfigureAwait(false);
        }

        private async Task<IDbContextTransaction> BeginTransactionWithNoPreconditionsAsync(
            System.Data.IsolationLevel isolationLevel, CancellationToken cancellationToken = default)
        {
            var dbTransaction = await ((global::MySql.Data.MySqlClient.MySqlConnection)base.DbConnection).BeginTransactionAsync(isolationLevel, cancellationToken)
                .ConfigureAwait(false);

            var guid = new Guid();

            CurrentTransaction = Dependencies.RelationalTransactionFactory.Create(
                this,
                dbTransaction,
                guid,
                Dependencies.TransactionLogger,
                transactionOwned: true);

            Dependencies.TransactionLogger.TransactionStarted(
                this,
                dbTransaction,
                CurrentTransaction.TransactionId,
                DateTimeOffset.UtcNow,
                new TimeSpan());

            return CurrentTransaction;
        }

        public virtual async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (CurrentTransaction == null)
            {
                throw new InvalidOperationException(RelationalStrings.NoActiveTransaction);
            }

            await ((MySqlRelationalTransaction)CurrentTransaction).CommitAsync(cancellationToken).ConfigureAwait(false);
        }

        public virtual async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (CurrentTransaction == null)
            {
                throw new InvalidOperationException(RelationalStrings.NoActiveTransaction);
            }

            await ((MySqlRelationalTransaction)CurrentTransaction).RollbackAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
