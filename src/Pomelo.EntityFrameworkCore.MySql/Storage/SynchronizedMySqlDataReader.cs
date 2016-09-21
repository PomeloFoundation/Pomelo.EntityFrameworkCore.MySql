using System;
using System.Collections;
using System.Data.Common;
using System.IO;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using MySql.Data.MySqlClient;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Storage
{
    /// <summary>
    /// <see cref="SynchronizedMySqlDataReader"/> wraps <see cref="MySqlDataReader" /> and holds a semaphore
    /// until it is disposed, at which point it is released. This prevents <see cref="MySqlRelationalCommand.ExecuteAsync"/>
    /// from being entered while there is an active data reader. It also enhances <see cref="GetFieldValue{T}"/> to
    /// use reflection to cast from <c>byte[]</c> if the regular method fails; this allows JSON objects to be
    /// deserialized.
    /// </summary>
    public class SynchronizedMySqlDataReader : DbDataReader
    {
        private readonly MySqlRelationalConnection _connection;
        private SemaphoreSlim _lock;
        private MySqlDataReader _reader;
        private bool _disposed;

        internal SynchronizedMySqlDataReader(MySqlDataReader reader, MySqlRelationalConnection connection)
        {
            _reader = reader;
            _connection = connection;
            _lock = _connection.Lock;
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
        public override int Depth => GetReader().Depth;
        public override IEnumerator GetEnumerator() => GetReader().GetEnumerator();
        public override Type GetProviderSpecificFieldType(int ordinal) => GetReader().GetProviderSpecificFieldType(ordinal);
        public override object GetProviderSpecificValue(int ordinal) => GetReader().GetProviderSpecificValue(ordinal);
        public override int GetProviderSpecificValues(object[] values) => GetReader().GetProviderSpecificValues(values);
        public override Stream GetStream(int ordinal) => GetReader().GetStream(ordinal);
        public override TextReader GetTextReader(int ordinal) => GetReader().GetTextReader(ordinal);
        public override Task<bool> IsDBNullAsync(int ordinal, CancellationToken cancellationToken) => GetReader().IsDBNullAsync(ordinal, cancellationToken);
        public override int VisibleFieldCount => GetReader().VisibleFieldCount;

#if NET45
		public override DataTable GetSchemaTable()
		{
			throw new NotSupportedException();
		}

		public override void Close()
		{
			CloseReader();
		}
#endif

        private bool? _nextResult;
        private bool PeekNextResult()
        {
            if (_nextResult == null)
            {
                _nextResult = _reader != null && GetReader().NextResult();
            }
            return _nextResult.Value;
        }

        private async Task<bool> PeekNextResultAsync(CancellationToken cancellationToken)
        {
            if (_nextResult == null)
            {
                _nextResult = _reader != null && await GetReader().NextResultAsync(cancellationToken).ConfigureAwait(false);
            }
            return _nextResult != null && _nextResult.Value;
        }

        public override bool NextResult()
        {
            var result = PeekNextResult();
	        if (!result)
	        {
		        CloseReader();
	        }
            _nextResult = null;
            return result;
        }

        public override async Task<bool> NextResultAsync(CancellationToken cancellationToken)
        {
            var result = await PeekNextResultAsync(cancellationToken).ConfigureAwait(false);
	        if (!result)
	        {
		        CloseReader();
	        }
	        _nextResult = null;
            return result;
        }

        public override bool Read()
        {
            var result = _reader != null && GetReader().Read();
            if (!result && _reader != null && !PeekNextResult())
            {
	            CloseReader();
            }
            return result;
        }

        public override async Task<bool> ReadAsync(CancellationToken cancellationToken)
        {
            var result = _reader != null && await GetReader().ReadAsync(cancellationToken).ConfigureAwait(false);
            if (!result && _reader != null && !await PeekNextResultAsync(cancellationToken).ConfigureAwait(false))
            {
                CloseReader();
            }
            return result;
        }

        public override T GetFieldValue<T>(int ordinal)
        {
            try
            {
                // try normal casting
                return GetReader().GetFieldValue<T>(ordinal);
            }
            catch (InvalidCastException e)
            {
                return ConvertWithReflection<T>(ordinal, e);
            }
        }

        public override async Task<T> GetFieldValueAsync<T>(int ordinal, CancellationToken cancellationToken)
        {
            try
            {
                // try normal casting
                return await GetReader().GetFieldValueAsync<T>(ordinal, cancellationToken).ConfigureAwait(false);
            }
            catch (InvalidCastException e)
            {
                return ConvertWithReflection<T>(ordinal, e);
            }
        }

        private T ConvertWithReflection<T>(int ordinal, InvalidCastException e)
        {
            try
            {
                // try casting using reflection; needed for json
                var dataParam = Expression.Parameter(typeof(string), "data");
                var body = Expression.Block(Expression.Convert(dataParam, typeof(T)));
                var run = Expression.Lambda(body, dataParam).Compile();
                return (T) run.DynamicInvoke(GetReader().GetValue(ordinal));
            }
            catch (Exception)
            {
                // throw original InvalidCastException
                throw e;
            }
        }

        private void CloseReader()
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
	                _connection.PoolingClose();
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                CloseReader();
                base.Dispose(disposing);
                _disposed = true;
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
