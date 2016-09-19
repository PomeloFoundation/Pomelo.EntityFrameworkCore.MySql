using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Utilities;
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
            // copied from base method
            Check.NotNull(connection, nameof(connection));
            Check.NotEmpty(executeMethod, nameof(executeMethod));
            var dbCommand = CreateCommand(connection, parameterValues);
            object result = null;
            if (openConnection)
            {
                await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
            }
            // end copied from base method

            cancellationToken.ThrowIfCancellationRequested();
            var mySqlConnection = connection as MySqlRelationalConnection;
            var locked = false;

            try
            {
                if (mySqlConnection != null)
                {
                    await mySqlConnection.Lock.WaitAsync(cancellationToken).ConfigureAwait(false);
                    locked = true;
                }
                // copied from base method
                switch (executeMethod)
                {
                    case nameof(ExecuteNonQuery):
                    {
                        using (dbCommand)
                        {
                            result = await dbCommand.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                        }

                        break;
                    }
                    case nameof(ExecuteScalar):
                    {
                        using (dbCommand)
                        {
                            result = await dbCommand.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
                        }

                        break;
                    }
                    case nameof(ExecuteReader):
                    {
                        try
                        {
                            // end copied from base method
                            if (locked)
                            {
                                // if calling 'ExecuteReader', transfer ownership of the Semaphore to it until it is disposed
                                result = new RelationalDataReader(
                                    openConnection ? connection : null,
                                    dbCommand,
                                    new SynchronizedMySqlDataReader(
                                        await dbCommand.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false) as MySqlDataReader,
                                        mySqlConnection.Lock));
                            }
                            // copied from base method
                            else
                            {
                                result = new RelationalDataReader(
                                    openConnection ? connection : null,
                                    dbCommand,
                                    await dbCommand.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false));
                            }
                        }
                        catch (Exception)
                        {
                            dbCommand.Dispose();
                            throw;
                        }
                        break;
                    }
                    default:
                    {
                        throw new NotSupportedException();
                    }
                }
            }
            catch (Exception)
            {
                if (openConnection && !closeConnection)
                {
                    connection.Close();
                }
                throw;
            }
            finally
            {
                if (closeConnection)
                {
                    connection.Close();
                }
                // end copied from base method
                if (locked && executeMethod != nameof(ExecuteReader))
                {
                    // if calling any other method, the command has finished executing and the lock can be released immediately
                    mySqlConnection.Lock.Release();
                }
            }
            return result;
        }

        private DbCommand CreateCommand(
            IRelationalConnection connection,
            IReadOnlyDictionary<string, object> parameterValues)
        {
            var command = connection.DbConnection.CreateCommand();

            command.CommandText = CommandText;

            if (connection.CurrentTransaction != null)
            {
                command.Transaction = connection.CurrentTransaction.GetDbTransaction();
            }

            if (connection.CommandTimeout != null)
            {
                command.CommandTimeout = (int) connection.CommandTimeout;
            }

            if (Parameters.Count > 0)
            {
                if (parameterValues == null)
                {
                    throw new InvalidOperationException(
                        RelationalStrings.MissingParameterValue(
                            Parameters[0].InvariantName));
                }

                foreach (var parameter in Parameters)
                {
                    object parameterValue;

                    if (parameterValues.TryGetValue(parameter.InvariantName, out parameterValue))
                    {
                        if (parameterValue.GetType().FullName.StartsWith("System.JsonObject")){
                            parameter.AddDbParameter(command, parameterValue.ToString());
                        } else {
                            parameter.AddDbParameter(command, parameterValue);
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException(
                            RelationalStrings.MissingParameterValue(parameter.InvariantName));
                    }
                }
            }

            return command;
        }

    }
}
