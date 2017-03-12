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
using System.Reflection;

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
		    IRelationalConnection connection,
		    string executeMethod,
		    IReadOnlyDictionary<string, object> parameterValues,
		    bool closeConnection = true)
	    {
		    return ExecuteAsync(IOBehavior.Synchronous, connection, executeMethod, parameterValues, closeConnection)
			    .GetAwaiter()
			    .GetResult();
	    }

	    protected override async Task<object> ExecuteAsync(
		    IRelationalConnection connection,
		    string executeMethod,
		    IReadOnlyDictionary<string, object> parameterValues,
		    bool closeConnection = true,
		    CancellationToken cancellationToken = default(CancellationToken))
	    {
		    return await ExecuteAsync(IOBehavior.Asynchronous, connection, executeMethod, parameterValues, closeConnection, cancellationToken).ConfigureAwait(false);
	    }

	    private async Task<object> ExecuteAsync(
		    IOBehavior ioBehavior,
		    [NotNull] IRelationalConnection connection,
		    [NotNull] string executeMethod,
		    [CanBeNull] IReadOnlyDictionary<string, object> parameterValues,
		    bool closeConnection,
		    CancellationToken cancellationToken = default(CancellationToken))
	    {
            Check.NotNull(connection, nameof(connection));
            Check.NotEmpty(executeMethod, nameof(executeMethod));
            var dbCommand = CreateCommand(connection, parameterValues);
            object result;

            cancellationToken.ThrowIfCancellationRequested();
            var mySqlConnection = connection as MySqlRelationalConnection;
            var locked = false;
		    var isReader = false;
		    var startTimestamp = Stopwatch.GetTimestamp();

		    try
            {
	            if (ioBehavior == IOBehavior.Asynchronous)
	            {
		            // ReSharper disable once PossibleNullReferenceException
		            await mySqlConnection.PoolingOpenAsync(cancellationToken).ConfigureAwait(false);
		            await mySqlConnection.Lock.WaitAsync(cancellationToken).ConfigureAwait(false);
	            }
	            else
	            {
		            // ReSharper disable once PossibleNullReferenceException
		            mySqlConnection.PoolingOpen();
		            mySqlConnection.Lock.Wait(cancellationToken);
	            }
	            locked = true;
                switch (executeMethod)
                {
                    case nameof(ExecuteNonQuery):
                    {
						if (ioBehavior == IOBehavior.Asynchronous)
							result = await dbCommand.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
						else
							result = dbCommand.ExecuteNonQuery();
	                    break;
                    }
                    case nameof(ExecuteScalar):
                    {
						if (ioBehavior == IOBehavior.Asynchronous)
							result = await dbCommand.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
						else
							result = dbCommand.ExecuteScalar();
	                    break;
                    }
                    case nameof(ExecuteReader):
                    {
						MySqlDataReader dataReader;
						if (ioBehavior == IOBehavior.Asynchronous)
							dataReader = await dbCommand.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false) as MySqlDataReader;
						else
							dataReader = dbCommand.ExecuteReader() as MySqlDataReader;
						result = new RelationalDataReader(connection, dbCommand, new SynchronizedMySqlDataReader(dataReader, mySqlConnection));
	                    isReader = true;
	                    break;
                    }
                    default:
                    {
                        throw new NotSupportedException();
                    }
                }
            }
            finally
            {
	            var currentTimestamp = Stopwatch.GetTimestamp();
	            Logger.LogCommandExecuted(dbCommand, startTimestamp, currentTimestamp);
	            if (!isReader)
	            {
					// NonQuery, Scalar, and Exceptions can be disposed, and should release locks
		            dbCommand.Dispose();
		            if (locked)
		            {
			            mySqlConnection.Lock.Release();
			            mySqlConnection.PoolingClose();
		            }
	            }
	            // ReSharper disable once PossibleNullReferenceException
	            if (!mySqlConnection.Pooling && closeConnection)
	            {
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
		                    else if (parameterValue is DateTimeOffset)
			                    parameter.AddDbParameter(command, ((DateTimeOffset) parameterValue).UtcDateTime);
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
