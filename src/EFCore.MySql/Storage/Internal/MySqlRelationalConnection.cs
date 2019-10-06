// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using MySql.Data.MySqlClient;

namespace Pomelo.EntityFrameworkCore.MySql.Storage.Internal
{
    public class MySqlRelationalConnection : RelationalConnection, IMySqlRelationalConnection
    {
        public MySqlRelationalConnection([NotNull] RelationalConnectionDependencies dependencies)
            : base(dependencies)
        {
        }

        protected override DbConnection CreateDbConnection()
            => new MySqlConnection(AddConnectionStringOptions(new MySqlConnectionStringBuilder(ConnectionString)).ConnectionString);

        public virtual IMySqlRelationalConnection CreateMasterConnection()
        {
            var connectionStringBuilder = new MySqlConnectionStringBuilder(ConnectionString)
            {
                Database = "",
                Pooling = false
            };

            var optionsBuilder = new DbContextOptionsBuilder()
                .UseMySql(AddConnectionStringOptions(connectionStringBuilder).ConnectionString, options => options.CommandTimeout(CommandTimeout));

            return new MySqlRelationalConnection(Dependencies.With(optionsBuilder.Options));
        }

        private MySqlConnectionStringBuilder AddConnectionStringOptions(MySqlConnectionStringBuilder builder)
        {
            if (CommandTimeout != null)
                builder.DefaultCommandTimeout = (uint)CommandTimeout.Value;

            return builder;
        }

        protected override bool SupportsAmbientTransactions => false;
        public override bool IsMultipleActiveResultSetsEnabled => false;

        public override async Task<IDbContextTransaction> BeginTransactionAsync(
            System.Data.IsolationLevel isolationLevel,
            CancellationToken cancellationToken = default(CancellationToken))
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
            System.Data.IsolationLevel isolationLevel, CancellationToken cancellationToken = default(CancellationToken))
        {
            var dbTransaction = await ((MySqlConnection)DbConnection).BeginTransactionAsync(isolationLevel, cancellationToken)
                .ConfigureAwait(false);

            CurrentTransaction = Dependencies.RelationalTransactionFactory.Create(
                this,
                dbTransaction,
                Dependencies.TransactionLogger,
                transactionOwned: true);

            Dependencies.TransactionLogger.TransactionStarted(
                this,
                dbTransaction,
                CurrentTransaction.TransactionId,
                DateTimeOffset.UtcNow);

            return CurrentTransaction;
        }

        public virtual async Task CommitTransactionAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (CurrentTransaction == null)
            {
                throw new InvalidOperationException(RelationalStrings.NoActiveTransaction);
            }

            await ((MySqlRelationalTransaction)CurrentTransaction).CommitAsync(cancellationToken).ConfigureAwait(false);
        }

        public virtual async Task RollbackTransactionAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (CurrentTransaction == null)
            {
                throw new InvalidOperationException(RelationalStrings.NoActiveTransaction);
            }

            await ((MySqlRelationalTransaction)CurrentTransaction).RollbackAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
