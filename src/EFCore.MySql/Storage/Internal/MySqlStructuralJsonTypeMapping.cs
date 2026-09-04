// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Data.Common;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Microsoft.EntityFrameworkCore.Storage;
using MySqlConnector;

namespace Pomelo.EntityFrameworkCore.MySql.Storage.Internal
{
    /// <summary>
    ///     Type mapping for structural JSON columns (complex types mapped with <c>.ToJson()</c>).
    ///     MySQL stores JSON as text internally, so we read via <see cref="DbDataReader.GetString" />
    ///     and convert to <see cref="MemoryStream" /> as expected by EF Core's JSON pipeline.
    ///     This is analogous to <c>SqlServerStructuralJsonTypeMapping</c>.
    /// </summary>
    public class MySqlStructuralJsonTypeMapping : JsonTypeMapping
    {
        private static readonly MethodInfo _getStringMethod
            = typeof(DbDataReader).GetRuntimeMethod(nameof(DbDataReader.GetString), new[] { typeof(int) });

        private static readonly PropertyInfo _utf8Property
            = typeof(Encoding).GetProperty(nameof(Encoding.UTF8));

        private static readonly MethodInfo _getBytesMethod
            = typeof(Encoding).GetMethod(nameof(Encoding.GetBytes), new[] { typeof(string) });

        private static readonly ConstructorInfo _memoryStreamConstructor
            = typeof(MemoryStream).GetConstructor(new[] { typeof(byte[]) });

        public static MySqlStructuralJsonTypeMapping Default { get; } = new("json");

        public MySqlStructuralJsonTypeMapping(string storeType)
            : base(storeType, typeof(JsonTypePlaceholder), dbType: null)
        {
        }

        protected MySqlStructuralJsonTypeMapping(RelationalTypeMappingParameters parameters)
            : base(parameters)
        {
        }

        /// <summary>
        ///     MySQL stores JSON as strings, so we read using <see cref="DbDataReader.GetString" />.
        /// </summary>
        public override MethodInfo GetDataReaderMethod()
            => _getStringMethod;

        /// <summary>
        ///     Converts the string read from the data reader into a <see cref="MemoryStream" />
        ///     that EF Core's JSON reader/writer pipeline can consume.
        ///     Creates: <c>new MemoryStream(Encoding.UTF8.GetBytes(stringValue))</c>.
        /// </summary>
        public override Expression CustomizeDataReaderExpression(Expression expression)
            => Expression.New(
                _memoryStreamConstructor,
                Expression.Call(
                    Expression.Property(null, _utf8Property),
                    _getBytesMethod,
                    expression));

        protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
            => new MySqlStructuralJsonTypeMapping(parameters);

        public override Type ClrType => typeof(JsonTypePlaceholder);

        protected virtual string EscapeSqlLiteral(string literal)
            => literal.Replace("'", "''");

        protected override string GenerateNonNullSqlLiteral(object value)
            => $"'{EscapeSqlLiteral((string)value)}'";

        protected override void ConfigureParameter(DbParameter parameter)
        {
            if (parameter is MySqlParameter mySqlParameter)
            {
                mySqlParameter.MySqlDbType = MySqlDbType.JSON;
            }

            base.ConfigureParameter(parameter);
        }
    }
}
