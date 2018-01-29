using System.Data.Common;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using JetBrains.Annotations;
using System.Data;
using System.Threading;
using System;
using Microsoft.EntityFrameworkCore.Internal;

//ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Storage.Internal
{
    public class MySqlRelationalConnection : RelationalConnection, IMySqlRelationalConnection
    {
        
        public MySqlRelationalConnection([NotNull] RelationalConnectionDependencies dependencies)
            : base(dependencies)
        {
        }

        protected override DbConnection CreateDbConnection() => new MySqlConnection(ConnectionString);

        public virtual IMySqlRelationalConnection CreateMasterConnection()
        {
            var csb = new MySqlConnectionStringBuilder(ConnectionString)
            {
                Database = "",
                Pooling = false
            };

            var contextOptions = new DbContextOptionsBuilder()
                .UseMySql(csb.ConnectionString)
                .Options;
                
            return new MySqlRelationalConnection(Dependencies.With(contextOptions));
        }

        public override bool IsMultipleActiveResultSetsEnabled => false;

        [NotNull]
        public override async Task<IDbContextTransaction> BeginTransactionAsync(
            IsolationLevel isolationLevel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (CurrentTransaction != null)
            {
                throw new InvalidOperationException(RelationalStrings.TransactionAlreadyStarted);
            }

            await OpenAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

            return await BeginTransactionWithNoPreconditionsAsync(isolationLevel, cancellationToken).ConfigureAwait(false);
        }

        private async Task<IDbContextTransaction> BeginTransactionWithNoPreconditionsAsync(IsolationLevel isolationLevel, CancellationToken cancellationToken=default(CancellationToken))
        {
            DbTransaction dbTransaction = null;
            if (DbConnection is MySqlConnection mySqlConnection)
            {
                dbTransaction = await mySqlConnection.BeginTransactionAsync(isolationLevel).ConfigureAwait(false);
            }
            else
            {
                dbTransaction = DbConnection.BeginTransaction(isolationLevel);
            }            

            CurrentTransaction
                = new MySqlRelationalTransaction(
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

        /// <summary>
        ///     Specifies an existing <see cref="DbTransaction" /> to be used for database operations.
        /// </summary>
        /// <param name="transaction"> The transaction to be used. </param>
        public override IDbContextTransaction UseTransaction(DbTransaction transaction)
        {
            if (transaction == null)
            {
                if (CurrentTransaction != null)
                {
                    CurrentTransaction = null;
                }
            }
            else
            {
                if (CurrentTransaction != null)
                {
                    throw new InvalidOperationException(RelationalStrings.TransactionAlreadyStarted);
                }

                Open();

                CurrentTransaction = new MySqlRelationalTransaction(
                    this, 
                    transaction, 
                    Dependencies.TransactionLogger, 
                    transactionOwned: false);

                Dependencies.TransactionLogger.TransactionUsed(
                    this, 
                    transaction, 
                    CurrentTransaction.TransactionId,
                    DateTimeOffset.UtcNow);
            }

            return CurrentTransaction;
        }

        public virtual async Task CommitTransactionAsync(CancellationToken cancellationToken=default(CancellationToken))
        {
            if (CurrentTransaction == null)
            {
                throw new InvalidOperationException(RelationalStrings.NoActiveTransaction);
            }

            await (CurrentTransaction as MySqlRelationalTransaction).CommitAsync().ConfigureAwait(false);
        }

        public virtual async Task RollbackTransactionAsync(CancellationToken cancellationToken=default(CancellationToken))
        {
            if (CurrentTransaction == null)
            {
                throw new InvalidOperationException(RelationalStrings.NoActiveTransaction);
            }

            await (CurrentTransaction as MySqlRelationalTransaction).RollbackAsync().ConfigureAwait(false);
        }
    }
}
