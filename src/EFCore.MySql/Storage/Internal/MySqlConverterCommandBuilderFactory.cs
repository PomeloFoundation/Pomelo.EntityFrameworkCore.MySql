using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;

namespace Pomelo.EntityFrameworkCore.MySql.Storage.Internal
{
    //
    // TODO: Remove entire class hierarchy once MySqlConnector adds additinal type casting for us.
    //

    // ReSharper disable once ClassNeverInstantiated.Local
    public class MySqlConverterCommandBuilderFactory : RelationalCommandBuilderFactory
    {
        public MySqlConverterCommandBuilderFactory(
            [NotNull] RelationalCommandBuilderDependencies dependencies)
            : base(dependencies)
        {
        }

        public override IRelationalCommandBuilder Create()
            => new MySqlConverterRelationalCommandBuilder(Dependencies);

        private class MySqlConverterRelationalCommandBuilder : RelationalCommandBuilder
        {
            public MySqlConverterRelationalCommandBuilder(
                [NotNull] RelationalCommandBuilderDependencies dependencies)
                : base(dependencies)
            {
            }

            public override IRelationalCommand Build()
                => new MySqlConverterRelationalCommand(Dependencies, ToString(), Parameters);

            private class MySqlConverterRelationalCommand : RelationalCommand
            {
                public MySqlConverterRelationalCommand([NotNull] RelationalCommandBuilderDependencies dependencies, [NotNull] string commandText, [NotNull] IReadOnlyList<IRelationalParameter> parameters) : base(dependencies, commandText, parameters)
                {
                }

                // INFO: The entire method is copy paste inherited.
                //       "RelationalDataReader" got replaced with "MySqlConverterRelationalDataReader".
                //       The "CleanupCommand" call got copy paste inherited and inlined as well.
                /// <summary>
                /// Uses the same code as in it's base class, except for returning a
                /// ConverterRelationalDataReader instead of a RelationalDataReader.
                /// </summary>
                public override RelationalDataReader ExecuteReader(RelationalCommandParameterObject parameterObject)
                {
                    var (connection, context, logger) = (parameterObject.Connection, parameterObject.Context, parameterObject.Logger);

                    var commandId = Guid.NewGuid();
                    var command = CreateCommand(parameterObject, commandId, DbCommandMethod.ExecuteReader);

                    connection.Open();

                    var startTime = DateTimeOffset.UtcNow;
                    var stopwatch = Stopwatch.StartNew();

                    var readerOpen = false;
                    try
                    {
                        var interceptionResult = logger?.CommandReaderExecuting(
                                                     connection,
                                                     command,
                                                     context,
                                                     commandId,
                                                     connection.ConnectionId,
                                                     startTime)
                                                 ?? default;

                        var reader = interceptionResult.HasResult
                            ? interceptionResult.Result
                            : command.ExecuteReader();

                        if (logger != null)
                        {
                            reader = logger.CommandReaderExecuted(
                                connection,
                                command,
                                context,
                                commandId,
                                connection.ConnectionId,
                                reader,
                                startTime,
                                stopwatch.Elapsed);
                        }

                        var result = new MySqlConverterRelationalDataReader(
                            connection,
                            command,
                            reader,
                            commandId,
                            logger);

                        readerOpen = true;

                        return result;
                    }
                    catch (Exception exception)
                    {
                        logger?.CommandError(
                            connection,
                            command,
                            context,
                            DbCommandMethod.ExecuteReader,
                            commandId,
                            connection.ConnectionId,
                            exception,
                            startTime,
                            stopwatch.Elapsed);

                        throw;
                    }
                    finally
                    {
                        if (!readerOpen)
                        {
                            // CleanupCommand is private static, so another copy paste inheritance here.
                            command.Parameters.Clear();
                            command.Dispose();
                            connection.Close();
                        }
                    }
                }

                // INFO: The entire method is copy paste inherited.
                //       "RelationalDataReader" got replaced with "MySqlConverterRelationalDataReader".
                //       The "CleanupCommandAsync" call got copy paste inherited and inlined as well.
                // TODO: Remove entire method in 3.1.
                //       Replace with overridden implementation of "CreateRelationalDataReader".
                /// <summary>
                /// Uses the same code as in it's base class, except for returning a
                /// ConverterRelationalDataReader instead of a RelationalDataReader.
                /// </summary>
                public override async Task<RelationalDataReader> ExecuteReaderAsync(
                    RelationalCommandParameterObject parameterObject,
                    CancellationToken cancellationToken = default)
                {
                    var (connection, context, logger) = (parameterObject.Connection, parameterObject.Context, parameterObject.Logger);

                    var commandId = Guid.NewGuid();
                    var command = CreateCommand(parameterObject, commandId, DbCommandMethod.ExecuteReader);

                    await connection.OpenAsync(cancellationToken);

                    var startTime = DateTimeOffset.UtcNow;
                    var stopwatch = Stopwatch.StartNew();

                    var readerOpen = false;
                    try
                    {
                        var interceptionResult = logger == null
                            ? default
                            : await logger.CommandReaderExecutingAsync(
                                connection,
                                command,
                                context,
                                commandId,
                                connection.ConnectionId,
                                startTime,
                                cancellationToken);

                        var reader = interceptionResult.HasResult
                            ? interceptionResult.Result
                            : await command.ExecuteReaderAsync(cancellationToken);

                        if (logger != null)
                        {
                            reader = await logger.CommandReaderExecutedAsync(
                                connection,
                                command,
                                context,
                                commandId,
                                connection.ConnectionId,
                                reader,
                                startTime,
                                stopwatch.Elapsed,
                                cancellationToken);
                        }

                        var result = new MySqlConverterRelationalDataReader(
                            connection,
                            command,
                            reader,
                            commandId,
                            logger);

                        readerOpen = true;

                        return result;
                    }
                    catch (Exception exception)
                    {
                        if (logger != null)
                        {
                            await logger.CommandErrorAsync(
                                connection,
                                command,
                                context,
                                DbCommandMethod.ExecuteReader,
                                commandId,
                                connection.ConnectionId,
                                exception,
                                startTime,
                                stopwatch.Elapsed,
                                cancellationToken);
                        }

                        throw;
                    }
                    finally
                    {
                        if (!readerOpen)
                        {
                            // CleanupCommandAsync is private static, so another copy paste inheritance here.
                            command.Parameters.Clear();
                            await command.DisposeAsync();
                            await connection.CloseAsync();
                        }
                    }
                }

                /// <summary>
                /// This class can be used to inject behavior into the default DataReader.
                /// The current implementation might not cover all possible cases yet, but just resolves
                /// all open CAST() issues reported by tests.
                /// </summary>
                private class MySqlConverterRelationalDataReader : RelationalDataReader
                {
                    public MySqlConverterRelationalDataReader(IRelationalConnection connection,
                        DbCommand command,
                        DbDataReader dataReader,
                        Guid commandId,
                        IDiagnosticsLogger<DbLoggerCategory.Database.Command> logger)
                        : base(connection, command, new MySqlConverterDataReader(dataReader), commandId, logger)
                    {
                    }

                    private class MySqlConverterDataReader : DbDataReader
                    {
                        private readonly DbDataReader _dataReader;

                        public MySqlConverterDataReader(DbDataReader dataReader)
                        {
                            _dataReader = dataReader;
                        }

                        public override bool GetBoolean(int ordinal) => _dataReader.GetBoolean(ordinal);
                        public override byte GetByte(int ordinal)
                        {
                            switch (_dataReader.GetValue(ordinal))
                            {
                                case byte byteValue:
                                    return byteValue;
                                case long longValue:
                                    // CAST(n AS byte) returns a long.
                                    return Convert.ToByte(longValue);
                                default:
                                    throw new InvalidCastException();
                            }
                        }

                        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length) => _dataReader.GetBytes(ordinal, dataOffset, buffer, bufferOffset, length);
                        public override char GetChar(int ordinal) => _dataReader.GetChar(ordinal);
                        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length) => _dataReader.GetChars(ordinal, dataOffset, buffer, bufferOffset, length);
                        public override string GetDataTypeName(int ordinal) => _dataReader.GetDataTypeName(ordinal);
                        public override DateTime GetDateTime(int ordinal) => _dataReader.GetDateTime(ordinal);
                        public override decimal GetDecimal(int ordinal) => _dataReader.GetDecimal(ordinal);

                        public override double GetDouble(int ordinal)
                        {
                            switch (_dataReader.GetValue(ordinal))
                            {
                                case double doubleValue:
                                    return doubleValue;
                                case float floatValue:
                                    return (double)floatValue;
                                case decimal decimalValue:
                                    return (double)decimalValue;
                                default:
                                    throw new InvalidCastException();
                            }
                        }

                        public override Type GetFieldType(int ordinal) => _dataReader.GetFieldType(ordinal);

                        public override float GetFloat(int ordinal)
                        {
                            switch (_dataReader.GetValue(ordinal))
                            {
                                case float floatValue:
                                    return floatValue;
                                case double doubleValue:
                                    // CAST(n AS float) uses a workaround, to at least return a double.
                                    return (float)doubleValue;
                                default:
                                    throw new InvalidCastException();
                            }
                        }

                        public override Guid GetGuid(int ordinal) => _dataReader.GetGuid(ordinal);
                        public override short GetInt16(int ordinal) => _dataReader.GetInt16(ordinal);
                        public override int GetInt32(int ordinal) => _dataReader.GetInt32(ordinal);
                        public override long GetInt64(int ordinal) => _dataReader.GetInt64(ordinal);
                        public override string GetName(int ordinal) => _dataReader.GetName(ordinal);
                        public override int GetOrdinal(string name) => _dataReader.GetOrdinal(name);
                        public override string GetString(int ordinal) => _dataReader.GetString(ordinal);

                        public override object GetValue(int ordinal) => _dataReader.GetValue(ordinal);
                        public override int GetValues(object[] values) => _dataReader.GetValues(values);
                        public override T GetFieldValue<T>(int ordinal) => _dataReader.GetFieldValue<T>(ordinal);

                        public override bool IsDBNull(int ordinal) => _dataReader.IsDBNull(ordinal);
                        public override int FieldCount => _dataReader.FieldCount;
                        public override object this[int ordinal] => _dataReader[ordinal];
                        public override object this[string name] => _dataReader[name];
                        public override int RecordsAffected => _dataReader.RecordsAffected;
                        public override bool HasRows => _dataReader.HasRows;
                        public override bool IsClosed => _dataReader.IsClosed;
                        public override bool NextResult() => _dataReader.NextResult();
                        public override bool Read() => _dataReader.Read();
                        public override int Depth => _dataReader.Depth;
                        public override IEnumerator GetEnumerator() => _dataReader.GetEnumerator();

                        protected override void Dispose(bool disposing)
                        {
                            if (disposing)
                            {
                                _dataReader.Dispose();
                            }

                            base.Dispose(disposing);
                        }
                    }
                }
            }
        }
    }
}
