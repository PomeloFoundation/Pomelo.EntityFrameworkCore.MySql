// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Text.Json;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Pomelo.EntityFrameworkCore.MySql.Json.Microsoft.Storage.ValueComparison.Internal;
using Pomelo.EntityFrameworkCore.MySql.Json.Microsoft.Storage.ValueConversion.Internal;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Json.Microsoft.Storage.Internal
{
    public class MySqlJsonMicrosoftTypeMappingSourcePlugin : MySqlJsonTypeMappingSourcePlugin
    {
        private static readonly Lazy<MySqlJsonMicrosoftJsonDocumentValueConverter> _jsonDocumentValueConverter = new Lazy<MySqlJsonMicrosoftJsonDocumentValueConverter>();
        private static readonly Lazy<MySqlJsonMicrosoftJsonElementValueConverter> _jsonElementValueConverter = new Lazy<MySqlJsonMicrosoftJsonElementValueConverter>();
        private static readonly Lazy<MySqlJsonMicrosoftStringValueConverter> _jsonStringValueConverter = new Lazy<MySqlJsonMicrosoftStringValueConverter>();

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
                    GetValueComparer(clrType),
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

            return (ValueConverter)Activator.CreateInstance(
                typeof(MySqlJsonMicrosoftPocoValueConverter<>).MakeGenericType(clrType));
        }

        protected override ValueComparer GetValueComparer(Type clrType)
            => MySqlJsonMicrosoftValueComparer.Create(clrType, Options.JsonChangeTrackingOptions);
    }
}
