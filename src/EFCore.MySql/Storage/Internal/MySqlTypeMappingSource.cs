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
using Pomelo.EntityFrameworkCore.MySql.Internal;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

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

        //
        // String mappings depend on the MySqlOptions.NoBackslashEscapes setting:
        //

        private MySqlStringTypeMapping _charUnicode;
        private MySqlStringTypeMapping _varcharUnicode;
        private MySqlStringTypeMapping _varcharmaxUnicode;

        private MySqlStringTypeMapping _nchar;
        private MySqlStringTypeMapping _nvarchar;

        private MySqlStringTypeMapping _enum;

        // DateTime
        private readonly MySqlDateTypeMapping _date = new MySqlDateTypeMapping("date", DbType.Date);
        private readonly MySqlDateTimeTypeMapping _dateTime6 = new MySqlDateTimeTypeMapping("datetime", typeof(DateTime), precision: 6);
        private readonly MySqlDateTimeTypeMapping _dateTime = new MySqlDateTimeTypeMapping("datetime", typeof(DateTime));
        private readonly MySqlDateTimeTypeMapping _timeStamp6 = new MySqlDateTimeTypeMapping("timestamp", typeof(DateTime), precision: 6);
        private readonly MySqlDateTimeOffsetTypeMapping _dateTimeOffset6 = new MySqlDateTimeOffsetTypeMapping("datetime", precision: 6);
        private readonly MySqlDateTimeOffsetTypeMapping _dateTimeOffset = new MySqlDateTimeOffsetTypeMapping("datetime");
        private readonly MySqlDateTimeOffsetTypeMapping _timeStampOffset6 = new MySqlDateTimeOffsetTypeMapping("timestamp", precision: 6);
        private readonly MySqlTimeSpanTypeMapping _time6 = new MySqlTimeSpanTypeMapping("time", precision: 6);
        private readonly MySqlTimeSpanTypeMapping _time = new MySqlTimeSpanTypeMapping("time");

        private readonly RelationalTypeMapping _binaryRowVersion
            = new MySqlDateTimeTypeMapping(
                "timestamp",
                typeof(byte[]),
                new BytesToDateTimeConverter(),
                new ByteArrayComparer());
        private readonly RelationalTypeMapping _binaryRowVersion6
            = new MySqlDateTimeTypeMapping(
                "timestamp",
                typeof(byte[]),
                new BytesToDateTimeConverter(),
                new ByteArrayComparer(),
                precision: 6);

        // guid
        private GuidTypeMapping _guid;

        // Scaffolding type mappings
        private readonly MySqlCodeGenerationMemberAccessTypeMapping _codeGenerationMemberAccess = new MySqlCodeGenerationMemberAccessTypeMapping();

        private Dictionary<string, RelationalTypeMapping> _storeTypeMappings;
        private Dictionary<Type, RelationalTypeMapping> _clrTypeMappings;
        private Dictionary<Type, RelationalTypeMapping> _scaffoldingClrTypeMappings;

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

        private bool _initialized;
        private readonly object _initializationLock = new object();

        public MySqlTypeMappingSource(
            [NotNull] TypeMappingSourceDependencies dependencies,
            [NotNull] RelationalTypeMappingSourceDependencies relationalDependencies,
            [NotNull] IMySqlOptions options)
            : base(dependencies, relationalDependencies)
        {
            _options = options;
        }

        private void Initialize()
        {
            //
            // String mappings depend on the MySqlOptions.NoBackslashEscapes setting:
            //

            _charUnicode = new MySqlStringTypeMapping("char", DbType.StringFixedLength, _options, unicode: true, fixedLength: true);
            _varcharUnicode = new MySqlStringTypeMapping("varchar", DbType.String, _options, unicode: true);
            _varcharmaxUnicode = new MySqlStringTypeMapping("longtext", DbType.String, _options, unicode: true);

            _nchar = new MySqlStringTypeMapping("nchar", DbType.StringFixedLength, _options, unicode: true, fixedLength: true);
            _nvarchar = new MySqlStringTypeMapping("nvarchar", DbType.String, _options, unicode: true);

            _enum = new MySqlStringTypeMapping("enum", DbType.String, _options, unicode: true);

            _guid = MySqlGuidTypeMapping.IsValidGuidFormat(_options.ConnectionSettings.GuidFormat)
                ? new MySqlGuidTypeMapping(_options.ConnectionSettings.GuidFormat)
                : null;

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
                    { "integer", _int },
                    { "integer unsigned", _uint },
                    { "bigint", _bigint },
                    { "bigint unsigned", _ubigint },

                    // decimals
                    { "decimal", _decimal },
                    { "decimal unsigned", _decimal }, // deprecated since 8.0.17-mysql
                    { "numeric", _decimal },
                    { "numeric unsigned", _decimal }, // deprecated since 8.0.17-mysql
                    { "dec", _decimal },
                    { "dec unsigned", _decimal }, // deprecated since 8.0.17-mysql
                    { "fixed", _decimal },
                    { "fixed unsigned", _decimal }, // deprecated since 8.0.17-mysql
                    { "double", _double },
                    { "double unsigned", _double }, // deprecated since 8.0.17-mysql
                    { "double precision", _double },
                    { "double precision unsigned", _double }, // deprecated since 8.0.17-mysql
                    { "real", _double },
                    { "real unsigned", _double }, // deprecated since 8.0.17-mysql
                    { "float", _float },
                    { "float unsigned", _float }, // deprecated since 8.0.17-mysql

                    // binary
                    { "binary", _binary },
                    { "varbinary", _varbinary },
                    { "tinyblob", _varbinary },
                    { "blob", _varbinary },
                    { "mediumblob", _varbinary },
                    { "longblob", _varbinary },

                    // string
                    { "char", _charUnicode },
                    { "varchar", _varcharUnicode },
                    { "tinytext", _varcharmaxUnicode },
                    { "text", _varcharmaxUnicode },
                    { "mediumtext", _varcharmaxUnicode },
                    { "longtext", _varcharmaxUnicode },

                    { "enum", _enum },

                    { "nchar", _nchar },
                    { "nvarchar", _nvarchar },

                    // DateTime
                    { "date", _date }
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
                    { typeof(byte), _utinyint },
                };

            // Boolean
            if (_options.DefaultDataTypeMappings.ClrBoolean != MySqlBooleanType.None)
            {
                _clrTypeMappings[typeof(bool)] = _options.DefaultDataTypeMappings.ClrBoolean == MySqlBooleanType.Bit1
                    ? _bit1
                    : _tinyint1;
            }

            // DateTime
            _storeTypeMappings["time"] = !_options.ServerVersion.SupportsDateTime6 ||
                                         _options.DefaultDataTypeMappings.ClrTimeSpan == MySqlTimeSpanType.Time
                ? _time
                : _time6;

            _clrTypeMappings[typeof(TimeSpan)] = !_options.ServerVersion.SupportsDateTime6 ||
                                                 _options.DefaultDataTypeMappings.ClrTimeSpan == MySqlTimeSpanType.Time
                ? _time
                : _time6;

            _clrTypeMappings[typeof(DateTime)] = !_options.ServerVersion.SupportsDateTime6 ||
                                                 _options.DefaultDataTypeMappings.ClrDateTime == MySqlDateTimeType.DateTime
                ? _dateTime
                : _options.DefaultDataTypeMappings.ClrDateTime == MySqlDateTimeType.Timestamp6
                    ? _timeStamp6
                    : _dateTime6;

            _clrTypeMappings[typeof(DateTimeOffset)] = !_options.ServerVersion.SupportsDateTime6 ||
                                                       _options.DefaultDataTypeMappings.ClrDateTime == MySqlDateTimeType.DateTime
                ? _dateTimeOffset
                : _options.DefaultDataTypeMappings.ClrDateTimeOffset == MySqlDateTimeType.Timestamp6
                    ? _timeStampOffset6
                    : _dateTimeOffset6;

            // Guid
            if (_guid != null)
            {
                _clrTypeMappings[typeof(Guid)] = _guid;
            }

            // Type mappings that only exist to work around the limited code generation capabilites when scaffolding:
            _scaffoldingClrTypeMappings = new Dictionary<Type, RelationalTypeMapping>
            {
                { typeof(MySqlCodeGenerationMemberAccess), _codeGenerationMemberAccess }
            };
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
            // This is a singleton class and therefore needs to be thread-safe.
            if (!_initialized)
            {
                lock (_initializationLock)
                {
                    if (!_initialized)
                    {
                        Initialize();
                        _initialized = true;
                    }
                }
            }

            var clrType = mappingInfo.ClrType;
            var storeTypeName = mappingInfo.StoreTypeName;
            var storeTypeNameBase = mappingInfo.StoreTypeNameBase;

            if (storeTypeName != null)
            {
                if (_options.DefaultDataTypeMappings.ClrBoolean != MySqlBooleanType.None)
                {
                    if (_options.DefaultDataTypeMappings.ClrBoolean == MySqlBooleanType.Bit1)
                    {
                        if (storeTypeName.Equals(_bit1.StoreType, StringComparison.OrdinalIgnoreCase))
                        {
                            return _bit1;
                        }
                    }
                    else
                    {
                        if (storeTypeName.Equals(_tinyint1.StoreType, StringComparison.OrdinalIgnoreCase))
                        {
                            return _tinyint1;
                        }
                    }
                }
                
                if (MySqlGuidTypeMapping.IsValidGuidFormat(_options.ConnectionSettings.GuidFormat))
                {
                    if (storeTypeName.Equals(_guid.StoreType, StringComparison.OrdinalIgnoreCase)
                        && (clrType == typeof(Guid) || clrType == null))
                    {
                        return _guid;
                    }
                }

                if (storeTypeNameBase.Equals(_dateTime6.StoreTypeNameBase, StringComparison.OrdinalIgnoreCase))
                {
                    if (clrType == null
                        || clrType == typeof(DateTime))
                    {
                        if (mappingInfo.Precision.GetValueOrDefault() > 0 ||
                            _options.ServerVersion.SupportsDateTime6 && _options.DefaultDataTypeMappings.ClrDateTime != MySqlDateTimeType.DateTime)
                        {
                            return _dateTime6;
                        }
                        else
                        {
                            return _dateTime;
                        }
                    }

                    if (clrType == typeof(DateTimeOffset))
                    {
                        if (mappingInfo.Precision.GetValueOrDefault() > 0 ||
                            _options.ServerVersion.SupportsDateTime6 && _options.DefaultDataTypeMappings.ClrDateTimeOffset != MySqlDateTimeType.DateTime)
                        {
                            return _dateTimeOffset6;
                        }
                        else
                        {
                            return _dateTimeOffset;
                        }
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
                    var isNationalCharSet = storeTypeNameBase != null
                                            && (storeTypeNameBase.Equals(_nchar.StoreTypeNameBase, StringComparison.OrdinalIgnoreCase)
                                                || storeTypeNameBase.Equals(_nvarchar.StoreTypeNameBase, StringComparison.OrdinalIgnoreCase));
                    var isFixedLength = mappingInfo.IsFixedLength == true;
                    var charset = isNationalCharSet ? _options.NationalCharSet : _options.CharSet;
                    var isUnicode = mappingInfo.IsUnicode ?? charset.IsUnicode;
                    var bytesPerChar = charset.MaxBytesPerChar;
                    var charSetSuffix = string.Empty;

                    if (isUnicode &&
                            (mappingInfo.IsKeyOrIndex &&
                                (_options.CharSetBehavior & CharSetBehavior.AppendToUnicodeIndexAndKeyColumns) != 0 ||
                            !mappingInfo.IsKeyOrIndex &&
                                (_options.CharSetBehavior & CharSetBehavior.AppendToUnicodeNonIndexAndKeyColumns) != 0) ||
                        !isUnicode &&
                            (mappingInfo.IsKeyOrIndex &&
                                (_options.CharSetBehavior & CharSetBehavior.AppendToAnsiIndexAndKeyColumns) != 0 ||
                            !mappingInfo.IsKeyOrIndex &&
                                (_options.CharSetBehavior & CharSetBehavior.AppendToAnsiNonIndexAndKeyColumns) != 0))
                    {
                        charSetSuffix = $" CHARACTER SET {(isNationalCharSet ? _options.NationalCharSet : _options.CharSet).Name}";
                    }

                    var maxSize = 8000 / bytesPerChar;

                    var size = mappingInfo.Size ??
                               (mappingInfo.IsKeyOrIndex
                                   // Allow to use at most half of the max key length, so at least 2 columns can fit
                                   ? Math.Min(_options.ServerVersion.MaxKeyLength / (bytesPerChar * 2), 255)
                                   : (int?)null);
                    if (size > maxSize)
                    {
                        size = null;
                    }

                    var dbType = isUnicode
                        ? isFixedLength ? DbType.StringFixedLength : DbType.String
                        : isFixedLength ? DbType.AnsiStringFixedLength : DbType.AnsiString;

                    return new MySqlStringTypeMapping(
                        size == null
                            ? "longtext" + charSetSuffix
                            : (isNationalCharSet ? "n" : string.Empty) +
                              (isFixedLength ? "char(" : "varchar(") + size + ")" + charSetSuffix,
                        dbType,
                        _options,
                        isUnicode,
                        size,
                        isFixedLength);
                }

                if (clrType == typeof(byte[]))
                {
                    if (mappingInfo.IsRowVersion == true)
                    {
                        return _options.ServerVersion.SupportsDateTime6
                            ? _binaryRowVersion6
                            : _binaryRowVersion;
                    }

                    var size = mappingInfo.Size ??
                               (mappingInfo.IsKeyOrIndex ? _options.ServerVersion.MaxKeyLength : (int?)null);

                    return new MySqlByteArrayTypeMapping(
                        size: size,
                        fixedLength: mappingInfo.IsFixedLength == true);
                }

                if (_scaffoldingClrTypeMappings.TryGetValue(clrType, out mapping))
                {
                    return mapping;
                }
            }

            return null;
        }

        protected override string ParseStoreTypeName(string storeTypeName, out bool? unicode, out int? size, out int? precision, out int? scale)
        {
            var storeTypeBaseName = base.ParseStoreTypeName(storeTypeName, out unicode, out size, out precision, out scale);

            if ((storeTypeName?.IndexOf("unsigned", StringComparison.OrdinalIgnoreCase) ?? -1) >= 0)
            {
                return storeTypeBaseName + " unsigned";
            }

            return storeTypeBaseName;
        }
    }
}
