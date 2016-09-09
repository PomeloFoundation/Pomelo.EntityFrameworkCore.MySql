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
            object result;
            try
            {
                if (mySqlConnection != null)
                {
                    await mySqlConnection.Lock.WaitAsync(cancellationToken).ConfigureAwait(false);
                    locked = true;
                }
                if (executeMethod == nameof(ExecuteReader) && mySqlConnection != null)
                {
                    await mySqlConnection.ActiveReader.WaitDoneReadingAsync(cancellationToken).ConfigureAwait(false);
                    mySqlConnection.ActiveReader.Dispose();
                    mySqlConnection.ActiveReader = null;
                }
                result = await base.ExecuteAsync(
                    connection,
                    executeMethod,
                    parameterValues,
                    openConnection,
                    closeConnection,
                    cancellationToken)
                    .ConfigureAwait(false);
                if (executeMethod == nameof(ExecuteReader) && mySqlConnection != null)
                {
                    mySqlConnection.ActiveReader = ((RelationalDataReader) result).DbDataReader as MySqlDataReader;
                }
            }
            finally
            {
                if (locked)
                {
                    mySqlConnection.Lock.Release();
                }
            }
            return result;
        }

    }
}