using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Utilities;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Storage.Internal
{
    public class MySqlRelationalCommand : RelationalCommand
    {
		public MySqlRelationalCommand(
            [NotNull] IDiagnosticsLogger<DbLoggerCategory.Database.Command> logger,
            [NotNull] string commandText,
            [NotNull] IReadOnlyList<IRelationalParameter> parameters)
			: base(logger, commandText, parameters)
        {
        }

	    protected override object Execute(
            [NotNull] IRelationalConnection connection,
            DbCommandMethod executeMethod,
            [CanBeNull] IReadOnlyDictionary<string, object> parameterValues)
        {
		    return ExecuteAsync(IOBehavior.Synchronous, connection, executeMethod, parameterValues)
			    .GetAwaiter()
			    .GetResult();
	    }

	    protected override async Task<object> ExecuteAsync(
            [NotNull] IRelationalConnection connection,
            DbCommandMethod executeMethod,
            [CanBeNull] IReadOnlyDictionary<string, object> parameterValues,
            CancellationToken cancellationToken = default(CancellationToken))
        {
		    return await ExecuteAsync(IOBehavior.Asynchronous, connection, executeMethod, parameterValues, cancellationToken).ConfigureAwait(false);
	    }

	    private async Task<object> ExecuteAsync(
		    IOBehavior ioBehavior,
		    [NotNull] IRelationalConnection connection,
            DbCommandMethod executeMethod,
            [CanBeNull] IReadOnlyDictionary<string, object> parameterValues,
            CancellationToken cancellationToken = default(CancellationToken))
	    {
            Check.NotNull(connection, nameof(connection));

            var dbCommand = CreateCommand(connection, parameterValues);
            var mySqlConnection = connection as MySqlRelationalConnection;

            if (ioBehavior == IOBehavior.Asynchronous)
                // ReSharper disable once PossibleNullReferenceException
                await mySqlConnection.OpenAsync(cancellationToken, false).ConfigureAwait(false);
            else
                // ReSharper disable once PossibleNullReferenceException
                mySqlConnection.Open();

            var commandId = Guid.NewGuid();

            var startTime = DateTimeOffset.UtcNow;
            var stopwatch = Stopwatch.StartNew();

            Logger.CommandExecuting(
                dbCommand,
                executeMethod,
                commandId,
                connection.ConnectionId,
                async: ioBehavior == IOBehavior.Asynchronous,
                startTime: startTime);

            object result;
            var readerOpen = false;
            try
            {
                switch (executeMethod)
                {
                    case DbCommandMethod.ExecuteNonQuery:
                    {
                        result = ioBehavior == IOBehavior.Asynchronous ?
                            await dbCommand.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false) :
                            dbCommand.ExecuteNonQuery();
                        break;
                    }
                    case DbCommandMethod.ExecuteScalar:
                    {
                        result = ioBehavior == IOBehavior.Asynchronous ?
                            await dbCommand.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false) :
                            dbCommand.ExecuteScalar();
                        break;
                    }
                    case DbCommandMethod.ExecuteReader:
                    {
                        var dataReader = ioBehavior == IOBehavior.Asynchronous ?
                            await dbCommand.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false) :
                            dbCommand.ExecuteReader();
                        result = new RelationalDataReader(connection, dbCommand, new WrappedMySqlDataReader(dataReader), commandId, Logger);
                        readerOpen = true;
                        break;
                    }
                    default:
                    {
                        throw new NotSupportedException();
                    }
                }

                Logger.CommandExecuted(
                    dbCommand,
                    executeMethod,
                    commandId,
                    connection.ConnectionId,
                    result,
                    async: ioBehavior == IOBehavior.Asynchronous,
                    startTime: startTime,
                    duration: stopwatch.Elapsed);
            }
            catch (Exception exception)
            {
                Logger.CommandError(
                    dbCommand,
                    executeMethod,
                    commandId,
                    connection.ConnectionId,
                    exception,
                    async: ioBehavior == IOBehavior.Asynchronous,
                    startTime: startTime,
                    duration: stopwatch.Elapsed);

                throw;
            }
            finally
            {
                dbCommand.Parameters.Clear();

                if (!readerOpen)
                {
                    dbCommand.Dispose();
                    connection.Close();
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
                    throw new InvalidOperationException(RelationalStrings.MissingParameterValue(Parameters[0].InvariantName));
                }

                foreach (var parameter in Parameters)
                {
                    object parameterValue;

                    if (parameterValues.TryGetValue(parameter.InvariantName, out parameterValue))
                    {
	                    if (parameterValue != null)
	                    {
		                    if (parameterValue is char)
			                    parameter.AddDbParameter(command, Convert.ToByte((char)parameterValue));
		                    else if (parameterValue.GetType().FullName.StartsWith("System.JsonObject"))
			                    parameter.AddDbParameter(command, parameterValue.ToString());
							else if (parameterValue.GetType().GetTypeInfo().IsEnum)
                                parameter.AddDbParameter(command, Convert.ChangeType(parameterValue, Enum.GetUnderlyingType(parameterValue.GetType())));
		                    else
			                    parameter.AddDbParameter(command, parameterValue);
	                    }
	                    else
	                    {
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
