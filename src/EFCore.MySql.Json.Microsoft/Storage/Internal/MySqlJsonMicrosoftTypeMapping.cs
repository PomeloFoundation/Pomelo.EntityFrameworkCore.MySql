using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MySql.Data.MySqlClient;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Json.Microsoft.Storage.Internal
{
    public class MySqlJsonMicrosoftTypeMapping<T> : MySqlJsonTypeMapping<T>
    {
        // Called via reflection.
        // ReSharper disable once UnusedMember.Global
        public MySqlJsonMicrosoftTypeMapping(
            [NotNull] string storeType,
            [CanBeNull] ValueConverter valueConverter,
            [NotNull] IMySqlOptions options)
            : base(
                storeType,
                valueConverter,
                options)
        {
        }

        protected MySqlJsonMicrosoftTypeMapping(
            RelationalTypeMappingParameters parameters,
            MySqlDbType mySqlDbType,
            IMySqlOptions options)
            : base(parameters, mySqlDbType, options)
        {
        }

        protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
            => new MySqlJsonMicrosoftTypeMapping<T>(parameters, MySqlDbType, Options);

        public override Expression GenerateCodeLiteral(object value)
        {
            var defaultJsonDocumentOptions = new Lazy<Expression>(() => Expression.New(typeof(JsonDocumentOptions)));
            var parseMethod = new Lazy<MethodInfo>(() => typeof(JsonDocument).GetMethod(nameof(JsonDocument.Parse), new[] {typeof(string), typeof(JsonDocumentOptions)}));

            return value switch
            {
                JsonDocument document => Expression.Call(parseMethod.Value, Expression.Constant(document.RootElement.ToString()), defaultJsonDocumentOptions.Value),
                JsonElement element => Expression.Property(
                    Expression.Call(parseMethod.Value, Expression.Constant(element.ToString()), defaultJsonDocumentOptions.Value),
                    nameof(JsonDocument.RootElement)),
                string s => Expression.Constant(s),
                _ => throw new NotSupportedException("Cannot generate code literals for JSON POCOs.")
            };
        }
    }
}
