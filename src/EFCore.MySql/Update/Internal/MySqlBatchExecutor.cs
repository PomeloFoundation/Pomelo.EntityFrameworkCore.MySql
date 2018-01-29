// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;

//ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Update.Internal
{

    public class MySqlBatchExecutor : IBatchExecutor
    {

        public int Execute(
            IEnumerable<ModificationCommandBatch> commandBatches,
            IRelationalConnection connection)
        {
            var rowsAffected = 0;
            connection.Open();
            IDbContextTransaction startedTransaction = null;
            try
            {
                if (connection.CurrentTransaction == null)
                {
                	startedTransaction = connection.BeginTransaction();
                }

                foreach (var commandbatch in commandBatches)
                {
                    commandbatch.Execute(connection);
                    rowsAffected += commandbatch.ModificationCommands.Count;
                }
                startedTransaction?.Commit();
                startedTransaction?.Dispose();
            }
            catch
            {
                try
                {
                    startedTransaction?.Rollback();
                    startedTransaction?.Dispose();
                }
                catch
                {
                    // if the connection was lost, rollback command will fail.  prefer to throw original exception in that case
                }
                throw;
            }
            finally
            {
                connection.Close();
            }

            return rowsAffected;
        }

        public async Task<int> ExecuteAsync(
            IEnumerable<ModificationCommandBatch> commandBatches,
            IRelationalConnection connection,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var rowsAffected = 0;
            await connection.OpenAsync(cancellationToken, false).ConfigureAwait(false);
            MySqlRelationalTransaction startedTransaction = null;
            try
            {
                if (connection.CurrentTransaction == null)
                {
                    // ReSharper disable once PossibleNullReferenceException
                    startedTransaction = await (connection as MySqlRelationalConnection).BeginTransactionAsync(cancellationToken).ConfigureAwait(false) as MySqlRelationalTransaction;
                }

                foreach (var commandbatch in commandBatches)
                {
                    await commandbatch.ExecuteAsync(connection, cancellationToken).ConfigureAwait(false);
                    rowsAffected += commandbatch.ModificationCommands.Count;
                }

                if (startedTransaction != null)
                {
                  await startedTransaction.CommitAsync(cancellationToken).ConfigureAwait(false);
                }
                startedTransaction?.Dispose();
            }
            catch
            {
                try
                {
                    startedTransaction?.Rollback();
                    startedTransaction?.Dispose();
                }
                catch
                {
                    // if the connection was lost, rollback command will fail.  prefer to throw original exception in that case
                }
                throw;
            }
            finally
            {
                connection.Close();
            }

            return rowsAffected;
        }
    }
}
