using System;
using System.Data.Common;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MySql.Data.MySqlClient;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Pomelo.EntityFrameworkCore.MySql.Storage.ValueConversion.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Storage.Internal
{
    public class MySqlJsonTypeMapping<T> : MySqlJsonTypeMapping
    {
        public MySqlJsonTypeMapping(
            [NotNull] string storeType,
            [NotNull] IMySqlOptions options)
            : base(
                storeType,
                typeof(T),
                options,
                typeof(T) == typeof(JsonDocument)
                    ? new JsonDocumentValueConverter()
                    : typeof(T) == typeof(JsonElement)
                        ? new JsonElementValueConverter()
                        : typeof(T) == typeof(string)
                            ? new JsonStringValueConverter()
                            : (ValueConverter)new JsonPocoValueConverter<T>())
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
            [NotNull] IMySqlOptions options,
            [CanBeNull] ValueConverter valueConverter)
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

        protected virtual string EscapeSqlLiteral([NotNull] string literal)
            => MySqlStringTypeMapping.EscapeSqlLiteralWithLineBreaks(literal, Options);

        protected override string GenerateNonNullSqlLiteral(object value)
            => value switch
            {
                string s => EscapeSqlLiteral(s),
                _ => EscapeSqlLiteral(JsonSerializer.Serialize(value))
            };

        public override Expression GenerateCodeLiteral(object value)
            => value switch
            {
                // TODO: Move to MySqlJsonSerializer.
                JsonDocument document => Expression.Call(_parseMethod, Expression.Constant(document.RootElement.ToString()), _defaultJsonDocumentOptions),
                JsonElement element => Expression.Property(
                    Expression.Call(_parseMethod, Expression.Constant(element.ToString()), _defaultJsonDocumentOptions),
                    nameof(JsonDocument.RootElement)),
                string s => Expression.Constant(s),
                _ => throw new NotSupportedException("Cannot generate code literals for JSON POCOs")
            };

        protected override void ConfigureParameter(DbParameter parameter)
        {
            base.ConfigureParameter(parameter);

            // MariaDB does not really have a JSON type. It is just a LONGTEXT alias. Therefore, MariaDB does not
            // process/compact JSON documents/values on its own by default.
            // if (Options.ServerVersion.SupportsJsonDataTypeEmulation &&
            //     parameter.Value is string stringValue)
            // {
            //     var valueConverter = new JsonDocumentValueConverter();
            //     parameter.Value = valueConverter.ConvertToProvider(valueConverter.ConvertFromProvider(stringValue));
            // }
        }

        private static readonly Expression _defaultJsonDocumentOptions = Expression.New(typeof(JsonDocumentOptions));
        private static readonly MethodInfo _parseMethod = typeof(JsonDocument).GetMethod(nameof(JsonDocument.Parse), new[] {typeof(string), typeof(JsonDocumentOptions)});
    }
}
