// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Utilities;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Storage.Internal
{
    public class MySqlTypeMapper : RelationalTypeMapper
    {
        private static readonly Regex TypeRe = new Regex(@"([a-z0-9]+)\s*?(?:\(\s*(\d+)?\s*\))?\s*(unsigned)?", RegexOptions.IgnoreCase);

        // boolean
        private readonly RelationalTypeMapping _bit              = new RelationalTypeMapping("bit", typeof(bool), DbType.Boolean);

        // integers
        private readonly RelationalTypeMapping _tinyint          = new RelationalTypeMapping("tinyint", typeof(sbyte), DbType.SByte);
        private readonly RelationalTypeMapping _utinyint         = new RelationalTypeMapping("tinyint unsigned", typeof(byte), DbType.Byte);
	    private readonly RelationalTypeMapping _smallint         = new RelationalTypeMapping("smallint", typeof(short), DbType.Int16);
	    private readonly RelationalTypeMapping _usmallint        = new RelationalTypeMapping("smallint unsigned", typeof(ushort), DbType.UInt16);
        private readonly RelationalTypeMapping _int              = new RelationalTypeMapping("int", typeof(int), DbType.Int32);
	    private readonly RelationalTypeMapping _uint             = new RelationalTypeMapping("int unsigned", typeof(int), DbType.UInt32);
	    private readonly RelationalTypeMapping _bigint           = new RelationalTypeMapping("bigint", typeof(long), DbType.Int64);
	    private readonly RelationalTypeMapping _ubigint          = new RelationalTypeMapping("bigint unsigned", typeof(ulong), DbType.UInt64);

	    // decimals
	    private readonly RelationalTypeMapping _decimal          = new RelationalTypeMapping("decimal(65, 30)", typeof(decimal), DbType.Decimal);
	    private readonly RelationalTypeMapping _double           = new RelationalTypeMapping("double", typeof(double), DbType.Double);
        private readonly RelationalTypeMapping _float            = new RelationalTypeMapping("float", typeof(float));

	    // binary
	    private readonly RelationalTypeMapping _binary           = new RelationalTypeMapping("binary", typeof(byte[]), DbType.Binary);
        private readonly RelationalTypeMapping _varbinary        = new RelationalTypeMapping("varbinary", typeof(byte[]), DbType.Binary);
	    private readonly MySqlMaxLengthMapping _varbinary767     = new MySqlMaxLengthMapping("varbinary(767)", typeof(byte[]), DbType.Binary);
	    private readonly RelationalTypeMapping _varbinarymax     = new RelationalTypeMapping("longblob", typeof(byte[]), DbType.Binary);

	    // string
        private readonly MySqlMaxLengthMapping _char             = new MySqlMaxLengthMapping("char", typeof(string), DbType.AnsiStringFixedLength);
        private readonly MySqlMaxLengthMapping _varchar          = new MySqlMaxLengthMapping("varchar", typeof(string), DbType.AnsiString);
	    private readonly MySqlMaxLengthMapping _varchar127       = new MySqlMaxLengthMapping("varchar(127)", typeof(string), DbType.AnsiString);
	    private readonly RelationalTypeMapping _varcharmax       = new MySqlMaxLengthMapping("longtext", typeof(string), DbType.AnsiString);

	    // DateTime
        private readonly RelationalTypeMapping _dateTime6        = new RelationalTypeMapping("datetime(6)", typeof(DateTime), DbType.DateTime);
        private readonly RelationalTypeMapping _dateTimeOffset6  = new RelationalTypeMapping("datetime(6)", typeof(DateTimeOffset), DbType.DateTime);
        private readonly RelationalTypeMapping _time6            = new RelationalTypeMapping("time(6)", typeof(TimeSpan), DbType.Time);

        // json
        private readonly RelationalTypeMapping _json             = new RelationalTypeMapping("json", typeof(JsonObject<>), DbType.String);

	    // row version
        private readonly RelationalTypeMapping _rowversion       = new RelationalTypeMapping("TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP", typeof(byte[]), DbType.Binary);

        // guid
	    private readonly RelationalTypeMapping _uniqueidentifier = new RelationalTypeMapping("char(36)", typeof(Guid));

        readonly Dictionary<string, RelationalTypeMapping> _simpleNameMappings;
        readonly Dictionary<Type, RelationalTypeMapping> _simpleMappings;

        public MySqlTypeMapper()
        {
            _simpleNameMappings
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
                    { "double", _double },
                    { "float", _float },

                    // binary
                    { "binary", _binary },
                    { "varbinary", _varbinary },
                    { "tinyblob", _varbinarymax },
                    { "blob", _varbinarymax },
                    { "mediumblob", _varbinarymax },
                    { "longblob", _varbinarymax },

                    // string
                    { "char", _char },
                    { "varchar", _varchar },
                    { "tinytext", _varcharmax },
                    { "text", _varcharmax },
                    { "mediumtext", _varcharmax },
                    { "longtext", _varcharmax },

                    // DateTime
                    { "datetime", _dateTime6 },
                    { "time", _time6 },

                    // json
                    { "json", _json },

                    // guid
                    { "char(36)", _uniqueidentifier }
                };

            _simpleMappings
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
	                { typeof(byte), _utinyint },
	                { typeof(char), _utinyint },

	                // DateTime
	                { typeof(DateTime), _dateTime6 },
	                { typeof(DateTimeOffset), _dateTimeOffset6 },
	                { typeof(TimeSpan), _time6 },

	                // json
	                { typeof(JsonObject<>), _json },

	                // guid
	                { typeof(Guid), _uniqueidentifier }
                };

            ByteArrayMapper
                = new ByteArrayRelationalTypeMapper(
                    8000,
                    _varbinarymax,
                    _varbinary767,
                    _varbinary767,
                    _rowversion, size => new MySqlMaxLengthMapping(
                        "varbinary(" + size + ")",
                        typeof(byte[]),
                        DbType.Binary,
                        unicode: false,
                        size: size,
                        hasNonDefaultUnicode: false,
                        hasNonDefaultSize: true));

            StringMapper
                = new StringRelationalTypeMapper(
                    8000,
                    _varcharmax,
                    _varchar127,
                    _varchar127,
                    size => new MySqlMaxLengthMapping(
                        "varchar(" + size + ")",
                        typeof(string),
                        dbType: DbType.AnsiString,
                        unicode: false,
                        size: size,
                        hasNonDefaultUnicode: true,
                        hasNonDefaultSize: true),
                    8000,
                    _varcharmax,
                    _varchar127,
                    _varchar127,
                    size => new MySqlMaxLengthMapping(
                        "varchar(" + size + ")",
                        typeof(string),
                        dbType: null,
                        unicode: true,
                        size: size,
                        hasNonDefaultUnicode: false,
                        hasNonDefaultSize: true));
        }

        protected override IReadOnlyDictionary<string, RelationalTypeMapping> GetStoreTypeMappings()
        {
            return _simpleNameMappings;
        }

        protected override IReadOnlyDictionary<Type, RelationalTypeMapping> GetClrTypeMappings()
        {
            return _simpleMappings;
        }

        public override IByteArrayRelationalTypeMapper ByteArrayMapper { get; }

        public override IStringRelationalTypeMapper StringMapper { get; }

        protected override string GetColumnType(IProperty property) => property.MySql().ColumnType;

        protected override RelationalTypeMapping CreateMappingFromStoreType([NotNull] string storeType)
        {
            Check.NotNull(storeType, nameof(storeType));
            storeType = storeType.Trim().ToLower();

            var matchType = storeType;
            var matchLen = 0;
            var matchUnsigned = false;
            var match = TypeRe.Match(storeType);
            if (match.Success)
            {
                matchType = match.Groups[1].Value.ToLower();
                if (!string.IsNullOrWhiteSpace(match.Groups[2].Value))
                    int.TryParse(match.Groups[2].Value, out matchLen);
                if (!string.IsNullOrWhiteSpace(match.Groups[3].Value))
                    matchUnsigned = true;
            }

            var exactMatch = matchType + (matchLen > 0 ? $"({matchLen})" : "") + (matchUnsigned ? " unsigned" : "");
            RelationalTypeMapping mapping;
            if (GetStoreTypeMappings().TryGetValue(exactMatch, out mapping))
                return mapping;

            var noLengthMatch = matchType + (matchUnsigned ? " unsigned" : "");
            if (!GetStoreTypeMappings().TryGetValue(noLengthMatch, out mapping))
                return null;

            if (mapping.ClrType == typeof(string) || mapping.ClrType == typeof(byte[]))
            {
                return mapping.CreateCopy(exactMatch, matchLen);
            }
            return mapping.CreateCopy(exactMatch, mapping.Size);
        }

        public override RelationalTypeMapping FindMapping(Type clrType)
        {
            Check.NotNull(clrType, nameof(clrType));
            
            if (clrType.Name == typeof(JsonObject<>).Name)
                return _json;

            return clrType == typeof(string)
                ? _varcharmax
                : (clrType == typeof(byte[])
                    ? _varbinarymax
                    : base.FindMapping(clrType));
        }

        protected override RelationalTypeMapping FindCustomMapping([NotNull] IProperty property)
        {
            Check.NotNull(property, nameof(property));

            var clrType = property.ClrType.UnwrapEnumType();

            return clrType == typeof(string)
                ? StringMapper.FindMapping(true, property.IsKey() || property.IsIndex(), property.GetMaxLength())
                : clrType == typeof(byte[])
                    ? ByteArrayMapper.FindMapping(false, property.IsKey() || property.IsIndex(), property.GetMaxLength())
                    : null;
        }
    }
}
