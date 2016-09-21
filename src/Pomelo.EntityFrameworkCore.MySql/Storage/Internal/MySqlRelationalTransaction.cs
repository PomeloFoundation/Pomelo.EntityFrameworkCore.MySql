using System;
using System.Data.Common;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Utilities;
using MySql.Data.MySqlClient;

namespace Microsoft.EntityFrameworkCore.Storage.Internal
{
    public class MySqlRelationalTransaction : IDbContextTransaction, IInfrastructure<DbTransaction>
    {
        private readonly IRelationalConnection _relationalConnection;
        private readonly MySqlTransaction _dbTransaction;
        private readonly bool _transactionOwned;

        private bool _disposed;

        public MySqlRelationalTransaction(
            [NotNull] IRelationalConnection connection,
            [NotNull] MySqlTransaction transaction,
            bool transactionOwned)
        {
            Check.NotNull(connection, nameof(connection));
            Check.NotNull(transaction, nameof(transaction));

            if (connection.DbConnection != transaction.Connection)
            {
                throw new InvalidOperationException(RelationalStrings.TransactionAssociatedWithDifferentConnection);
            }

            _relationalConnection = connection;

            _dbTransaction = transaction;
            _transactionOwned = transactionOwned;
        }

        public void Commit()
        {
            _dbTransaction.Commit();
            ClearTransaction();
        }

	    public async Task CommitAsync(CancellationToken cancellationToken = default(CancellationToken))
	    {
		    await _dbTransaction.CommitAsync(cancellationToken).ConfigureAwait(false);
		    ClearTransaction();
	    }

        public void Rollback()
        {
            _dbTransaction.Rollback();
            ClearTransaction();
        }

	    public async Task RollbackAsync(CancellationToken cancellationToken = default(CancellationToken))
	    {
		    await _dbTransaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
		    ClearTransaction();
	    }

	    public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                if (_transactionOwned)
                {
                    _dbTransaction.Dispose();
                }
                ClearTransaction();
            }
        }

        private void ClearTransaction()
        {
            Debug.Assert((_relationalConnection.CurrentTransaction == null) ||
                         (_relationalConnection.CurrentTransaction == this));
            _relationalConnection.UseTransaction(null);
        }

        DbTransaction IInfrastructure<DbTransaction>.Instance => _dbTransaction;
    }
}
