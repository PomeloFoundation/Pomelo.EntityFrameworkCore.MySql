// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Utilities;
using Pomelo.Data.MySql;


// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Storage.Internal
{
    public class MySqlTypeMapper : RelationalTypeMapper
    {
        private readonly RelationalTypeMapping _bigint = new RelationalTypeMapping("bigint", typeof(long), DbType.Int64);
        private readonly RelationalTypeMapping _bit = new RelationalTypeMapping("bit", typeof(bool));
        private readonly MySqlMaxLengthMapping _char = new MySqlMaxLengthMapping("char", typeof(char), DbType.AnsiStringFixedLength);
        private readonly RelationalTypeMapping _datetime = new RelationalTypeMapping("datetime", typeof(DateTime), DbType.DateTime);
        private readonly RelationalTypeMapping _datetimeoffset = new RelationalTypeMapping("varchar(255)", typeof(DateTimeOffset), DbType.DateTimeOffset);
        private readonly RelationalTypeMapping _decimal = new RelationalTypeMapping("decimal(18, 2)", typeof(decimal));
        private readonly RelationalTypeMapping _double = new RelationalTypeMapping("double", typeof(double));
        private readonly RelationalTypeMapping _float = new RelationalTypeMapping("float", typeof(float));
        private readonly RelationalTypeMapping _int = new RelationalTypeMapping("int", typeof(int), DbType.Int32);
        private readonly RelationalTypeMapping _smallint = new RelationalTypeMapping("smallint", typeof(short), DbType.Int16);
        private readonly RelationalTypeMapping _tinyint = new RelationalTypeMapping("tinyint unsigned", typeof(byte), DbType.Byte);


        private readonly RelationalTypeMapping _rowversion = new RelationalTypeMapping("TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP", typeof(byte[]), DbType.Binary);
        private readonly MySqlMaxLengthMapping _nchar = new MySqlMaxLengthMapping("nchar", typeof(string), DbType.StringFixedLength);
        private readonly MySqlMaxLengthMapping _nvarchar = new MySqlMaxLengthMapping("nvarchar", typeof(string));
        private readonly RelationalTypeMapping _varcharmax = new MySqlMaxLengthMapping("longtext", typeof(string), DbType.AnsiString);

        private readonly MySqlMaxLengthMapping _varchar = new MySqlMaxLengthMapping("varchar", typeof(string), DbType.AnsiString);
        private readonly MySqlMaxLengthMapping _varchar255 = new MySqlMaxLengthMapping("varchar(255)", typeof(string), DbType.AnsiString);
        private readonly RelationalTypeMapping _varbinary = new RelationalTypeMapping("varbinary", typeof(byte[]), DbType.Binary);
        private readonly MySqlMaxLengthMapping _varbinary255 = new MySqlMaxLengthMapping("varbinary(255)", typeof(byte[]), DbType.Binary);
        private readonly RelationalTypeMapping _varbinarymax = new RelationalTypeMapping("longblob", typeof(byte[]), DbType.Binary);


        private readonly RelationalTypeMapping _uniqueidentifier = new RelationalTypeMapping("char(38)", typeof(Guid));
        private readonly RelationalTypeMapping _time = new RelationalTypeMapping("time(6)", typeof(TimeSpan), DbType.Time);

        private readonly ConcurrentDictionary<int, RelationalTypeMapping> _boundedStringMappings = new ConcurrentDictionary<int, RelationalTypeMapping>();
        private readonly ConcurrentDictionary<int, RelationalTypeMapping> _boundedBinaryMappings = new ConcurrentDictionary<int, RelationalTypeMapping>();


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
                    { "timestamp", _datetimeoffset },
                    { "dec", _decimal },
                    { "decimal", _decimal },
                    { "double", _double },
                    { "float", _float },
                    { "image", _varbinary },
                    { "int", _int },
                    { "money", _decimal },
                    { "nchar", _nchar },
                    { "ntext", _nvarchar },
                    { "numeric", _decimal },
                    { "nvarchar", _nvarchar },
                    { "smallint", _smallint },
                    { "smallmoney", _decimal },
                    { "text", _varchar },
                    { "time", _time },
                    { "tinyint", _tinyint },
                    { "uniqueidentifier", _uniqueidentifier },
                    { "varbinary", _varbinary },
                    { "varchar", _varchar },
                    { "varchar(8000)", _varcharmax }
                };

            _simpleMappings
                = new Dictionary<Type, RelationalTypeMapping>
                {
                    { typeof(int), _int },
                    { typeof(long), _bigint },
                    { typeof(DateTime), _datetime },
                    { typeof(DateTimeOffset), _datetimeoffset },
                    { typeof(Guid), _uniqueidentifier },
                    { typeof(bool), _bit },
                    { typeof(byte), _tinyint },
                    { typeof(double), _double },
                    { typeof(char), _int },
                    { typeof(sbyte), new RelationalTypeMapping("tinyint", typeof(sbyte)) },
                    { typeof(ushort), new RelationalTypeMapping("int", typeof(ushort)) },
                    { typeof(uint), new RelationalTypeMapping("bigint", typeof(uint)) },
                    { typeof(ulong), new RelationalTypeMapping("real(20, 0)", typeof(ulong)) },
                    { typeof(short), _smallint },
                    { typeof(float), _float },
                    { typeof(decimal), _decimal },
                    { typeof(TimeSpan), _time }
                };

            ByteArrayMapper
                = new ByteArrayRelationalTypeMapper(
                    8000,
                    _varbinarymax,
                    _varbinarymax,
                    _varbinary255,
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
                    767,
                    _varcharmax,
                    _varcharmax,
                    _varchar255,
                    size => new MySqlMaxLengthMapping(
                        "varchar(" + size + ")",
                        typeof(string),
                        dbType: DbType.AnsiString,
                        unicode: false,
                        size: size,
                        hasNonDefaultUnicode: true,
                        hasNonDefaultSize: true),
                    767,
                    _varcharmax,
                    _varcharmax,
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
