// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Data.Common;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MySqlConnector;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Storage.Internal
{
    public class MySqlJsonTypeMapping<T> : MySqlJsonTypeMapping
    {
        public MySqlJsonTypeMapping(
            [NotNull] string storeType,
            [CanBeNull] ValueConverter valueConverter,
            [CanBeNull] ValueComparer valueComparer,
            [NotNull] IMySqlOptions options)
            : base(
                storeType,
                typeof(T),
                valueConverter,
                valueComparer,
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

    public abstract class MySqlJsonTypeMapping : MySqlStringTypeMapping
    {
        [NotNull]
        protected virtual IMySqlOptions Options { get; }

        public MySqlJsonTypeMapping(
            [NotNull] string storeType,
            [NotNull] Type clrType,
            [CanBeNull] ValueConverter valueConverter,
            [CanBeNull] ValueComparer valueComparer,
            [NotNull] IMySqlOptions options)
            : base(
                new RelationalTypeMappingParameters(
                    new CoreTypeMappingParameters(
                        clrType,
                        valueConverter,
                        valueComparer),
                    storeType,
                    unicode: true),
                MySqlDbType.JSON,
                options,
                false,
                false)
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
            : base(parameters, mySqlDbType, options, false, false)
        {
            Options = options;
        }

        protected override void ConfigureParameter(DbParameter parameter)
        {
            base.ConfigureParameter(parameter);

            // MySqlConnector does not know how to handle our custom MySqlJsonString type, that could be used when a
            // string parameter is explicitly cast to it.
            if (parameter.Value is MySqlJsonString mySqlJsonString)
            {
                parameter.Value = (string)mySqlJsonString;
            }
        }
    }
}
