// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Data;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Utilities;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Storage.Internal
{
    public class MySqlTypeMapper : RelationalTypeMapper
    {
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
	    private readonly MySqlMaxLengthMapping _char             = new MySqlMaxLengthMapping("char", typeof(char), DbType.AnsiStringFixedLength);
	    private readonly RelationalTypeMapping _varbinary        = new RelationalTypeMapping("varbinary", typeof(byte[]), DbType.Binary);
	    private readonly MySqlMaxLengthMapping _varbinary767     = new MySqlMaxLengthMapping("varbinary(767)", typeof(byte[]), DbType.Binary);
	    private readonly RelationalTypeMapping _varbinarymax     = new RelationalTypeMapping("longblob", typeof(byte[]), DbType.Binary);

	    // string
	    private readonly MySqlMaxLengthMapping _varchar          = new MySqlMaxLengthMapping("varchar", typeof(string), DbType.AnsiString);
	    private readonly MySqlMaxLengthMapping _varchar255       = new MySqlMaxLengthMapping("varchar(255)", typeof(string), DbType.AnsiString);
	    private readonly MySqlMaxLengthMapping _nchar            = new MySqlMaxLengthMapping("varchar", typeof(string), DbType.StringFixedLength);
	    private readonly MySqlMaxLengthMapping _nvarchar         = new MySqlMaxLengthMapping("varchar", typeof(string));
	    private readonly RelationalTypeMapping _varcharmax       = new MySqlMaxLengthMapping("longtext", typeof(string), DbType.AnsiString);

	    // DateTime
	    private readonly RelationalTypeMapping _datetime         = new RelationalTypeMapping("datetime(6)", typeof(DateTime), DbType.DateTime);
	    private readonly RelationalTypeMapping _datetimeoffset   = new RelationalTypeMapping("datetime(6)", typeof(DateTimeOffset), DbType.DateTime);
	    private readonly RelationalTypeMapping _time             = new RelationalTypeMapping("time(6)", typeof(TimeSpan), DbType.Time);

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
                    { "bigint", _bigint },
                    //{ "binary varying", _varbinary },
                    { "binary", _varbinary },
                    { "bit", _bit },
                    { "char varying", _varchar },
                    { "char varying(8000)", _varcharmax },
                    { "char", _char },
                    { "character varying", _varchar },
                    { "character varying(8000)", _varcharmax },
                    { "character", _char },
                    { "date", _datetime },
                    { "datetime", _datetime },
	                { "dec", _decimal },
                    { "decimal", _decimal },
                    { "double", _double },
                    { "double unsigned", _decimal },
                    { "float", _float },
                    { "image", _varbinary },
                    { "int", _int },
                    { "longblob", _varbinarymax },
                    { "longtext", _varcharmax },
                    { "mediumblob", _varbinarymax },
                    { "mediumint", _int },
                    { "mediumtext", _varcharmax },
                    { "money", _decimal },
                    { "nchar", _nchar },
                    { "ntext", _nvarchar },
                    { "numeric", _decimal },
                    { "nvarchar", _nvarchar },
                    { "smallint", _smallint },
                    { "smallmoney", _decimal },
                    { "text", _varchar },
                    { "time", _time },
                    { "timestamp", _datetime },
                    { "tinyint", _tinyint },
                    { "uniqueidentifier", _uniqueidentifier },
                    { "varbinary", _varbinary },
                    { "varchar", _varchar },
                    { "varchar(8000)", _varcharmax },
                    { "json", _json }
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
	                { typeof(DateTime), _datetime },
	                { typeof(DateTimeOffset), _datetimeoffset },
	                { typeof(TimeSpan), _time },

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
                    _varchar255,
                    _varchar255,
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
                    _varchar255,
                    _varchar255,
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
