// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Text.Json;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Pomelo.EntityFrameworkCore.MySql.Json.Microsoft.Storage.ValueConversion.Internal;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Json.Microsoft.Storage.Internal
{
    public class MySqlJsonMicrosoftTypeMappingSourcePlugin : MySqlJsonTypeMappingSourcePlugin
    {
        private static readonly Lazy<JsonDocumentValueConverter> _jsonDocumentValueConverter = new Lazy<JsonDocumentValueConverter>();
        private static readonly Lazy<JsonElementValueConverter> _jsonElementValueConverter = new Lazy<JsonElementValueConverter>();
        private static readonly Lazy<JsonStringValueConverter> _jsonStringValueConverter = new Lazy<JsonStringValueConverter>();

        public MySqlJsonMicrosoftTypeMappingSourcePlugin(
            [NotNull] IMySqlOptions options)
            : base(options)
        {
        }

        protected override Type MySqlJsonTypeMappingType => typeof(MySqlJsonMicrosoftTypeMapping<>);

        protected override RelationalTypeMapping FindDomMapping(RelationalTypeMappingInfo mappingInfo)
        {
            var clrType = mappingInfo.ClrType;

            if (clrType == typeof(JsonDocument) ||
                clrType == typeof(JsonElement))
            {
                return (RelationalTypeMapping)Activator.CreateInstance(
                    MySqlJsonTypeMappingType.MakeGenericType(clrType),
                    "json",
                    GetValueConverter(clrType),
                    Options);
            }

            return null;
        }

        protected override ValueConverter GetValueConverter(Type clrType)
        {
            if (clrType == typeof(JsonDocument))
            {
                return _jsonDocumentValueConverter.Value;
            }

            if (clrType == typeof(JsonElement))
            {
                return _jsonElementValueConverter.Value;
            }

            if (clrType == typeof(string))
            {
                return _jsonStringValueConverter.Value;
            }

            return (ValueConverter)Activator.CreateInstance(typeof(JsonPocoValueConverter<>).MakeGenericType(clrType));
        }
    }
}
