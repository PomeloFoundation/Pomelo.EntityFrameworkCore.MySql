// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MySqlConnector;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Json.Newtonsoft.Storage.Internal
{
    public class MySqlJsonNewtonsoftTypeMapping<T> : MySqlJsonTypeMapping<T>
    {
        // Called via reflection.
        // ReSharper disable once UnusedMember.Global
        public MySqlJsonNewtonsoftTypeMapping(
            [NotNull] string storeType,
            [CanBeNull] ValueConverter valueConverter,
            [CanBeNull] ValueComparer valueComparer,
            [NotNull] IMySqlOptions options)
            : base(
                storeType,
                valueConverter,
                valueComparer,
                options)
        {
        }

        protected MySqlJsonNewtonsoftTypeMapping(
            RelationalTypeMappingParameters parameters,
            MySqlDbType mySqlDbType,
            IMySqlOptions options)
            : base(parameters, mySqlDbType, options)
        {
        }

        protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
            => new MySqlJsonNewtonsoftTypeMapping<T>(parameters, MySqlDbType, Options);

        public override Expression GenerateCodeLiteral(object value)
        {
            var defaultJsonLoadSettings = new Lazy<Expression>(() => Expression.New(typeof(JsonLoadSettings)));
            var parseMethod = new Lazy<MethodInfo>(() => typeof(JToken).GetMethod(nameof(JToken.Parse), new[] {typeof(string), typeof(JsonLoadSettings)}));

            return value switch
            {
                JToken jToken => Expression.Call(parseMethod.Value, Expression.Constant(jToken.ToString(Formatting.None)), defaultJsonLoadSettings.Value),
                string s => Expression.Constant(s),
                _ => throw new NotSupportedException("Cannot generate code literals for JSON POCOs.")
            };
        }
    }
}
