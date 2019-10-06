using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Storage.Internal
{
    // ReSharper disable once ClassNeverInstantiated.Local
    public class MySqlConverterCommandBuilderFactory : RelationalCommandBuilderFactory
    {
        public MySqlConverterCommandBuilderFactory(
            IDiagnosticsLogger<DbLoggerCategory.Database.Command> logger,
            IRelationalTypeMappingSource typeMappingSource)
            : base(logger, typeMappingSource)
        {
        }

        protected override IRelationalCommandBuilder CreateCore(
            IDiagnosticsLogger<DbLoggerCategory.Database.Command> logger,
            IRelationalTypeMappingSource relationalTypeMappingSource)
            => new MySqlConverterRelationalCommandBuilder(
                logger, relationalTypeMappingSource);

        private class MySqlConverterRelationalCommandBuilder : RelationalCommandBuilder
        {
            public MySqlConverterRelationalCommandBuilder(
                IDiagnosticsLogger<DbLoggerCategory.Database.Command> logger,
                IRelationalTypeMappingSource typeMappingSource)
                : base(logger, typeMappingSource)
            {
            }

            protected override IRelationalCommand BuildCore(
                IDiagnosticsLogger<DbLoggerCategory.Database.Command> logger,
                string commandText,
                IReadOnlyList<IRelationalParameter> parameters)
                => new MySqlConverterRelationalCommand(logger, commandText, parameters);

            private class MySqlConverterRelationalCommand : RelationalCommand
            {
                public MySqlConverterRelationalCommand(
                    IDiagnosticsLogger<DbLoggerCategory.Database.Command> logger,
                    string commandText,
                    IReadOnlyList<IRelationalParameter> parameters)
                    : base(logger, commandText, parameters)
                {
                }

                // TODO: Copy and paste base code again in 3.0.
                //       Then replace "RelationalDataReader" with "MySqlConverterRelationalDataReader".
                // TODO: Remove entire method in 3.1.
                //       Replace with overridden implementation of "CreateRelationalDataReader".
                /// <summary>
                /// Uses the same code as in it's base class, except for returning a
                /// ConverterRelationalDataReader instead of a RelationalDataReader.
                /// </summary>
                protected override object Execute(
                    IRelationalConnection connection,
                    DbCommandMethod executeMethod,
                    IReadOnlyDictionary<string, object> parameterValues)
                {
                    if (connection == null)
                    {
                        throw new ArgumentNullException(nameof(connection));
                    }

                    var dbCommand = CreateCommand(connection, parameterValues);

                    connection.Open();

                    var commandId = Guid.NewGuid();

                    var startTime = DateTimeOffset.UtcNow;
                    var stopwatch = Stopwatch.StartNew();

                    Logger.CommandExecuting(
                        dbCommand,
                        executeMethod,
                        commandId,
                        connection.ConnectionId,
                        async: false,
                        startTime: startTime);

                    object result;
                    var readerOpen = false;
                    try
                    {
                        switch (executeMethod)
                        {
                            case DbCommandMethod.ExecuteNonQuery:
                                {
                                    result = dbCommand.ExecuteNonQuery();

                                    break;
                                }
                            case DbCommandMethod.ExecuteScalar:
                                {
                                    result = dbCommand.ExecuteScalar();

                                    break;
                                }
                            case DbCommandMethod.ExecuteReader:
                                {
                                    result
                                        = new MySqlConverterRelationalDataReader(
                                            connection,
                                            dbCommand,
                                            dbCommand.ExecuteReader(),
                                            commandId,
                                            Logger);
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
                            false,
                            startTime,
                            stopwatch.Elapsed);
                    }
                    catch (Exception exception)
                    {
                        Logger.CommandError(
                            dbCommand,
                            executeMethod,
                            commandId,
                            connection.ConnectionId,
                            exception,
                            false,
                            startTime,
                            stopwatch.Elapsed);

                        throw;
                    }
                    finally
                    {
                        if (!readerOpen)
                        {
                            dbCommand.Parameters.Clear();
                            dbCommand.Dispose();
                            connection.Close();
                        }
                    }

                    return result;
                }

                // TODO: Copy and paste base code again in 3.0.
                //       Then replace "RelationalDataReader" with "MySqlConverterRelationalDataReader".
                // TODO: Remove entire method in 3.1.
                //       Replace with overridden implementation of "CreateRelationalDataReader".
                /// <summary>
                /// Uses the same code as in it's base class, except for returning a
                /// ConverterRelationalDataReader instead of a RelationalDataReader.
                /// </summary>
                protected override async Task<object> ExecuteAsync(
                    IRelationalConnection connection,
                    DbCommandMethod executeMethod,
                    IReadOnlyDictionary<string, object> parameterValues,
                    CancellationToken cancellationToken = default)
                {
                    if (connection == null)
                    {
                        throw new ArgumentNullException(nameof(connection));
                    }

                    var dbCommand = CreateCommand(connection, parameterValues);

                    await connection.OpenAsync(cancellationToken);

                    var commandId = Guid.NewGuid();

                    var startTime = DateTimeOffset.UtcNow;
                    var stopwatch = Stopwatch.StartNew();

                    Logger.CommandExecuting(
                        dbCommand,
                        executeMethod,
                        commandId,
                        connection.ConnectionId,
                        async: true,
                        startTime: startTime);

                    object result;
                    var readerOpen = false;
                    try
                    {
                        switch (executeMethod)
                        {
                            case DbCommandMethod.ExecuteNonQuery:
                                {
                                    result = await dbCommand.ExecuteNonQueryAsync(cancellationToken);

                                    break;
                                }
                            case DbCommandMethod.ExecuteScalar:
                                {
                                    result = await dbCommand.ExecuteScalarAsync(cancellationToken);

                                    break;
                                }
                            case DbCommandMethod.ExecuteReader:
                                {
                                    result = new MySqlConverterRelationalDataReader(
                                        connection,
                                        dbCommand,
                                        await dbCommand.ExecuteReaderAsync(cancellationToken),
                                        commandId,
                                        Logger);
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
                            true,
                            startTime,
                            stopwatch.Elapsed);
                    }
                    catch (Exception exception)
                    {
                        Logger.CommandError(
                            dbCommand,
                            executeMethod,
                            commandId,
                            connection.ConnectionId,
                            exception,
                            true,
                            startTime,
                            stopwatch.Elapsed);

                        throw;
                    }
                    finally
                    {
                        if (!readerOpen)
                        {
                            dbCommand.Parameters.Clear();
                            dbCommand.Dispose();
                            connection.Close();
                        }
                    }

                    return result;
                }

                // TODO: The test suit needs to be extended, to fully ensure expected CAST() behavior.
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
