using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities.DebugServices;

public class DebugRelationalDataReader : RelationalDataReader
{
    private DebugDbDataReader _dataReaderWrapper;

    public override void Initialize(
        IRelationalConnection relationalConnection,
        DbCommand command,
        DbDataReader reader,
        Guid commandId,
        IRelationalCommandDiagnosticsLogger logger)
    {
        _dataReaderWrapper?.Dispose();
        _dataReaderWrapper = new DebugDbDataReader(reader, commandId, logger!.Logger);

        base.Initialize(relationalConnection, command, _dataReaderWrapper, commandId, logger);
    }

    public override void Dispose()
    {
        _dataReaderWrapper.Dispose();

        base.Dispose();
    }

    public override async ValueTask DisposeAsync()
    {
        await _dataReaderWrapper.DisposeAsync();

        await base.DisposeAsync();
    }
}

public class DebugDbDataReader : DbDataReader, IDisposable
{
    private readonly DbDataReader _implementation;
    private readonly ILogger _logger;
    private readonly StringBuilder _stringBuilder;
    private bool _isClosed;

    public DebugDbDataReader(DbDataReader implementation, Guid commandId, ILogger logger)
    {
        _implementation = implementation;
        _logger = logger;
        _stringBuilder = new StringBuilder();

        _stringBuilder.AppendLine($"DebugRelationalDataReader for Command {commandId}:");
        _stringBuilder.AppendLine(string.Join("\t", Enumerable.Range(0, _implementation.FieldCount).Select(f => _implementation.GetName(f))));
    }

    public override bool Read()
    {
        var result = _implementation.Read();

        if (result)
        {
            _stringBuilder.AppendLine(string.Join("\t", Enumerable.Range(0, _implementation.FieldCount).Select(f => _implementation.GetValue(f)?.ToString())));
        }

        return result;
    }

    public override async Task<bool> ReadAsync(CancellationToken cancellationToken)
    {
        var result = await _implementation.ReadAsync(cancellationToken);

        if (result)
        {
            _stringBuilder.AppendLine(string.Join("\t", Enumerable.Range(0, _implementation.FieldCount).Select(f => _implementation.GetValue(f)?.ToString())));
        }

        return result;
    }

    public override void Close()
    {
        if (!_isClosed)
        {
            _logger.LogInformation(_stringBuilder.ToString());
            _isClosed = true;
        }

        _implementation.Close();
    }

    public override Task CloseAsync()
    {
        if (!_isClosed)
        {
            _logger.LogInformation(_stringBuilder.ToString());
            _isClosed = true;
        }

        return _implementation.CloseAsync();
    }

    #region Delegated

    void IDisposable.Dispose()
        => _implementation.Dispose();

    public override ValueTask DisposeAsync()
        => _implementation.DisposeAsync();

    public override Task<T> GetFieldValueAsync<T>(int ordinal, CancellationToken cancellationToken)
        => _implementation.GetFieldValueAsync<T>(ordinal, cancellationToken);

    public override T GetFieldValue<T>(int ordinal)
        => _implementation.GetFieldValue<T>(ordinal);

    public override Type GetProviderSpecificFieldType(int ordinal)
        => _implementation.GetProviderSpecificFieldType(ordinal);

    public override object GetProviderSpecificValue(int ordinal)
        => _implementation.GetProviderSpecificValue(ordinal);

    public override int GetProviderSpecificValues(object[] values)
        => _implementation.GetProviderSpecificValues(values);

    public override DataTable GetSchemaTable()
        => _implementation.GetSchemaTable();

    public override Task<DataTable> GetSchemaTableAsync(CancellationToken cancellationToken = new CancellationToken())
        => _implementation.GetSchemaTableAsync(cancellationToken);

    public override Task<ReadOnlyCollection<DbColumn>> GetColumnSchemaAsync(CancellationToken cancellationToken = new CancellationToken())
        => _implementation.GetColumnSchemaAsync(cancellationToken);

    public override Stream GetStream(int ordinal)
        => _implementation.GetStream(ordinal);

    public override TextReader GetTextReader(int ordinal)
        => _implementation.GetTextReader(ordinal);

    public override Task<bool> IsDBNullAsync(int ordinal, CancellationToken cancellationToken)
        => _implementation.IsDBNullAsync(ordinal, cancellationToken);

    public override Task<bool> NextResultAsync(CancellationToken cancellationToken)
        => _implementation.NextResultAsync(cancellationToken);

    public override int VisibleFieldCount
        => _implementation.VisibleFieldCount;

    public override bool Equals(object obj)
        => _implementation.Equals(obj);

    public override int GetHashCode()
        => _implementation.GetHashCode();

    public override string ToString()
        => _implementation.ToString();

    public override bool GetBoolean(int ordinal)
        => _implementation.GetBoolean(ordinal);

    public override byte GetByte(int ordinal)
        => _implementation.GetByte(ordinal);

    public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
        => _implementation.GetBytes(ordinal, dataOffset, buffer, bufferOffset, length);

    public override char GetChar(int ordinal)
        => _implementation.GetChar(ordinal);

    public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
        => _implementation.GetChars(ordinal, dataOffset, buffer, bufferOffset, length);

    public override string GetDataTypeName(int ordinal)
        => _implementation.GetDataTypeName(ordinal);

    public override DateTime GetDateTime(int ordinal)
        => _implementation.GetDateTime(ordinal);

    public override decimal GetDecimal(int ordinal)
        => _implementation.GetDecimal(ordinal);

    public override double GetDouble(int ordinal)
        => _implementation.GetDouble(ordinal);

    public override Type GetFieldType(int ordinal)
        => _implementation.GetFieldType(ordinal);

    public override float GetFloat(int ordinal)
        => _implementation.GetFloat(ordinal);

    public override Guid GetGuid(int ordinal)
        => _implementation.GetGuid(ordinal);

    public override short GetInt16(int ordinal)
        => _implementation.GetInt16(ordinal);

    public override int GetInt32(int ordinal)
        => _implementation.GetInt32(ordinal);

    public override long GetInt64(int ordinal)
        => _implementation.GetInt64(ordinal);

    public override string GetName(int ordinal)
        => _implementation.GetName(ordinal);

    public override int GetOrdinal(string name)
        => _implementation.GetOrdinal(name);

    public override string GetString(int ordinal)
        => _implementation.GetString(ordinal);

    public override object GetValue(int ordinal)
        => _implementation.GetValue(ordinal);

    public override int GetValues(object[] values)
        => _implementation.GetValues(values);

    public override bool IsDBNull(int ordinal)
        => _implementation.IsDBNull(ordinal);

    public override int FieldCount
        => _implementation.FieldCount;

    public override object this[int ordinal]
        => _implementation[ordinal];

    public override object this[string name]
        => _implementation[name];

    public override int RecordsAffected
        => _implementation.RecordsAffected;

    public override bool HasRows
        => _implementation.HasRows;

    public override bool IsClosed
        => _implementation.IsClosed;

    public override bool NextResult()
        => _implementation.NextResult();

    public override int Depth
        => _implementation.Depth;

    public override IEnumerator GetEnumerator()
        => _implementation.GetEnumerator();

    #endregion Delegated
}
