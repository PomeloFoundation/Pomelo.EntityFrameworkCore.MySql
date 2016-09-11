using System;
using System.Collections;
using System.Data.Common;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Storage.Internal
{
    /// <summary>
    /// <see cref="SynchronizedMySqlDataReader"/> wraps <see cref="MySqlDataReader" /> and holds a semaphore
    /// until it is disposed, at which point it is released. This prevents <see cref="MySqlRelationalCommand.ExecuteAsync"/>
    /// from being entered while there is an active data reader.
    /// </summary>
    public sealed class SynchronizedMySqlDataReader : DbDataReader
    {
        private SemaphoreSlim _lock;
        private MySqlDataReader _reader;

        internal SynchronizedMySqlDataReader(MySqlDataReader reader, SemaphoreSlim @lock)
        {
            _reader = reader;
            _lock = @lock;
        }

        public override bool GetBoolean(int ordinal) => GetReader().GetBoolean(ordinal);
        public override byte GetByte(int ordinal) => GetReader().GetByte(ordinal);
        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length) => GetReader().GetBytes(ordinal, dataOffset, buffer, bufferOffset, length);
        public override char GetChar(int ordinal) => GetReader().GetChar(ordinal);
        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length) => GetReader().GetChars(ordinal, dataOffset, buffer, bufferOffset, length);
        public override string GetDataTypeName(int ordinal) => GetReader().GetDataTypeName(ordinal);
        public override DateTime GetDateTime(int ordinal) => GetReader().GetDateTime(ordinal);
        public override decimal GetDecimal(int ordinal) => GetReader().GetDecimal(ordinal);
        public override double GetDouble(int ordinal) => GetReader().GetDouble(ordinal);
        public override Type GetFieldType(int ordinal) => GetReader().GetFieldType(ordinal);
        public override float GetFloat(int ordinal) => GetReader().GetFloat(ordinal);
        public override Guid GetGuid(int ordinal) => GetReader().GetGuid(ordinal);
        public override short GetInt16(int ordinal) => GetReader().GetInt16(ordinal);
        public override int GetInt32(int ordinal) => GetReader().GetInt32(ordinal);
        public override long GetInt64(int ordinal) => GetReader().GetInt64(ordinal);
        public override string GetName(int ordinal) => GetReader().GetName(ordinal);
        public override int GetOrdinal(string name) => GetReader().GetOrdinal(name);
        public override string GetString(int ordinal) => GetReader().GetString(ordinal);
        public override object GetValue(int ordinal) => GetReader().GetValue(ordinal);
        public override int GetValues(object[] values) => GetReader().GetValues(values);
        public override bool IsDBNull(int ordinal) => GetReader().IsDBNull(ordinal);
        public override int FieldCount => GetReader().FieldCount;
        public override object this[int ordinal] => GetReader()[ordinal];
        public override object this[string name] => GetReader()[name];
        public override int RecordsAffected => GetReader().RecordsAffected;
        public override bool HasRows => GetReader().HasRows;
        public override bool IsClosed => _reader == null || _reader.IsClosed;
        public override bool NextResult() => GetReader().NextResult();
        public override bool Read() => GetReader().Read();
        public override int Depth => GetReader().Depth;
        public override IEnumerator GetEnumerator() => GetReader().GetEnumerator();
        public override T GetFieldValue<T>(int ordinal) => GetReader().GetFieldValue<T>(ordinal);
        public override Task<T> GetFieldValueAsync<T>(int ordinal, CancellationToken cancellationToken) => GetReader().GetFieldValueAsync<T>(ordinal, cancellationToken);
        public override Type GetProviderSpecificFieldType(int ordinal) => GetReader().GetProviderSpecificFieldType(ordinal);
        public override object GetProviderSpecificValue(int ordinal) => GetReader().GetProviderSpecificValue(ordinal);
        public override int GetProviderSpecificValues(object[] values) => GetReader().GetProviderSpecificValues(values);
        public override Stream GetStream(int ordinal) => GetReader().GetStream(ordinal);
        public override TextReader GetTextReader(int ordinal) => GetReader().GetTextReader(ordinal);
        public override Task<bool> IsDBNullAsync(int ordinal, CancellationToken cancellationToken) => GetReader().IsDBNullAsync(ordinal, cancellationToken);
        public override Task<bool> NextResultAsync(CancellationToken cancellationToken) => GetReader().NextResultAsync(cancellationToken);
        public override Task<bool> ReadAsync(CancellationToken cancellationToken) => GetReader().ReadAsync(cancellationToken);
        public override int VisibleFieldCount => GetReader().VisibleFieldCount;

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (_reader != null)
                {
                    // dispose the underlying MySQL data reader
                    _reader.Dispose();
                    _reader = null;
                }
            }
            finally
            {
                if (_lock != null)
                {
                    // release the shared lock, so another statement can be executed
                    _lock.Release();
                    _lock = null;
                }

                base.Dispose(disposing);
            }
        }

        private DbDataReader GetReader()
        {
            if (_reader == null)
                throw new ObjectDisposedException(nameof(SynchronizedMySqlDataReader));
            return _reader;
        }
    }
}
