﻿// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Storage.Internal
{
    // ReSharper disable PrivateFieldCanBeConvertedToLocalVariable
    public class MySqlTypeMappingSource : RelationalTypeMappingSource
    {
        // boolean
        private readonly MySqlBoolTypeMapping _bit1 = new MySqlBoolTypeMapping("bit", size: 1);
        private readonly MySqlBoolTypeMapping _tinyint1 = MySqlBoolTypeMapping.Default;

        // bit
        private readonly MySqlULongTypeMapping _bit = new MySqlULongTypeMapping("bit");

        // integers
        private readonly MySqlSByteTypeMapping _tinyint = MySqlSByteTypeMapping.Default;
        private readonly MySqlByteTypeMapping _utinyint = MySqlByteTypeMapping.Default;
        private readonly MySqlShortTypeMapping _smallint = MySqlShortTypeMapping.Default;
        private readonly MySqlUShortTypeMapping _usmallint = MySqlUShortTypeMapping.Default;
        private readonly MySqlIntTypeMapping _int = MySqlIntTypeMapping.Default;
        private readonly MySqlUIntTypeMapping _uint = MySqlUIntTypeMapping.Default;
        private readonly MySqlLongTypeMapping _bigint = MySqlLongTypeMapping.Default;
        private readonly MySqlULongTypeMapping _ubigint = MySqlULongTypeMapping.Default;

        // decimals
        private readonly MySqlDecimalTypeMapping _decimal = MySqlDecimalTypeMapping.Default;
        private readonly MySqlDoubleTypeMapping _double = MySqlDoubleTypeMapping.Default;
        private readonly MySqlFloatTypeMapping _float = MySqlFloatTypeMapping.Default;

        // binary
        private readonly RelationalTypeMapping _binary = new MySqlByteArrayTypeMapping(fixedLength: true);
        private readonly RelationalTypeMapping _varbinary = MySqlByteArrayTypeMapping.Default;

        //
        // String mappings depend on the MySqlOptions.NoBackslashEscapes setting:
        //

        private MySqlStringTypeMapping _charUnicode;
        private MySqlStringTypeMapping _varcharUnicode;
        private MySqlStringTypeMapping _tinytextUnicode;
        private MySqlStringTypeMapping _textUnicode;
        private MySqlStringTypeMapping _mediumtextUnicode;
        private MySqlStringTypeMapping _longtextUnicode;

        private MySqlStringTypeMapping _nchar;
        private MySqlStringTypeMapping _nvarchar;

        private MySqlStringTypeMapping _enum;

        // DateTime
        private readonly MySqlYearTypeMapping _year = MySqlYearTypeMapping.Default;
        private readonly MySqlDateTypeMapping _dateDateOnly = MySqlDateTypeMapping.Default;
        private readonly MySqlDateTypeMapping _dateDateTime = new MySqlDateTypeMapping("date", typeof(DateTime));
        private readonly MySqlTimeTypeMapping _timeTimeOnly = MySqlTimeTypeMapping.Default;
        private readonly MySqlTimeTypeMapping _timeTimeSpan = new MySqlTimeTypeMapping("time", typeof(TimeSpan));
        private readonly MySqlDateTimeTypeMapping _dateTime = MySqlDateTimeTypeMapping.Default;
        private readonly MySqlDateTimeTypeMapping _timeStamp = new MySqlDateTimeTypeMapping("timestamp");
        private readonly MySqlDateTimeOffsetTypeMapping _dateTimeOffset = MySqlDateTimeOffsetTypeMapping.Default;
        private readonly MySqlDateTimeOffsetTypeMapping _timeStampOffset = new MySqlDateTimeOffsetTypeMapping("timestamp");

        private readonly RelationalTypeMapping _binaryRowVersion
            = new MySqlDateTimeTypeMapping(
                "timestamp",
                null,
                typeof(byte[]),
                new BytesToDateTimeConverter(),
                new ByteArrayComparer());
        private readonly RelationalTypeMapping _binaryRowVersion6
            = new MySqlDateTimeTypeMapping(
                "timestamp",
                6,
                typeof(byte[]),
                new BytesToDateTimeConverter(),
                new ByteArrayComparer());

        // guid
        private GuidTypeMapping _guid;

        // JSON default mapping
        private MySqlJsonTypeMapping<string> _jsonDefaultString;

        // Scaffolding type mappings
        private readonly MySqlCodeGenerationMemberAccessTypeMapping _codeGenerationMemberAccess = MySqlCodeGenerationMemberAccessTypeMapping.Default;
        private readonly MySqlCodeGenerationServerVersionCreationTypeMapping _codeGenerationServerVersionCreation = MySqlCodeGenerationServerVersionCreationTypeMapping.Default;

        private Dictionary<string, RelationalTypeMapping[]> _storeTypeMappings;
        private Dictionary<Type, RelationalTypeMapping> _clrTypeMappings;
        private Dictionary<Type, RelationalTypeMapping> _scaffoldingClrTypeMappings;

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

            _charUnicode = new MySqlStringTypeMapping("char", StoreTypePostfix.Size, fixedLength: true, noBackslashEscapes: _options.NoBackslashEscapes, replaceLineBreaksWithCharFunction: _options.ReplaceLineBreaksWithCharFunction);
            _varcharUnicode = new MySqlStringTypeMapping("varchar", StoreTypePostfix.Size, noBackslashEscapes: _options.NoBackslashEscapes, replaceLineBreaksWithCharFunction: _options.ReplaceLineBreaksWithCharFunction);
            _tinytextUnicode = new MySqlStringTypeMapping("tinytext", StoreTypePostfix.None, noBackslashEscapes: _options.NoBackslashEscapes, replaceLineBreaksWithCharFunction: _options.ReplaceLineBreaksWithCharFunction);
            _textUnicode = new MySqlStringTypeMapping("text", StoreTypePostfix.None, noBackslashEscapes: _options.NoBackslashEscapes, replaceLineBreaksWithCharFunction: _options.ReplaceLineBreaksWithCharFunction);
            _mediumtextUnicode = new MySqlStringTypeMapping("mediumtext", StoreTypePostfix.None, noBackslashEscapes: _options.NoBackslashEscapes, replaceLineBreaksWithCharFunction: _options.ReplaceLineBreaksWithCharFunction);
            _longtextUnicode = new MySqlStringTypeMapping("longtext", StoreTypePostfix.None, noBackslashEscapes: _options.NoBackslashEscapes, replaceLineBreaksWithCharFunction: _options.ReplaceLineBreaksWithCharFunction);

            _nchar = new MySqlStringTypeMapping("nchar", StoreTypePostfix.Size, fixedLength: true, noBackslashEscapes: _options.NoBackslashEscapes, replaceLineBreaksWithCharFunction: _options.ReplaceLineBreaksWithCharFunction);
            _nvarchar = new MySqlStringTypeMapping("nvarchar", StoreTypePostfix.Size, noBackslashEscapes: _options.NoBackslashEscapes, replaceLineBreaksWithCharFunction: _options.ReplaceLineBreaksWithCharFunction);

            _enum = new MySqlStringTypeMapping("enum", StoreTypePostfix.None, noBackslashEscapes: _options.NoBackslashEscapes, replaceLineBreaksWithCharFunction: _options.ReplaceLineBreaksWithCharFunction);

            _guid = MySqlGuidTypeMapping.IsValidGuidFormat(_options.ConnectionSettings.GuidFormat)
                ? new MySqlGuidTypeMapping(_options.ConnectionSettings.GuidFormat)
                : null;

            _jsonDefaultString = new MySqlJsonTypeMapping<string>("json", null, null, _options.NoBackslashEscapes, _options.ReplaceLineBreaksWithCharFunction);

            _storeTypeMappings
                = new Dictionary<string, RelationalTypeMapping[]>(StringComparer.OrdinalIgnoreCase)
                {
                    // bit
                    { "bit",                       new[] { _bit } },

                    // integers
                    { "tinyint",                   new[] { _tinyint } },
                    { "tinyint unsigned",          new[] { _utinyint } },
                    { "smallint",                  new[] { _smallint } },
                    { "smallint unsigned",         new[] { _usmallint } },
                    { "mediumint",                 new[] { _int } },
                    { "mediumint unsigned",        new[] { _uint } },
                    { "int",                       new[] { _int } },
                    { "int unsigned",              new[] { _uint } },
                    { "integer",                   new[] { _int } },
                    { "integer unsigned",          new[] { _uint } },
                    { "bigint",                    new[] { _bigint } },
                    { "bigint unsigned",           new[] { _ubigint } },

                    // decimals
                    { "decimal",                   new[] { _decimal } },
                    { "decimal unsigned",          new[] { _decimal } }, // deprecated since 8.0.17-mysql
                    { "numeric",                   new[] { _decimal } },
                    { "numeric unsigned",          new[] { _decimal } }, // deprecated since 8.0.17-mysql
                    { "dec",                       new[] { _decimal } },
                    { "dec unsigned",              new[] { _decimal } }, // deprecated since 8.0.17-mysql
                    { "fixed",                     new[] { _decimal } },
                    { "fixed unsigned",            new[] { _decimal } }, // deprecated since 8.0.17-mysql
                    { "double",                    new[] { _double } },
                    { "double unsigned",           new[] { _double } }, // deprecated since 8.0.17-mysql
                    { "double precision",          new[] { _double } },
                    { "double precision unsigned", new[] { _double } }, // deprecated since 8.0.17-mysql
                    { "real",                      new[] { _double } },
                    { "real unsigned",             new[] { _double } }, // deprecated since 8.0.17-mysql
                    { "float",                     new[] { _float } },
                    { "float unsigned",            new[] { _float } }, // deprecated since 8.0.17-mysql

                    // binary
                    { "binary",                    new[] { _binary } },
                    { "varbinary",                 new[] { _varbinary } },
                    { "tinyblob",                  new[] { _varbinary } },
                    { "blob",                      new[] { _varbinary } },
                    { "mediumblob",                new[] { _varbinary } },
                    { "longblob",                  new[] { _varbinary } },

                    // string
                    { "char",                      new[] { _charUnicode } },
                    { "varchar",                   new[] { _varcharUnicode } },
                    { "tinytext",                  new[] { _tinytextUnicode } },
                    { "text",                      new[] { _textUnicode } },
                    { "mediumtext",                new[] { _mediumtextUnicode } },
                    { "longtext",                  new[] { _longtextUnicode } },

                    { "enum",                      new[] { _enum } },

                    { "nchar",                     new[] { _nchar } },
                    { "nvarchar",                  new[] { _nvarchar } },

                    // DateTime
                    { "year",                      new[] { _year } },
                    { "date",                      new RelationalTypeMapping[] { _dateDateOnly, _dateDateTime } },
                    { "time",                      new RelationalTypeMapping[] { _timeTimeOnly, _timeTimeSpan } },
                    { "datetime",                  new RelationalTypeMapping[] { _dateTime, _dateTimeOffset } },
                    { "timestamp",                 new RelationalTypeMapping[] { _timeStamp, _timeStampOffset } },
                };

            _clrTypeMappings
                = new Dictionary<Type, RelationalTypeMapping>
                {
	                // integers
	                { typeof(short),    _smallint },
                    { typeof(ushort),   _usmallint },
                    { typeof(int),      _int },
                    { typeof(uint),     _uint },
                    { typeof(long),     _bigint },
                    { typeof(ulong),    _ubigint },

	                // decimals
	                { typeof(decimal),  _decimal },
                    { typeof(float),    _float },
                    { typeof(double),   _double },

	                // byte / char
	                { typeof(sbyte),    _tinyint },
                    { typeof(byte),     _utinyint },

                    // datetimes
                    { typeof(DateOnly), _dateDateOnly },
                    { typeof(TimeOnly), _timeTimeOnly.WithPrecisionAndScale(_options.DefaultDataTypeMappings.ClrTimeOnlyPrecision, null) },
                    { typeof(TimeSpan), _options.DefaultDataTypeMappings.ClrTimeSpan switch
                        {
                            MySqlTimeSpanType.Time6 => _timeTimeSpan.WithPrecisionAndScale(6, null),
                            MySqlTimeSpanType.Time => _timeTimeSpan,
                            _ => _timeTimeSpan
                        }},
                    { typeof(DateTime), _options.DefaultDataTypeMappings.ClrDateTime switch
                        {
                            MySqlDateTimeType.DateTime6 =>_dateTime.WithPrecisionAndScale(6, null),
                            MySqlDateTimeType.Timestamp6 => _timeStamp.WithPrecisionAndScale(6, null),
                            MySqlDateTimeType.Timestamp => _timeStamp,
                            _ => _dateTime,
                        }},
                    { typeof(DateTimeOffset), _options.DefaultDataTypeMappings.ClrDateTimeOffset switch
                        {
                            MySqlDateTimeType.DateTime6 =>_dateTimeOffset.WithPrecisionAndScale(6, null),
                            MySqlDateTimeType.Timestamp6 => _timeStampOffset.WithPrecisionAndScale(6, null),
                            MySqlDateTimeType.Timestamp => _timeStampOffset,
                            _ => _dateTimeOffset,
                        }},

                    // json
                    { typeof(MySqlJsonString), _jsonDefaultString }
                };

            // Boolean
            if (_options.DefaultDataTypeMappings.ClrBoolean != MySqlBooleanType.None)
            {
                var bit1AsBool = _options.DefaultDataTypeMappings.ClrBoolean == MySqlBooleanType.Bit1;

                _storeTypeMappings[bit1AsBool ? "bit(1)" : "tinyint(1)"] = new RelationalTypeMapping[] { bit1AsBool ? _bit1 : _tinyint1 };
                _clrTypeMappings[typeof(bool)] = bit1AsBool ? _bit1 : _tinyint1;
            }

            // Guid
            if (_guid != null)
            {
                _storeTypeMappings[_guid.StoreType] = new RelationalTypeMapping[]{ _guid };
                _clrTypeMappings[typeof(Guid)] = _guid;
            }

            // Type mappings that only exist to work around the limited code generation capabilites when scaffolding:
            _scaffoldingClrTypeMappings = new Dictionary<Type, RelationalTypeMapping>
            {
                { typeof(MySqlCodeGenerationMemberAccess), _codeGenerationMemberAccess },
                { typeof(MySqlCodeGenerationServerVersionCreation), _codeGenerationServerVersionCreation },
            };
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
                // First look for the fully qualified store type name.
                if (_storeTypeMappings.TryGetValue(storeTypeName, out var mappings))
                {
                    // We found the user-specified store type.
                    // If no CLR type was provided, we're probably scaffolding from an existing database. Take the first
                    // mapping as the default.
                    // If a CLR type was provided, look for a mapping between the store and CLR types. If none is found,
                    // fail immediately.
                    return clrType == null
                        ? mappings[0]
                        : mappings.FirstOrDefault(m => m.ClrType == clrType);
                }

                // Then look for the base store type name.
                if (_storeTypeMappings.TryGetValue(storeTypeNameBase, out mappings))
                {
                    return clrType == null
                        ? mappings[0]
                            .WithTypeMappingInfo(in mappingInfo)
                        : mappings.FirstOrDefault(m => m.ClrType == clrType)
                            ?.WithTypeMappingInfo(in mappingInfo);
                }

                if (storeTypeName.Equals("json", StringComparison.OrdinalIgnoreCase) &&
                    (clrType == null || clrType == typeof(string) || clrType == typeof(MySqlJsonString)))
                {
                    return _jsonDefaultString;
                }

                // A store type name was provided, but is unknown. This could be a domain (alias) type, in which case
                // we proceed with a CLR type lookup (if the type doesn't exist at all the failure will come later).
            }

            if (clrType != null)
            {
                if (_clrTypeMappings.TryGetValue(clrType, out var mapping))
                {
                    // If needed, clone the mapping with the configured length/precision/scale
                    if (mappingInfo.Precision.HasValue)
                    {
                        if (clrType == typeof(decimal))
                        {
                            return mapping.WithPrecisionAndScale(mappingInfo.Precision.Value, mappingInfo.Scale);
                        }

                        if (clrType == typeof(DateTime) ||
                            clrType == typeof(DateTimeOffset) ||
                            clrType == typeof(TimeSpan))
                        {
                            return mapping.WithPrecisionAndScale(mappingInfo.Precision.Value, null);
                        }
                    }

                    return mapping;
                }

                if (clrType == typeof(string))
                {
                    var isFixedLength = mappingInfo.IsFixedLength == true;

                    // Because we cannot check the annotations of the property mapping, we can't know whether `HasPrefixLength()` has been
                    // used or not. Therefore by default, the `LimitKeyedOrIndexedStringColumnLength` option will be true, and we will
                    // ensure, that the length of string properties will be set to a reasonable length, so that two columns limited this
                    // way could still fit.
                    // If users disable the `LimitKeyedOrIndexedStringColumnLength` option, they are responsible for oppropriately calling
                    // `HasPrefixLength()` for string properties, that are not mapped to a store type, where needed.
                    var size = mappingInfo.Size ??
                               (mappingInfo.IsKeyOrIndex &&
                                _options.LimitKeyedOrIndexedStringColumnLength
                                   // Allow to use at most half of the max key length, so at least 2 columns can fit
                                   ? Math.Min(_options.ServerVersion.MaxKeyLength / (_options.DefaultCharSet.MaxBytesPerChar * 2), 255)
                                   : (int?)null);

                    // If a string column size is bigger than it can/might be, we automatically adjust it to a variable one with an
                    // unlimited size.
                    if (size > 65_535 / _options.DefaultCharSet.MaxBytesPerChar)
                    {
                        size = null;
                        isFixedLength = false;
                    }
                    else if (size < 0) // specifying HasMaxLength(-1) is valid and should lead to an unbounded string/text.
                    {
                        size = null;
                    }

                    mapping = isFixedLength
                        ? _charUnicode
                        : size == null
                            ? _longtextUnicode
                            : _varcharUnicode;

                    return size == null
                        ? mapping
                        : mapping.WithStoreTypeAndSize($"{mapping.StoreTypeNameBase}({size})", size);
                }

                if (clrType == typeof(byte[]))
                {
                    if (mappingInfo.IsRowVersion == true)
                    {
                        return _options.ServerVersion.Supports.DateTime6
                            ? _binaryRowVersion6
                            : _binaryRowVersion;
                    }

                    var size = mappingInfo.Size ??
                               (mappingInfo.IsKeyOrIndex
                                   ? _options.ServerVersion.MaxKeyLength
                                   : (int?)null);

                    // Specifying HasMaxLength(-1) is valid and should lead to an unbounded byte array/blob.
                    if (size < 0)
                    {
                        size = null;
                    }

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

        protected override string ParseStoreTypeName(
            string storeTypeName,
            ref bool? unicode,
            ref int? size,
            ref int? precision,
            ref int? scale)
        {
            var storeTypeBaseName = base.ParseStoreTypeName(storeTypeName, ref unicode, ref size, ref precision, ref scale);

            if (storeTypeBaseName is not null)
            {
                // We are checking for a character set clause as part of the store type base name here, because it was common before 5.0
                // to specify charsets this way, because there were no character set specific annotations available yet.
                // Users might still use migrations generated with previous versions and just add newer migrations on top of those.
                var characterSetOccurrenceIndex = storeTypeBaseName.IndexOf("character set", StringComparison.OrdinalIgnoreCase);

                if (characterSetOccurrenceIndex < 0)
                {
                    characterSetOccurrenceIndex = storeTypeBaseName.IndexOf("charset", StringComparison.OrdinalIgnoreCase);
                }

                if (characterSetOccurrenceIndex >= 0)
                {
                    storeTypeBaseName = storeTypeBaseName[..characterSetOccurrenceIndex].TrimEnd();
                }

                if (storeTypeName.Contains("unsigned", StringComparison.OrdinalIgnoreCase))
                {
                    storeTypeBaseName += " unsigned";
                }
            }

            return storeTypeBaseName;
        }
    }
}
