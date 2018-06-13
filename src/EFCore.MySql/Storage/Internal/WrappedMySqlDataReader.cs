// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Data.Common;
using System.IO;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Pomelo.EntityFrameworkCore.MySql.Storage.Internal
{
    /// <summary>
    /// <see cref="WrappedMySqlDataReader"/> wraps <see cref="DbDataReader" /> and enhances <see cref="GetFieldValue{T}"/> to
    /// use reflection to cast from <c>byte[]</c> if the regular method fails; this allows JSON objects to be deserialized.
    /// </summary>
    public class WrappedMySqlDataReader : DbDataReader
    {
        private DbDataReader _reader;
        private bool _disposed;
        private static readonly ConcurrentDictionary<string, Delegate> _compiledExpressionCache = new ConcurrentDictionary<string, Delegate>();

        internal WrappedMySqlDataReader(DbDataReader reader)
        {
            _reader = reader;
        }

        public override bool GetBoolean(int ordinal) => GetReader().GetBoolean(ordinal);
	    public override byte GetByte(int ordinal) => GetReader().GetByte(ordinal);
	    public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length) => GetReader().GetBytes(ordinal, dataOffset, buffer, bufferOffset, length);
	    public override char GetChar(int ordinal) => Convert.ToChar(GetReader().GetByte(ordinal));
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
        public override bool NextResult() => GetReader().NextResult();
        public override Task<bool> NextResultAsync(CancellationToken cancellationToken) => GetReader().NextResultAsync(cancellationToken);
        public override bool Read() => GetReader().Read();
        public override Task<bool> ReadAsync(CancellationToken cancellationToken) => GetReader().ReadAsync(cancellationToken);

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

        public override T GetFieldValue<T>(int ordinal)
        {
            try
            {
                // try normal casting
                if (typeof(T) == typeof(char))
                {
                    return (T)Convert.ChangeType(Convert.ToChar(GetReader().GetFieldValue<byte>(ordinal)), typeof(T));
                }
                // for all JsonObject types, use explicit conversion with reflection
                else if (IsJsonObjectType(typeof(T)))
                {
                    return ConvertWithReflection<T>(ordinal);
                }

                return GetReader().GetFieldValue<T>(ordinal);
            }
            catch (InvalidCastException e)
            {
                return ConvertWithReflection<T>(ordinal, e);
            }
        }

        private bool IsJsonObjectType(Type type)
        {
            return type == typeof(JsonObject) || type.IsGenericType && type.GetGenericTypeDefinition() == typeof(JsonObject<>);
        }

        private T ConvertWithReflection<T>(int ordinal)
        {
            var run = GetCompiledConversionExpression(typeof(T));
            return (T)run.DynamicInvoke(GetReader().GetValue(ordinal));
        }

        private T ConvertWithReflection<T>(int ordinal, InvalidCastException e)
        {
            try
            {
                // for unlisted types, try casting using reflection
                return ConvertWithReflection<T>(ordinal);
            }
            catch (Exception)
            {
                // throw original InvalidCastException
                throw e;
            }
        }

        private Delegate GetCompiledConversionExpression(Type reflectionType)
        {
            var typeFullName = reflectionType.FullName;
            return _compiledExpressionCache.GetOrAdd(typeFullName, _ =>
            {
                var dataParam = Expression.Parameter(typeof(string), "data");
                var body = Expression.Block(Expression.Convert(dataParam, reflectionType));
                return Expression.Lambda(body, dataParam).Compile();
            });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                if (!_disposed)
                {
                    // dispose the underlying MySQL data reader
                    _reader.Dispose();
                }
                base.Dispose(disposing);
                _disposed = true;
            }
        }

        private DbDataReader GetReader()
        {
            return _reader;
        }
    }
}
