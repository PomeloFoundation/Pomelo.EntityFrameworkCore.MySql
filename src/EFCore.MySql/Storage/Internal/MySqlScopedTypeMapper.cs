// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Data;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Utilities;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Storage.Internal
{
    public class MySqlScopedTypeMapper : IMySqlScopedTypeMapper
    {
        private static readonly DateTimeTypeMapping DateTime             = new DateTimeTypeMapping("datetime", DbType.DateTime);
        private static readonly DateTimeOffsetTypeMapping DateTimeOffset = new DateTimeOffsetTypeMapping("datetime", DbType.DateTime);
        private static readonly TimeSpanTypeMapping Time                 = new TimeSpanTypeMapping("time", DbType.Time);
        private static readonly GuidTypeMapping OldGuid                  = new GuidTypeMapping("binary(16)", DbType.Guid);

        private readonly IMySqlOptions _options;
        private readonly IMySqlTypeMapper _typeMapper;

        public MySqlScopedTypeMapper(
            [NotNull] IMySqlOptions options,
            [NotNull] IMySqlTypeMapper typeMapper)
        {
            Check.NotNull(options, nameof(options));
            Check.NotNull(typeMapper, nameof(typeMapper));
            _options = options;
            _typeMapper = typeMapper;
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

        protected virtual RelationalTypeMapping MaybeConvertMapping(RelationalTypeMapping mapping)
        {
            // OldGuids
            if (_options.ConnectionSettings.OldGuids)
            {
                if (mapping.StoreType == "binary(16)" && mapping.ClrType == typeof(byte[]))
                    return OldGuid;
                if (mapping.StoreType == "char(36)" && mapping.ClrType == typeof(Guid))
                    return OldGuid;
            }

            // SupportsDateTime6
            if (!_options.ConnectionSettings.ServerVersion.SupportsDateTime6)
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
