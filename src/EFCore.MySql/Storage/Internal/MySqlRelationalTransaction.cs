// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Data.Common;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using MySqlConnector;

namespace Pomelo.EntityFrameworkCore.MySql.Storage.Internal
{
    public class MySqlRelationalTransaction : RelationalTransaction
    {
        private readonly IRelationalConnection _relationalConnection;
        private readonly IDiagnosticsLogger<DbLoggerCategory.Database.Transaction> _logger;
        private bool _connectionClosed;

        public MySqlRelationalTransaction(
            [NotNull] IRelationalConnection connection,
            [NotNull] DbTransaction transaction,
            [NotNull] Guid transactionId,
            [NotNull] IDiagnosticsLogger<DbLoggerCategory.Database.Transaction> logger,
            bool transactionOwned)
            : base(connection, transaction, transactionId, logger, transactionOwned)
        {
            if (connection.DbConnection != transaction.Connection)
            {
                throw new InvalidOperationException(RelationalStrings.TransactionAssociatedWithDifferentConnection);
            }

            _relationalConnection = connection;
            _logger = logger;
        }

        private DbTransaction DbTransaction => ((IInfrastructure<DbTransaction>)this).Instance;

        public override async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            var startTime = DateTimeOffset.UtcNow;
            var stopwatch = Stopwatch.StartNew();

            try
            {
                if (DbTransaction is MySqlTransaction transaction)
                {
                    await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    DbTransaction.Commit();
                }

                _logger.TransactionCommitted(
                    _relationalConnection,
                    DbTransaction,
                    TransactionId,
                    startTime,
                    stopwatch.Elapsed);
            }
            catch (Exception e)
            {
                _logger.TransactionError(
                    _relationalConnection,
                    DbTransaction,
                    TransactionId,
                    "CommitAsync",
                    e,
                    startTime,
                    stopwatch.Elapsed);
                throw;
            }

            ClearTransaction();
        }

        /// <summary>
        ///     Discards all changes made to the database in the current transaction.
        /// </summary>
        public override async Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            var startTime = DateTimeOffset.UtcNow;
            var stopwatch = Stopwatch.StartNew();

            try
            {
                if (DbTransaction is MySqlTransaction transaction)
                {
                    await transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    DbTransaction.Rollback();
                }

                _logger.TransactionRolledBack(
                    _relationalConnection,
                    DbTransaction,
                    TransactionId,
                    startTime,
                    stopwatch.Elapsed);
            }
            catch (Exception e)
            {
                _logger.TransactionError(
                    _relationalConnection,
                    DbTransaction,
                    TransactionId,
                    "RollbackAsync",
                    e,
                    startTime,
                    stopwatch.Elapsed);
                throw;
            }

            ClearTransaction();
        }

        protected override void ClearTransaction()
        {
            Debug.Assert(_relationalConnection.CurrentTransaction == null || _relationalConnection.CurrentTransaction == this);

            _relationalConnection.UseTransaction(null);

            if (!_connectionClosed)
            {
                _connectionClosed = true;

                _relationalConnection.Close();
            }
        }
    }
}
