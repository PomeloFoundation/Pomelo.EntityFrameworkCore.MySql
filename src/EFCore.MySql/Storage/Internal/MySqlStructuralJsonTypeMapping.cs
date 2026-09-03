// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Data;
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
    /// The type mapping used by EF Core 10 when a structural type (complex type or owned entity) is mapped to a JSON column
    /// (e.g. via <c>ToJson()</c>). MySQL and MariaDB store such data in a native <c>json</c> column.
    /// </summary>
    public class MySqlStructuralJsonTypeMapping : JsonTypeMapping
    {
        private static readonly MethodInfo CreateUtf8StreamMethod
            = typeof(MySqlStructuralJsonTypeMapping).GetMethod(nameof(CreateUtf8Stream), [typeof(string)])!;

        private static readonly MethodInfo GetStringMethod
            = typeof(DbDataReader).GetRuntimeMethod(nameof(DbDataReader.GetString), [typeof(int)])!;

        public static MySqlStructuralJsonTypeMapping Default
            => JsonTypeDefault;

        public static MySqlStructuralJsonTypeMapping JsonTypeDefault { get; } = new("json");

        public MySqlStructuralJsonTypeMapping(string storeType)
            : base(storeType, typeof(JsonTypePlaceholder), System.Data.DbType.String)
        {
        }

        protected MySqlStructuralJsonTypeMapping(RelationalTypeMappingParameters parameters)
            : base(parameters)
        {
        }

        public override MethodInfo GetDataReaderMethod()
            => GetStringMethod;

        public static MemoryStream CreateUtf8Stream(string json)
            => json == ""
                ? throw new InvalidOperationException("Cannot read a JSON value from an empty string.")
                : new MemoryStream(Encoding.UTF8.GetBytes(json));

        public override Expression CustomizeDataReaderExpression(Expression expression)
            => Expression.Call(CreateUtf8StreamMethod, expression);

        protected virtual string EscapeSqlLiteral(string literal)
            => literal.Replace("'", "''");

        protected override string GenerateNonNullSqlLiteral(object value)
            // MySQL and MariaDB store structural types in a native `json` column. Emitting a JSON-typed literal
            // (instead of a plain quoted string) ensures the value is treated as JSON by comparisons and JSON
            // functions, avoiding implicit string-to-JSON conversion mismatches.
            => $"CAST('{EscapeSqlLiteral((string)value)}' AS json)";

        protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
            => new MySqlStructuralJsonTypeMapping(parameters);

        protected override void ConfigureParameter(DbParameter parameter)
        {
            base.ConfigureParameter(parameter);

            if (parameter is MySqlParameter mySqlParameter)
            {
                mySqlParameter.MySqlDbType = MySqlDbType.JSON;
            }
        }
    }
}
