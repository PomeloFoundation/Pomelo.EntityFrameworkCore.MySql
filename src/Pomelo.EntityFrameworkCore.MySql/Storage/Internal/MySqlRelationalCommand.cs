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
		    return ExecuteAsync(IOBehavior.Synchronous, connection, executeMethod, parameterValues, openConnection, closeConnection)
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
		    return await ExecuteAsync(IOBehavior.Asynchronous, connection, executeMethod, parameterValues, openConnection, closeConnection, cancellationToken).ConfigureAwait(false);
	    }

	    private async Task<object> ExecuteAsync(
		    IOBehavior ioBehavior,
		    [NotNull] IRelationalConnection connection,
		    [NotNull] string executeMethod,
		    [CanBeNull] IReadOnlyDictionary<string, object> parameterValues,
		    bool openConnection,
		    bool closeConnection,
		    CancellationToken cancellationToken = default(CancellationToken))
	    {
            Check.NotNull(connection, nameof(connection));
            Check.NotEmpty(executeMethod, nameof(executeMethod));
            var dbCommand = CreateCommand(connection, parameterValues);
            object result;
            if (openConnection)
            {
	            if (ioBehavior == IOBehavior.Asynchronous)
		            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
	            else
		            connection.Open();
            }

            cancellationToken.ThrowIfCancellationRequested();
            var mySqlConnection = connection as MySqlRelationalConnection;
            var locked = false;

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
                        using (dbCommand)
                        {
	                        if (ioBehavior == IOBehavior.Asynchronous)
		                        result = await dbCommand.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
	                        else
		                        result = dbCommand.ExecuteNonQuery();
                        }
                        break;
                    }
                    case nameof(ExecuteScalar):
                    {
                        using (dbCommand)
                        {
	                        if (ioBehavior == IOBehavior.Asynchronous)
		                        result = await dbCommand.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
	                        else
		                        result = dbCommand.ExecuteScalar();
                        }
                        break;
                    }
                    case nameof(ExecuteReader):
                    {
                        try
                        {
	                        MySqlDataReader dataReader;
	                        if (ioBehavior == IOBehavior.Asynchronous)
		                        dataReader = await dbCommand.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false) as MySqlDataReader;
	                        else
		                        dataReader = dbCommand.ExecuteReader() as MySqlDataReader;

	                        result = new RelationalDataReader(openConnection ? connection : null, dbCommand,
                                new SynchronizedMySqlDataReader(dataReader, mySqlConnection));
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
                if (locked && executeMethod != nameof(ExecuteReader))
                {
                    // if calling any other method, the command has finished executing and the lock can be released immediately
	                // ReSharper disable once PossibleNullReferenceException
	                mySqlConnection.Lock.Release();
	                mySqlConnection.PoolingClose();
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
