using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MySql.Data.MySqlClient;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Storage.Internal
{
    public class MySqlRelationalCommand : RelationalCommand
    {
        public MySqlRelationalCommand(
            [NotNull] ISensitiveDataLogger logger,
            [NotNull] DiagnosticSource diagnosticSource,
            [NotNull] string commandText,
            [NotNull] IReadOnlyList<IRelationalParameter> parameters)
            : base(logger, diagnosticSource, commandText, parameters)
        {
        }

        protected override object Execute(
            [NotNull] IRelationalConnection connection,
            [NotNull] string executeMethod,
            [CanBeNull] IReadOnlyDictionary<string, object> parameterValues,
            bool openConnection,
            bool closeConnection)
        {
            return ExecuteAsync(connection, executeMethod, parameterValues, openConnection, closeConnection)
                .GetAwaiter()
                .GetResult();
        }

        protected override async Task<object> ExecuteAsync(
            [NotNull] IRelationalConnection connection,
            [NotNull] string executeMethod,
            [CanBeNull] IReadOnlyDictionary<string, object> parameterValues,
            bool openConnection,
            bool closeConnection,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            var mySqlConnection = connection as MySqlRelationalConnection;
            var locked = false;
            object result = null;
            try
            {
                if (mySqlConnection != null)
                {
                    await mySqlConnection.Lock.WaitAsync(cancellationToken).ConfigureAwait(false);
                    locked = true;
                }
                result = await base.ExecuteAsync(
                    connection,
                    executeMethod,
                    parameterValues,
                    openConnection,
                    closeConnection,
                    cancellationToken)
                    .ConfigureAwait(false);
            }
            finally
            {
                if (locked)
                {
                    if (executeMethod == nameof(ExecuteReader) && result != null)
                    {
                        // if calling 'ExecuteReader', transfer ownership of the Semaphore to it until it is disposed
                        result = new SynchronizedMySqlDataReader((MySqlDataReader) result, mySqlConnection.Lock);
                    }
                    else
                    {
                        // if calling any other method, the command has finished executing and the lock can be released immediately
                        mySqlConnection.Lock.Release();
                    }
                }
            }
            return result;
        }

    }
}
