// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace Pomelo.EntityFrameworkCore.MySql.Storage.Internal
{
    // ReSharper disable PrivateFieldCanBeConvertedToLocalVariable
    public class MySqlTypeMappingSource : RelationalTypeMappingSource
    {
        // boolean
        private readonly MySqlBoolTypeMapping _bit1 = new MySqlBoolTypeMapping("bit", size: 1);
        private readonly MySqlBoolTypeMapping _tinyint1 = new MySqlBoolTypeMapping("tinyint", size: 1);

        // bit
        private readonly ULongTypeMapping _bit = new ULongTypeMapping("bit", DbType.UInt64);

        // integers
        private readonly SByteTypeMapping _tinyint = new SByteTypeMapping("tinyint", DbType.SByte);
        private readonly ByteTypeMapping _utinyint = new ByteTypeMapping("tinyint unsigned", DbType.Byte);
        private readonly ShortTypeMapping _smallint = new ShortTypeMapping("smallint", DbType.Int16);
        private readonly UShortTypeMapping _usmallint = new UShortTypeMapping("smallint unsigned", DbType.UInt16);
        private readonly IntTypeMapping _int = new IntTypeMapping("int", DbType.Int32);
        private readonly UIntTypeMapping _uint = new UIntTypeMapping("int unsigned", DbType.UInt32);
        private readonly LongTypeMapping _bigint = new LongTypeMapping("bigint", DbType.Int64);
        private readonly ULongTypeMapping _ubigint = new ULongTypeMapping("bigint unsigned", DbType.UInt64);

        // decimals
        private readonly MySqlDecimalTypeMapping _decimal = new MySqlDecimalTypeMapping("decimal", precision: 65, scale: 30);
        private readonly MySqlDoubleTypeMapping _double = new MySqlDoubleTypeMapping("double", DbType.Double);
        private readonly MySqlFloatTypeMapping _float = new MySqlFloatTypeMapping("float", DbType.Single);

        // binary
        private readonly RelationalTypeMapping _binary = new MySqlByteArrayTypeMapping(fixedLength: true);
        private readonly RelationalTypeMapping _varbinary = new MySqlByteArrayTypeMapping();

        // String mappings depend on the MySqlOptions.NoBackslashEscapes setting.
        private MySqlStringTypeMapping _char;
        private MySqlStringTypeMapping _varchar;
        private MySqlStringTypeMapping _nchar;
        private MySqlStringTypeMapping _nvarchar;
        private MySqlStringTypeMapping _varcharmax;
        private MySqlStringTypeMapping _nvarcharmax;
        private MySqlStringTypeMapping _enum;

        // DateTime
        private readonly MySqlDateTypeMapping _date = new MySqlDateTypeMapping("date", DbType.Date);
        private readonly MySqlDateTimeTypeMapping _dateTime6 = new MySqlDateTimeTypeMapping("datetime", precision: 6);
        private readonly MySqlDateTimeTypeMapping _dateTime = new MySqlDateTimeTypeMapping("datetime");
        private readonly MySqlDateTimeTypeMapping _timeStamp6 = new MySqlDateTimeTypeMapping("timestamp", precision: 6);
        private readonly MySqlDateTimeOffsetTypeMapping _dateTimeOffset6 = new MySqlDateTimeOffsetTypeMapping("datetime", precision: 6);
        private readonly MySqlDateTimeOffsetTypeMapping _dateTimeOffset = new MySqlDateTimeOffsetTypeMapping("datetime");
        private readonly MySqlDateTimeOffsetTypeMapping _timeStampOffset6 = new MySqlDateTimeOffsetTypeMapping("timestamp", precision: 6);
        private readonly MySqlTimeSpanTypeMapping _time6 = new MySqlTimeSpanTypeMapping("time", precision: 6);
        private readonly MySqlTimeSpanTypeMapping _time = new MySqlTimeSpanTypeMapping("time");

        private readonly RelationalTypeMapping _binaryRowVersion
            = new MySqlDateTimeTypeMapping(
                "timestamp",
                new BytesToDateTimeConverter(),
                new ByteArrayComparer());
        private readonly RelationalTypeMapping _binaryRowVersion6
            = new MySqlDateTimeTypeMapping(
                "timestamp",
                new BytesToDateTimeConverter(),
                new ByteArrayComparer(),
                precision: 6);

        // guid
        private readonly GuidTypeMapping _uniqueidentifier = new MySqlGuidTypeMapping("char", DbType.Guid, size: 36);
        private readonly GuidTypeMapping _oldGuid = new MySqlOldGuidTypeMapping();

        private Dictionary<string, RelationalTypeMapping> _storeTypeMappings;
        private Dictionary<string, RelationalTypeMapping> _unicodeStoreTypeMappings;
        private Dictionary<Type, RelationalTypeMapping> _clrTypeMappings;

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
        private readonly IMySqlConnectionInfo _connectionInfo;

        private bool _initialized;

        public MySqlTypeMappingSource(
            [NotNull] TypeMappingSourceDependencies dependencies,
            [NotNull] RelationalTypeMappingSourceDependencies relationalDependencies,
            [NotNull] IMySqlOptions options,
            [NotNull] IMySqlConnectionInfo connectionInfo)
            : base(dependencies, relationalDependencies)
        {
            _options = options;
            _connectionInfo = connectionInfo;
        }

        protected void Initialize()
        {
            // String mappings depend on the MySqlOptions.NoBackslashEscapes setting.
            _char = new MySqlStringTypeMapping("char", DbType.AnsiStringFixedLength, fixedLength: true, noBackslashEscapes: _options.NoBackslashEscapes);
            _varchar = new MySqlStringTypeMapping("varchar", DbType.AnsiString, noBackslashEscapes: _options.NoBackslashEscapes);
            _nchar = new MySqlStringTypeMapping("nchar", DbType.StringFixedLength, unicode: true, fixedLength: true, noBackslashEscapes: _options.NoBackslashEscapes);
            _nvarchar = new MySqlStringTypeMapping("nvarchar", DbType.String, unicode: true, noBackslashEscapes: _options.NoBackslashEscapes);
            _varcharmax = new MySqlStringTypeMapping("longtext", DbType.AnsiString, noBackslashEscapes: _options.NoBackslashEscapes);
            _nvarcharmax = new MySqlStringTypeMapping("longtext", DbType.String, unicode: true, noBackslashEscapes: _options.NoBackslashEscapes);
            _enum = new MySqlStringTypeMapping("enum", DbType.String, unicode: true, noBackslashEscapes: _options.NoBackslashEscapes);
            
            _storeTypeMappings
                = new Dictionary<string, RelationalTypeMapping>(StringComparer.OrdinalIgnoreCase)
                {
                    // bit
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
                    { "enum", _enum },

                    // DateTime
                    { "date", _date }
                };

            _unicodeStoreTypeMappings = new Dictionary<string, RelationalTypeMapping>(StringComparer.OrdinalIgnoreCase)
                {
                    {"char", _nchar},
                    {"varchar", _nvarchar},
                    {"nchar", _nchar},
                    {"nvarchar", _nvarchar},
                    {"tinytext", _nvarcharmax},
                    {"text", _nvarcharmax},
                    {"mediumtext", _nvarcharmax},
                    {"longtext", _nvarcharmax},
                    {"enum", _enum}
                };

            _clrTypeMappings
                = new Dictionary<Type, RelationalTypeMapping>
                {
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

            // Boolean
            _clrTypeMappings[typeof(bool)] = _options.ConnectionSettings.TreatTinyAsBoolean
                ? _tinyint1
                : _bit1;

            // Guid
            _clrTypeMappings[typeof(Guid)] = _options.ConnectionSettings.OldGuids
                ? _oldGuid
                : _uniqueidentifier;

            // DateTime
            if (_connectionInfo.ServerVersion.SupportsDateTime6)
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
                throw new ArgumentException($@"Missing length for data type ""{relationalMapping?.StoreType}"".");
            }
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected override RelationalTypeMapping FindMapping(in RelationalTypeMappingInfo mappingInfo) =>
            // first, try any plugins, allowing them to override built-in mappings
            base.FindMapping(mappingInfo) ??
            FindRawMapping(mappingInfo)?.Clone(mappingInfo);

        private RelationalTypeMapping FindRawMapping(RelationalTypeMappingInfo mappingInfo)
        {
            // Use deferred initialization to support connection (string) based type mapping in
            // design time mode (scaffolder etc.).
            if (!_initialized)
            {
                Initialize();
                _initialized = true;
            }

            var clrType = mappingInfo.ClrType;
            var storeTypeName = mappingInfo.StoreTypeName;
            var storeTypeNameBase = mappingInfo.StoreTypeNameBase;

            if (storeTypeName != null)
            {
                if (_options.ConnectionSettings.TreatTinyAsBoolean)
                {
                    if (storeTypeName.Equals(_tinyint1.StoreType, StringComparison.OrdinalIgnoreCase))
                    {
                        return _tinyint1;
                    }
                }
                else
                {
                    if (storeTypeName.Equals(_bit1.StoreType, StringComparison.OrdinalIgnoreCase))
                    {
                        return _bit1;
                    }
                }

                if (_options.ConnectionSettings.OldGuids)
                {
                    if (storeTypeName.Equals(_oldGuid.StoreType, StringComparison.OrdinalIgnoreCase)
                        && clrType == typeof(Guid))
                    {
                        return _oldGuid;
                    }
                }
                else
                {
                    if (storeTypeName.Equals(_uniqueidentifier.StoreType, StringComparison.OrdinalIgnoreCase)
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
                    if (storeTypeNameBase.Equals(_dateTime6.StoreTypeNameBase, StringComparison.OrdinalIgnoreCase))
                    {
                        if (clrType == null
                            || clrType == typeof(DateTime))
                        {
                            return _connectionInfo.ServerVersion.SupportsDateTime6 ? _dateTime6 : _dateTime;
                        }
                        if (clrType == typeof(DateTimeOffset))
                        {
                            return _connectionInfo.ServerVersion.SupportsDateTime6 ? _dateTimeOffset6 : _dateTimeOffset;
                        }
                    }
                    else if (storeTypeNameBase.Equals(_timeStamp6.StoreTypeNameBase, StringComparison.OrdinalIgnoreCase))
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
                            (_options.CharSetBehavior & CharSetBehavior.AppendToAnsiIndexAndKeyColumns) != 0)
                        ||
                        (!mappingInfo.IsKeyOrIndex &&
                            (_options.CharSetBehavior & CharSetBehavior.AppendToAnsiNonIndexAndKeyColumns) != 0)
                        ))
                    {
                        charSetSuffix = $" CHARACTER SET {_options.AnsiCharSetInfo.CharSetName}";
                    }

                    if (!isAnsi && (
                        (mappingInfo.IsKeyOrIndex &&
                             (_options.CharSetBehavior & CharSetBehavior.AppendToUnicodeIndexAndKeyColumns) != 0)
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
                                   ? Math.Min(_connectionInfo.ServerVersion.MaxKeyLength / (bytesPerChar * 2), 255)
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
                        isFixedLength,
                        _options.NoBackslashEscapes);
                }

                if (clrType == typeof(byte[]))
                {
                    if (mappingInfo.IsRowVersion == true)
                    {
                        return _connectionInfo.ServerVersion.SupportsDateTime6 ? _binaryRowVersion6 : _binaryRowVersion;
                    }

                    var size = mappingInfo.Size ??
                               (mappingInfo.IsKeyOrIndex ? _connectionInfo.ServerVersion.MaxKeyLength : (int?)null);

                    return new MySqlByteArrayTypeMapping(
                        size: size,
                        fixedLength: mappingInfo.IsFixedLength == true);
                }
            }

            return null;
        }
    }
}
