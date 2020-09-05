﻿using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MySql.Data.MySqlClient;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Storage.Internal
{
    public class MySqlJsonTypeMapping<T> : MySqlJsonTypeMapping
    {
        public MySqlJsonTypeMapping(
            [NotNull] string storeType,
            [CanBeNull] ValueConverter valueConverter,
            [NotNull] IMySqlOptions options)
            : base(
                storeType,
                typeof(T),
                valueConverter,
                options)
        {
        }

        protected MySqlJsonTypeMapping(
            RelationalTypeMappingParameters parameters,
            MySqlDbType mySqlDbType,
            IMySqlOptions options)
            : base(parameters, mySqlDbType, options)
        {
        }

        protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
            => new MySqlJsonTypeMapping<T>(parameters, MySqlDbType, Options);
    }

    public abstract class MySqlJsonTypeMapping : MySqlTypeMapping
    {
        [NotNull]
        protected IMySqlOptions Options { get; }

        public MySqlJsonTypeMapping(
            [NotNull] string storeType,
            [NotNull] Type clrType,
            [CanBeNull] ValueConverter valueConverter,
            [NotNull] IMySqlOptions options)
            : base(
                storeType,
                clrType,
                MySqlDbType.JSON,
                valueConverter: valueConverter)
        {
            if (storeType != "json")
            {
                throw new ArgumentException($"The store type '{nameof(storeType)}' must be 'json'.", nameof(storeType));
            }

            Options = options;
        }

        protected MySqlJsonTypeMapping(
            RelationalTypeMappingParameters parameters,
            MySqlDbType mySqlDbType,
            IMySqlOptions options)
            : base(parameters, mySqlDbType)
        {
            Options = options;
        }

        protected override string GenerateNonNullSqlLiteral(object value)
            => MySqlStringTypeMapping.EscapeSqlLiteralWithLineBreaks((string)value, Options);
    }
}
