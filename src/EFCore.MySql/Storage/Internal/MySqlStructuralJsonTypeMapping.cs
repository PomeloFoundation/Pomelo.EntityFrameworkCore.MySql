// Copyright (c) Microting. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Data.Common;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Microsoft.EntityFrameworkCore.Storage;
using MySqlConnector;

namespace Microting.EntityFrameworkCore.MySql.Storage.Internal
{
    /// <summary>
    /// Type mapping for complex JSON types (types mapped with .ToJson()).
    /// This mapping handles the conversion from MySQL's string-based JSON to MemoryStream expected by EF Core.
    /// Similar to Npgsql's NpgsqlStructuralJsonTypeMapping.
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
        /// MySQL stores JSON as strings, so we read using GetString.
        /// </summary>
        public override MethodInfo GetDataReaderMethod()
        {
            return _getStringMethod;
        }

        /// <summary>
        /// Customizes the data reader expression to convert string to MemoryStream.
        /// Creates: new MemoryStream(Encoding.UTF8.GetBytes(stringValue))
        /// </summary>
        public override Expression CustomizeDataReaderExpression(Expression expression)
        {
            return Expression.New(
                _memoryStreamConstructor,
                Expression.Call(
                    Expression.Property(null, _utf8Property),
                    _getBytesMethod,
                    expression));
        }

        protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
            => new MySqlStructuralJsonTypeMapping(parameters);

        /// <summary>
        /// Override to ensure we return JsonTypePlaceholder type consistently
        /// </summary>
        public override Type ClrType => typeof(JsonTypePlaceholder);

        protected virtual string EscapeSqlLiteral(string literal)
            => literal.Replace("'", "''");

        protected override string GenerateNonNullSqlLiteral(object value)
            => $"'{EscapeSqlLiteral((string)value)}'";

        protected override void ConfigureParameter(DbParameter parameter)
        {
            // MySQL uses JSON db type for JSON columns
            if (parameter is MySqlParameter mySqlParameter)
            {
                mySqlParameter.MySqlDbType = MySqlDbType.JSON;
            }

            base.ConfigureParameter(parameter);
        }
    }
}
