// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Data;
using EFCore.MySql.Infrastructure.Internal;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;

namespace EFCore.MySql.Storage.Internal
{
    public class MySqlSmartTypeMapper : MySqlTypeMapper
    {
        private static readonly MySqlDateTimeTypeMapping _dateTime = new MySqlDateTimeTypeMapping("datetime", DbType.DateTime);
        private static readonly MySqlDateTimeOffsetTypeMapping _dateTimeOffset = new MySqlDateTimeOffsetTypeMapping("datetime", DbType.DateTime);
        private static readonly TimeSpanTypeMapping _time = new TimeSpanTypeMapping("time", DbType.Time);
        private static readonly GuidTypeMapping _oldGuid = new GuidTypeMapping("binary(16)", DbType.Guid);

        private readonly IMySqlOptions _options;

        public MySqlSmartTypeMapper(
            [NotNull] RelationalTypeMapperDependencies dependencies,
            [NotNull] IMySqlOptions options)
            : base(dependencies)
        {
            Check.NotNull(options, nameof(options));
            _options = options;
        }

        public override RelationalTypeMapping FindMapping(IProperty property)
        {
            var mapping = base.FindMapping(property);
            return mapping == null ? null : MaybeConvertMapping(mapping);
        }

        public override RelationalTypeMapping FindMapping(Type clrType)
        {
            var mapping = base.FindMapping(clrType);
            return mapping == null ? null : MaybeConvertMapping(mapping);
        }

        public override RelationalTypeMapping FindMapping(string storeType)
        {
            var mapping = base.FindMapping(storeType);
            return mapping == null ? null : MaybeConvertMapping(mapping);
        }

        protected virtual RelationalTypeMapping MaybeConvertMapping(RelationalTypeMapping mapping)
        {
            // OldGuids
            if (_options.ConnectionSettings.OldGuids)
            {
                if (mapping.StoreType == "binary(16)" && mapping.ClrType == typeof(byte[]))
                {
                    return _oldGuid;
                }

                if (mapping.StoreType == "char(36)" && mapping.ClrType == typeof(Guid))
                {
                    return _oldGuid;
                }
            }

            // SupportsDateTime6
            if (!_options.ConnectionSettings.ServerVersion.SupportsDateTime6)
            {
                if (mapping.StoreType == "datetime(6)" && mapping.ClrType == typeof(DateTime))
                {
                    return _dateTime;
                }

                if (mapping.StoreType == "datetime(6)" && mapping.ClrType == typeof(DateTimeOffset))
                {
                    return _dateTimeOffset;
                }

                if (mapping.StoreType == "time(6)")
                {
                    return _time;
                }
            }

            return mapping;
        }
    }
}
