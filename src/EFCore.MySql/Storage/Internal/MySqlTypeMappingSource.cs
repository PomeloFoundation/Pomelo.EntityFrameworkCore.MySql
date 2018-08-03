// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Data;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace Pomelo.EntityFrameworkCore.MySql.Storage.Internal
{
    public class MySqlTypeMappingSource : RelationalTypeMappingSource
    {
        // boolean
        private readonly MySqlBoolTypeMapping _bit          = new MySqlBoolTypeMapping();

        // integers
        private readonly SByteTypeMapping _tinyint          = new SByteTypeMapping("tinyint", DbType.SByte);
        private readonly ByteTypeMapping _utinyint          = new ByteTypeMapping("tinyint unsigned", DbType.Byte);
	    private readonly ShortTypeMapping _smallint         = new ShortTypeMapping("smallint", DbType.Int16);
	    private readonly UShortTypeMapping _usmallint       = new UShortTypeMapping("smallint unsigned", DbType.UInt16);
        private readonly IntTypeMapping _int                = new IntTypeMapping("int", DbType.Int32);
	    private readonly UIntTypeMapping _uint              = new UIntTypeMapping("int unsigned", DbType.UInt32);
	    private readonly LongTypeMapping _bigint            = new LongTypeMapping("bigint", DbType.Int64);
	    private readonly ULongTypeMapping _ubigint          = new ULongTypeMapping("bigint unsigned", DbType.UInt64);

	    // decimals
	    private readonly MySqlDecimalTypeMapping _decimal   = new MySqlDecimalTypeMapping("decimal(65, 30)", 65, 30);
	    private readonly MySqlDoubleTypeMapping _double     = new MySqlDoubleTypeMapping();
        private readonly MySqlFloatTypeMapping _float       = new MySqlFloatTypeMapping();

	    // binary
	    private readonly RelationalTypeMapping _binary           = new MySqlByteArrayTypeMapping(fixedLength: true);
        private readonly RelationalTypeMapping _varbinary        = new MySqlByteArrayTypeMapping();

	    // string
        private readonly MySqlStringTypeMapping _char            =
            new MySqlStringTypeMapping("char", DbType.AnsiStringFixedLength, fixedLength: true);
        private readonly MySqlStringTypeMapping _varchar         =
            new MySqlStringTypeMapping("varchar", DbType.AnsiString);
        private readonly MySqlStringTypeMapping _nchar           =
            new MySqlStringTypeMapping("nchar", DbType.StringFixedLength, unicode: true, fixedLength: true);
        private readonly MySqlStringTypeMapping _nvarchar        =
            new MySqlStringTypeMapping("nvarchar", DbType.String, unicode: true);
	    private readonly MySqlStringTypeMapping _varcharmax      =
	        new MySqlStringTypeMapping("longtext CHARACTER SET latin1", DbType.AnsiString);
        private readonly MySqlStringTypeMapping _nvarcharmax     =
            new MySqlStringTypeMapping("longtext CHARACTER SET ucs2", DbType.String, unicode: true);

        // DateTime
        private readonly MySqlDateTypeMapping _date                       = new MySqlDateTypeMapping("date");
        private readonly MySqlDateTimeTypeMapping _dateTime6              = new MySqlDateTimeTypeMapping("datetime(6)", precision: 6);
        private readonly MySqlDateTimeTypeMapping _dateTime               = new MySqlDateTimeTypeMapping("datetime");
        private readonly MySqlDateTimeTypeMapping _timeStamp6             = new MySqlDateTimeTypeMapping("timestamp(6)", precision: 6);
        private readonly MySqlDateTimeOffsetTypeMapping _dateTimeOffset6  = new MySqlDateTimeOffsetTypeMapping("datetime(6)", precision: 6);
        private readonly MySqlDateTimeOffsetTypeMapping _dateTimeOffset   = new MySqlDateTimeOffsetTypeMapping("datetime");
        private readonly MySqlDateTimeOffsetTypeMapping _timeStampOffset6 = new MySqlDateTimeOffsetTypeMapping("timestamp(6)", precision: 6);
        private readonly MySqlTimeSpanTypeMapping _time6                  = new MySqlTimeSpanTypeMapping("time(6)", precision: 6);
        private readonly MySqlTimeSpanTypeMapping _time                   = new MySqlTimeSpanTypeMapping("time");

        private readonly RelationalTypeMapping _binaryRowVersion =
            new MySqlDateTimeTypeMapping("timestamp",
                new BytesToDateTimeConverter(), new ByteArrayComparer());
        private readonly RelationalTypeMapping _binaryRowVersion6 =
            new MySqlDateTimeTypeMapping("timestamp(6)",
                new BytesToDateTimeConverter(), new ByteArrayComparer());

        // guid
	    private readonly GuidTypeMapping _uniqueidentifier   = new GuidTypeMapping("char(36)", DbType.Guid);
        private readonly GuidTypeMapping _oldGuid            = new MySqlOldGuidTypeMapping();

        readonly Dictionary<string, RelationalTypeMapping> _storeTypeMappings;
        readonly Dictionary<string, RelationalTypeMapping> _unicodeStoreTypeMappings;
        readonly Dictionary<Type, RelationalTypeMapping> _clrTypeMappings;

        // These are disallowed only if specified without any kind of length specified in parenthesis.
        private readonly HashSet<string> _disallowedMappings = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "binary",
            "char",
            "nchar",
            "varbinary",
            "varchar",
            "nvarchar"
        };

        private readonly IMySqlOptions _options;

        public MySqlTypeMappingSource(
            [NotNull] TypeMappingSourceDependencies dependencies,
            [NotNull] RelationalTypeMappingSourceDependencies relationalDependencies,
            [NotNull] IMySqlOptions options)
            : base(dependencies, relationalDependencies)
        {
            _options = options;

            _storeTypeMappings
                = new Dictionary<string, RelationalTypeMapping>(StringComparer.OrdinalIgnoreCase)
                {
                    // boolean
                    { "bit", _bit },

                    // integers
                    { "tinyint", _tinyint },
                    { "tinyint unsigned", _utinyint },
                    { "smallint", _smallint },
                    { "smallint unsigned", _usmallint },
                    { "mediumint", _int },
                    { "mediumint unsigned", _uint },
                    { "int", _int },
                    { "int unsigned", _uint },
                    { "bigint", _bigint },
                    { "bigint unsigned", _ubigint },

                    // decimals
                    { "decimal", _decimal },
                    { "dec", _decimal },
                    { "fixed", _decimal },
                    { "double", _double },
                    { "double precision", _double },
                    { "real", _double },
                    { "float", _float },

                    // binary
                    { "binary", _binary },
                    { "varbinary", _varbinary },
                    { "tinyblob", _varbinary },
                    { "blob", _varbinary },
                    { "mediumblob", _varbinary },
                    { "longblob", _varbinary },

                    // string
                    { "char", _char },
                    { "varchar", _varchar },
                    { "nchar", _nchar },
                    { "nvarchar", _nvarchar },
                    { "tinytext", _varcharmax },
                    { "text", _varcharmax },
                    { "mediumtext", _varcharmax },
                    { "longtext", _varcharmax },

                    // DateTime
                    { "date", _date }
                };

            _unicodeStoreTypeMappings
                = new Dictionary<string, RelationalTypeMapping>(StringComparer.OrdinalIgnoreCase)
                {
                    { "char", _nchar },
                    { "varchar", _nvarchar },
                    { "nchar", _nchar },
                    { "nvarchar", _nvarchar },
                    { "tinytext", _nvarcharmax },
                    { "text", _nvarcharmax },
                    { "mediumtext", _nvarcharmax },
                    { "longtext", _nvarcharmax }
                };

            _clrTypeMappings
                = new Dictionary<Type, RelationalTypeMapping>
                {
	                // boolean
	                { typeof(bool), _bit },

	                // integers
	                { typeof(short), _smallint },
	                { typeof(ushort), _usmallint },
	                { typeof(int), _int },
	                { typeof(uint), _uint },
	                { typeof(long), _bigint },
	                { typeof(ulong), _ubigint },

	                // decimals
	                { typeof(decimal), _decimal },
	                { typeof(float), _float },
	                { typeof(double), _double },

	                // byte / char
	                { typeof(sbyte), _tinyint },
	                { typeof(byte), _utinyint }
                };

            // guid
            _clrTypeMappings[typeof(Guid)] = _options.ConnectionSettings.OldGuids
                ? _oldGuid
                : _uniqueidentifier;

            // DateTime
            if (_options.ServerVersion.SupportsDateTime6)
            {
                _storeTypeMappings["time"] = _time6;
                _clrTypeMappings[typeof(DateTime)] = _dateTime6;
                _clrTypeMappings[typeof(DateTimeOffset)] = _dateTimeOffset6;
                _clrTypeMappings[typeof(TimeSpan)] = _time6;
            }
            else
            {
                _storeTypeMappings["time"] = _time;
                _clrTypeMappings[typeof(DateTime)] = _dateTime;
                _clrTypeMappings[typeof(DateTimeOffset)] = _dateTimeOffset;
                _clrTypeMappings[typeof(TimeSpan)] = _time;
            }
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected override void ValidateMapping(CoreTypeMapping mapping, IProperty property)
        {
            var relationalMapping = mapping as RelationalTypeMapping;

            if (_disallowedMappings.Contains(relationalMapping?.StoreType))
            {
                throw new ArgumentException("Unqualified data type " + relationalMapping?.StoreType);
            }
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected override RelationalTypeMapping FindMapping(in RelationalTypeMappingInfo mappingInfo)
            => FindRawMapping(mappingInfo)?.Clone(mappingInfo);

        private RelationalTypeMapping FindRawMapping(RelationalTypeMappingInfo mappingInfo)
        {
            var clrType = mappingInfo.ClrType;
            var storeTypeName = mappingInfo.StoreTypeName;
            var storeTypeNameBase = mappingInfo.StoreTypeNameBase;

            if (storeTypeName != null)
            {
                if (_options.ConnectionSettings.OldGuids)
                {
                    if (storeTypeName.Equals("binary(16)", StringComparison.OrdinalIgnoreCase)
                        && clrType == typeof(Guid))
                    {
                        return _oldGuid;
                    }
                }
                else
                {
                    if (storeTypeName.Equals("char(36)", StringComparison.OrdinalIgnoreCase)
                        && clrType == typeof(Guid))
                    {
                        return _uniqueidentifier;
                    }
                }

                if (mappingInfo.IsUnicode == true)
                {
                    if (_unicodeStoreTypeMappings.TryGetValue(storeTypeName, out var mapping)
                        || _unicodeStoreTypeMappings.TryGetValue(storeTypeNameBase, out mapping))
                    {
                        return clrType == null
                               || mapping.ClrType == clrType
                            ? mapping
                            : null;
                    }
                }
                else
                {
                    if (storeTypeNameBase.Equals("datetime", StringComparison.OrdinalIgnoreCase))
                    {
                        if (clrType == null
                            || clrType == typeof(DateTime))
                        {
                            return _options.ServerVersion.SupportsDateTime6 ? _dateTime6 : _dateTime;
                        }
                        if (clrType == typeof(DateTimeOffset))
                        {
                            return _options.ServerVersion.SupportsDateTime6 ? _dateTimeOffset6 : _dateTimeOffset;
                        }
                    } else if (storeTypeNameBase.Equals("timestamp", StringComparison.OrdinalIgnoreCase))
                    {
                        if (clrType == null
                            || clrType == typeof(DateTime))
                        {
                            return _timeStamp6;
                        }
                        if (clrType == typeof(DateTimeOffset))
                        {
                            return _timeStampOffset6;
                        }
                    }

                    if (_storeTypeMappings.TryGetValue(storeTypeName, out var mapping)
                        || _storeTypeMappings.TryGetValue(storeTypeNameBase, out mapping))
                    {
                        return clrType == null
                               || mapping.ClrType == clrType
                            ? mapping
                            : null;
                    }
                }
            }

            if (clrType != null)
            {
                if (_clrTypeMappings.TryGetValue(clrType, out var mapping))
                {
                    return mapping;
                }

                if (clrType.TryGetElementType(typeof(JsonObject<>)) != null)
                {
                    return new MySqlJsonTypeMapping(clrType, unicode: mappingInfo.IsUnicode);
                }

                if (clrType == typeof(string))
                {
                    // Some of this logic could be moved into MySqlStringTypeMapping once EF #11896 is fixed
                    var isAnsi = mappingInfo.IsUnicode == false;
                    var isFixedLength = mappingInfo.IsFixedLength == true;
                    var charSetSuffix = "";
                    var bytesPerChar = isAnsi
                        ? _options.AnsiCharSetInfo.BytesPerChar
                        : _options.UnicodeCharSetInfo.BytesPerChar;

                    if (isAnsi && (
                        (mappingInfo.IsKeyOrIndex &&
                            (_options.CharSetBehavior & CharSetBehavior.AppendToAnsiIndexAndKeyColumns)!= 0)
                        ||
                        (!mappingInfo.IsKeyOrIndex &&
                            (_options.CharSetBehavior & CharSetBehavior.AppendToAnsiNonIndexAndKeyColumns) != 0)
                        ))
                    {
                        charSetSuffix = $" CHARACTER SET {_options.AnsiCharSetInfo.CharSetName}";
                    }

                    if (!isAnsi && (
                        (mappingInfo.IsKeyOrIndex &&
                             (_options.CharSetBehavior & CharSetBehavior.AppendToUnicodeIndexAndKeyColumns)!= 0)
                        ||
                        (!mappingInfo.IsKeyOrIndex &&
                             (_options.CharSetBehavior & CharSetBehavior.AppendToUnicodeNonIndexAndKeyColumns) != 0)
                        ))
                    {
                        charSetSuffix = $" CHARACTER SET {_options.UnicodeCharSetInfo.CharSetName}";
                    }

                    var maxSize = 8000 / bytesPerChar;

                    var size = mappingInfo.Size ??
                               (mappingInfo.IsKeyOrIndex
                                   // Allow to use at most half of the max key length, so at least 2 columns can fit
                                   ? Math.Min(_options.ServerVersion.IndexMaxBytes / (bytesPerChar * 2), 255)
                                   : (int?)null);
                    if (size > maxSize)
                    {
                        size = null;
                    }

                    var dbType = isAnsi
                        ? (isFixedLength ? DbType.AnsiStringFixedLength : DbType.AnsiString)
                        : (isFixedLength ? DbType.StringFixedLength : DbType.String);

                    return new MySqlStringTypeMapping(
                        size == null
                            ? "longtext" + charSetSuffix
                            : (isFixedLength ? "char(" : "varchar(") + size + ")" + charSetSuffix,
                        dbType,
                        !isAnsi,
                        size,
                        isFixedLength);
                }

                if (clrType == typeof(byte[]))
                {
                    if (mappingInfo.IsRowVersion == true)
                    {
                        return _options.ServerVersion.SupportsDateTime6 ? _binaryRowVersion6 : _binaryRowVersion;
                    }

                    var size = mappingInfo.Size ??
                               (mappingInfo.IsKeyOrIndex ? _options.ServerVersion.IndexMaxBytes : (int?)null);

                    return new MySqlByteArrayTypeMapping(
                        size: size,
                        fixedLength: mappingInfo.IsFixedLength == true);
                }
            }

            return null;
        }
    }
}
