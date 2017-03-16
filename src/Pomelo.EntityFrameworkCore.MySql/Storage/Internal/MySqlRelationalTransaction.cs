using System;
using System.Data.Common;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Utilities;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using System.Data;

namespace Microsoft.EntityFrameworkCore.Storage.Internal
{
    public class MySqlRelationalTransaction : IDbContextTransaction, IInfrastructure<DbTransaction>
    {
        private readonly IRelationalConnection _relationalConnection;
        private readonly MySqlTransaction _dbTransaction;
	    private readonly ILogger _logger;
	    private readonly bool _transactionOwned;

        private bool _disposed;

        public MySqlRelationalTransaction(
            [NotNull] IRelationalConnection connection,
            [NotNull] MySqlTransaction transaction,
            [NotNull] ILogger logger,
            bool transactionOwned)
        {
            Check.NotNull(connection, nameof(connection));
            Check.NotNull(transaction, nameof(transaction));
	        Check.NotNull(logger, nameof(logger));

	        if (connection.DbConnection != transaction.Connection)
            {
                throw new InvalidOperationException(RelationalStrings.TransactionAssociatedWithDifferentConnection);
            }

            _relationalConnection = connection;
            _dbTransaction = transaction;
	        _logger = logger;
	        _transactionOwned = transactionOwned;
        }

        public void Commit()
        {
	        _logger.LogDebug(
		        RelationalEventId.CommittingTransaction,
		        () => RelationalStrings.RelationalLoggerCommittingTransaction);

	        _dbTransaction.Commit();
            ClearTransaction();
        }

	    public async Task CommitAsync(CancellationToken cancellationToken = default(CancellationToken))
	    {
		    _logger.LogDebug(
			    RelationalEventId.CommittingTransaction,
			    () => RelationalStrings.RelationalLoggerCommittingTransaction);

		    await _dbTransaction.CommitAsync(cancellationToken).ConfigureAwait(false);
		    ClearTransaction();
	    }

        public void Rollback()
        {
	        _logger.LogDebug(
		        RelationalEventId.RollingbackTransaction,
		        () => RelationalStrings.RelationalLoggerRollingbackTransaction);

	        _dbTransaction.Rollback();
            ClearTransaction();
        }

	    public async Task RollbackAsync(CancellationToken cancellationToken = default(CancellationToken))
	    {
		    _logger.LogDebug(
			    RelationalEventId.RollingbackTransaction,
			    () => RelationalStrings.RelationalLoggerRollingbackTransaction);

		    await _dbTransaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
		    ClearTransaction();
	    }

	    public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                if (_transactionOwned && _dbTransaction.Connection.State == ConnectionState.Closed)
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
