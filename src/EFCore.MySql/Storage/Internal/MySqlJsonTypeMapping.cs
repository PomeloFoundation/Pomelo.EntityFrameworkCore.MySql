// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Pomelo.EntityFrameworkCore.MySql.Storage.Internal
{
    /// <summary>
    ///     <para>
    ///         Represents the mapping between a .NET <see cref="JsonObject" /> type and a database type.
    ///     </para>
    ///     <para>
    ///         This type is typically used by database providers (and other extensions). It is generally
    ///         not used in application code.
    ///     </para>
    /// </summary>
    public class MySqlJsonTypeMapping : RelationalTypeMapping
    {
        private static readonly ConcurrentDictionary<Type, ValueConverter> JsonConverters = new ConcurrentDictionary<Type, ValueConverter>();
        private static readonly ConcurrentDictionary<Type, ValueComparer> JsonComparers = new ConcurrentDictionary<Type, ValueComparer>();

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public MySqlJsonTypeMapping(
            Type clrType,
            string storeType = null,
            bool? unicode = null)
            : this(
                new RelationalTypeMappingParameters(
                    new CoreTypeMappingParameters(clrType, GetConverter(clrType), GetComprarer(clrType)),
                    storeType ?? "json",
                    StoreTypePostfix.None,
                    System.Data.DbType.String,
                    unicode ?? false))
        {
        }

        private static ValueConverter GetConverter(Type jsonType)
            => JsonConverters.GetOrAdd(jsonType, t =>
                (ValueConverter)typeof(JsonToStringConverter<>)
                    .MakeGenericType(t.TryGetElementType(typeof(JsonObject<>)))
                    .GetDeclaredConstructor(new Type[0]).Invoke(new object[0]));

        private static ValueComparer GetComprarer(Type jsonType)
            => JsonComparers.GetOrAdd(jsonType, t =>
                (ValueComparer)typeof(JsonComparer<>)
                    .MakeGenericType(t.TryGetElementType(typeof(JsonObject<>)))
                    .GetDeclaredConstructor(new Type[0]).Invoke(new object[0]));

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected MySqlJsonTypeMapping(RelationalTypeMappingParameters parameters)
            : base(parameters)
        {
        }

        /// <summary>
        ///     Creates a copy of this mapping.
        /// </summary>
        /// <param name="parameters"> The parameters for this mapping. </param>
        /// <returns> The newly created mapping. </returns>
        protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
            => new MySqlJsonTypeMapping(parameters);

        /// <summary>
        ///     Generates the escaped SQL representation of a literal value.
        /// </summary>
        /// <param name="literal">The value to be escaped.</param>
        /// <returns>
        ///     The generated string.
        /// </returns>
        protected virtual string EscapeSqlLiteral([NotNull]string literal)
            => Check.NotNull(literal, nameof(literal)).Replace("'", "''");

        /// <summary>
        ///     Generates the SQL representation of a literal value.
        /// </summary>
        /// <param name="value">The literal value.</param>
        /// <returns>
        ///     The generated string.
        /// </returns>
        protected override string GenerateNonNullSqlLiteral(object value)
            => $"'{EscapeSqlLiteral((string)value)}'";

        private class JsonToStringConverter<T> : ValueConverter<JsonObject<T>, string>
            where T : class
        {
            public JsonToStringConverter()
                : base(
                    v => v.ToString(),
                    v => new JsonObject<T>(v))
            {
            }
        }

        private class JsonComparer<T> : ValueComparer<JsonObject<T>>
            where T : class
        {
            public JsonComparer()
                : base((l, r) => object.Equals(l, r), v => v.GetHashCode())
            {
            }
        }
    }
}
