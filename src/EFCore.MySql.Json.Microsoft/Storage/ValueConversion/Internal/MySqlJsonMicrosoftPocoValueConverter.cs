// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

// ReSharper disable once CheckNamespace
namespace Pomelo.EntityFrameworkCore.MySql.Json.Microsoft.Storage.ValueConversion.Internal
{
    public class MySqlJsonMicrosoftPocoValueConverter<T> : ValueConverter<T, string>
    {
        public MySqlJsonMicrosoftPocoValueConverter(JsonSerializerOptions jsonSerializerOptions)
            : base(
                v => ConvertToProviderCore(v, jsonSerializerOptions),
                v => ConvertFromProviderCore(v, jsonSerializerOptions))
        {
        }

        public static string ConvertToProviderCore(T v, JsonSerializerOptions jsonSerializerOptions)
            => JsonSerializer.Serialize(v, jsonSerializerOptions);

        public static T ConvertFromProviderCore(string v, JsonSerializerOptions jsonSerializerOptions)
            => JsonSerializer.Deserialize<T>(v, jsonSerializerOptions);
    }
}
