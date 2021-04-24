// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Data;
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
        private MySqlStringTypeMapping _tinytextUnicode;
        private MySqlStringTypeMapping _textUnicode;
        private MySqlStringTypeMapping _mediumtextUnicode;
        private MySqlStringTypeMapping _longtextUnicode;

        private MySqlStringTypeMapping _nchar;
        private MySqlStringTypeMapping _nvarchar;

        private MySqlStringTypeMapping _enum;

        // DateTime
        private readonly MySqlYearTypeMapping _year = new MySqlYearTypeMapping("year");
        private readonly MySqlDateTypeMapping _date = new MySqlDateTypeMapping("date");
        private readonly MySqlTimeSpanTypeMapping _time = new MySqlTimeSpanTypeMapping("time");
        private readonly MySqlDateTimeTypeMapping _dateTime = new MySqlDateTimeTypeMapping("datetime");
        private readonly MySqlDateTimeTypeMapping _timeStamp = new MySqlDateTimeTypeMapping("timestamp");
        private readonly MySqlDateTimeOffsetTypeMapping _dateTimeOffset = new MySqlDateTimeOffsetTypeMapping("datetime");
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
        private readonly MySqlCodeGenerationMemberAccessTypeMapping _codeGenerationMemberAccess = new MySqlCodeGenerationMemberAccessTypeMapping();
        private readonly MySqlCodeGenerationServerVersionCreationTypeMapping _codeGenerationServerVersionCreation = new MySqlCodeGenerationServerVersionCreationTypeMapping();

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

            _charUnicode = new MySqlStringTypeMapping("char", _options, StoreTypePostfix.Size, fixedLength: true);
            _varcharUnicode = new MySqlStringTypeMapping("varchar", _options, StoreTypePostfix.Size);
            _tinytextUnicode = new MySqlStringTypeMapping("tinytext", _options, StoreTypePostfix.None);
            _textUnicode = new MySqlStringTypeMapping("text", _options, StoreTypePostfix.None);
            _mediumtextUnicode = new MySqlStringTypeMapping("mediumtext", _options, StoreTypePostfix.None);
            _longtextUnicode = new MySqlStringTypeMapping("longtext", _options, StoreTypePostfix.None);

            _nchar = new MySqlStringTypeMapping("nchar", _options, StoreTypePostfix.Size, fixedLength: true);
            _nvarchar = new MySqlStringTypeMapping("nvarchar", _options, StoreTypePostfix.Size);

            _enum = new MySqlStringTypeMapping("enum", _options, StoreTypePostfix.None);

            _guid = MySqlGuidTypeMapping.IsValidGuidFormat(_options.ConnectionSettings.GuidFormat)
                ? new MySqlGuidTypeMapping(_options.ConnectionSettings.GuidFormat)
                : null;

            _jsonDefaultString = new MySqlJsonTypeMapping<string>("json", null, null, _options);

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
                    { "date",                      new[] { _date } },
                    { "time",                      new[] { _time } },
                    { "datetime",                  new RelationalTypeMapping[] { _dateTime, _dateTimeOffset } },
                    { "timestamp",                 new RelationalTypeMapping[] { _timeStamp, _timeStampOffset } },
                };

            _clrTypeMappings
                = new Dictionary<Type, RelationalTypeMapping>
                {
	                // integers
	                { typeof(short),   _smallint },
                    { typeof(ushort),  _usmallint },
                    { typeof(int),     _int },
                    { typeof(uint),    _uint },
                    { typeof(long),    _bigint },
                    { typeof(ulong),   _ubigint },

	                // decimals
	                { typeof(decimal), _decimal },
                    { typeof(float),   _float },
                    { typeof(double),  _double },

	                // byte / char
	                { typeof(sbyte),   _tinyint },
                    { typeof(byte),    _utinyint },

                    // datetimes
                    { typeof(TimeSpan), _options.DefaultDataTypeMappings.ClrTimeSpan == MySqlTimeSpanType.Time6
                        ? _time.Clone(6, null)
                        : _time },
                    { typeof(DateTime), _options.DefaultDataTypeMappings.ClrDateTime switch
                        {
                            MySqlDateTimeType.DateTime6 =>_dateTime.Clone(6, null),
                            MySqlDateTimeType.Timestamp6 => _timeStamp.Clone(6, null),
                            MySqlDateTimeType.Timestamp => _timeStamp,
                            _ => _dateTime,
                        }},
                    { typeof(DateTimeOffset), _options.DefaultDataTypeMappings.ClrDateTimeOffset switch
                        {
                            MySqlDateTimeType.DateTime6 =>_dateTimeOffset.Clone(6, null),
                            MySqlDateTimeType.Timestamp6 => _timeStampOffset.Clone(6, null),
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
                            .Clone(in mappingInfo)
                        : mappings.FirstOrDefault(m => m.ClrType == clrType)
                            ?.Clone(in mappingInfo);
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
                            return mapping.Clone(mappingInfo.Precision.Value, mappingInfo.Scale);
                        }

                        if (clrType == typeof(DateTime) ||
                            clrType == typeof(DateTimeOffset) ||
                            clrType == typeof(TimeSpan))
                        {
                            return mapping.Clone(mappingInfo.Precision.Value, null);
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
                    if (size > 65_553 / _options.DefaultCharSet.MaxBytesPerChar)
                    {
                        size = null;
                        isFixedLength = false;
                    }

                    mapping = isFixedLength
                        ? _charUnicode
                        : size == null
                            ? _longtextUnicode
                            : _varcharUnicode;

                    return size == null
                        ? mapping
                        : mapping.Clone($"{mapping.StoreTypeNameBase}({size})", size);
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

            return (storeTypeName?.IndexOf("unsigned", StringComparison.OrdinalIgnoreCase) ?? -1) >= 0
                ? storeTypeBaseName + " unsigned"
                : storeTypeBaseName;
        }
    }
}
