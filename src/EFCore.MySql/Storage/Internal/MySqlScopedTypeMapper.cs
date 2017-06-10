// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Data;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Utilities;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Storage.Internal
{
    public class MySqlScopedTypeMapper : IRelationalTypeMapper
    {
        private static readonly DateTimeTypeMapping DateTime             = new DateTimeTypeMapping("datetime", DbType.DateTime);
        private static readonly DateTimeOffsetTypeMapping DateTimeOffset = new DateTimeOffsetTypeMapping("datetime", DbType.DateTime);
        private static readonly TimeSpanTypeMapping Time                 = new TimeSpanTypeMapping("time", DbType.Time);
        private static readonly GuidTypeMapping OldGuid                  = new GuidTypeMapping("binary(16)", DbType.Guid);

        private MySqlConnectionSettings _connectionSettings;
        private readonly IDbContextOptions _options;
        private readonly MySqlTypeMapper _typeMapper;

        public MySqlScopedTypeMapper(
            [NotNull] MySqlTypeMapper typeMapper,
            [CanBeNull] IDbContextOptions options)
        {
            Check.NotNull(typeMapper, nameof(typeMapper));

            _typeMapper = typeMapper;
            _options = options;
        }

        public virtual RelationalTypeMapping FindMapping(IProperty property)
        {
            var mapping = _typeMapper.FindMapping(property);
            return mapping == null ? null : MaybeConvertMapping(mapping);
        }

        public virtual RelationalTypeMapping FindMapping(Type clrType)
        {
            var mapping = _typeMapper.FindMapping(clrType);
            return mapping == null ? null : MaybeConvertMapping(mapping);
        }

        public virtual RelationalTypeMapping FindMapping(string storeType)
        {
            var mapping = _typeMapper.FindMapping(storeType);
            return mapping == null ? null : MaybeConvertMapping(mapping);
        }

        public string ConnectionString { get; set; }

        public MySqlConnectionSettings ConnectionSettings
        {
            get
            {
                if (_connectionSettings != null)
                    return _connectionSettings;

                if (ConnectionString != null)
                    return _connectionSettings = MySqlConnectionSettings.GetSettings(ConnectionString);

                var relationalOptions = RelationalOptionsExtension.Extract(_options);
                if (relationalOptions.ConnectionString != null)
                    return _connectionSettings = MySqlConnectionSettings.GetSettings(relationalOptions.ConnectionString);
                if (relationalOptions.Connection != null)
                    return _connectionSettings = MySqlConnectionSettings.GetSettings(relationalOptions.Connection);

                throw new InvalidOperationException(RelationalStrings.NoConnectionOrConnectionString);
            }
        }

        protected virtual RelationalTypeMapping MaybeConvertMapping(RelationalTypeMapping mapping)
        {
            // OldGuids
            if (ConnectionSettings.OldGuids)
            {
                if (mapping.StoreType == "binary(16)" && mapping.ClrType == typeof(byte[]))
                    return OldGuid;
                if (mapping.StoreType == "char(36)" && mapping.ClrType == typeof(Guid))
                    return OldGuid;
            }

            // SupportsDateTime6
            if (!ConnectionSettings.ServerVersion.SupportsDateTime6)
            {
                if (mapping.StoreType == "datetime(6)" && mapping.ClrType == typeof(DateTime))
                    return DateTime;
                if (mapping.StoreType == "datetime(6)" && mapping.ClrType == typeof(DateTimeOffset))
                    return DateTimeOffset;
                if (mapping.StoreType == "time(6)")
                    return Time;
            }

            return mapping;
        }

        public virtual void ValidateTypeName(string storeType) => _typeMapper.ValidateTypeName(storeType);

        public virtual IByteArrayRelationalTypeMapper ByteArrayMapper => _typeMapper.ByteArrayMapper;
        public virtual IStringRelationalTypeMapper StringMapper => _typeMapper.StringMapper;
        public virtual bool IsTypeMapped(Type clrType) => _typeMapper.IsTypeMapped(clrType);
    }
}
