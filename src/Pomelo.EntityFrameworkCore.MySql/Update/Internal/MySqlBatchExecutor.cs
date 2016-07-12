// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Storage;
using Pomelo.Data.MySql;

namespace Microsoft.EntityFrameworkCore.Update.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used 
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class MySqlBatchExecutor : IBatchExecutor
    {
        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used 
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual int Execute(
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
                    // Fixed Issue #1: DataReader conflicted when added multiple entities
                    try
                    {
                        (connection.DbConnection as MySqlConnection).Reader.Dispose();
                        (connection.DbConnection as MySqlConnection).Reader = null;
                    }
                    catch
                    {
                    }
                    rowsAffected += commandbatch.ModificationCommands.Count;
                }

                startedTransaction?.Commit();
            }
            finally
            {
                startedTransaction?.Dispose();
                connection.Close();
            }

            return rowsAffected;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used 
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual async Task<int> ExecuteAsync(
            IEnumerable<ModificationCommandBatch> commandBatches,
            IRelationalConnection connection,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var rowsAffected = 0;
            await connection.OpenAsync(cancellationToken);
            IDbContextTransaction startedTransaction = null;
            try
            {
                if (connection.CurrentTransaction == null)
                {
                    startedTransaction = connection.BeginTransaction();
                }

                foreach (var commandbatch in commandBatches)
                {
                    await commandbatch.ExecuteAsync(connection, cancellationToken);
                    rowsAffected += commandbatch.ModificationCommands.Count;
                }

                startedTransaction?.Commit();
            }
            finally
            {
                startedTransaction?.Dispose();
                connection.Close();
            }

            return rowsAffected;
        }
    }
}
