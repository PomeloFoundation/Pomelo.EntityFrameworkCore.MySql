// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

// ReSharper disable once CheckNamespace
namespace Pomelo.EntityFrameworkCore.MySql.Json.Microsoft.Storage.ValueConversion.Internal
{
    public class MySqlJsonMicrosoftPocoValueConverter<T> : ValueConverter<T, string>
    {
        public MySqlJsonMicrosoftPocoValueConverter()
            : base(
                v => ConvertToProviderCore(v),
                v => ConvertFromProviderCore(v))
        {
        }

        private static string ConvertToProviderCore(T v)
            => JsonSerializer.Serialize(v);

        private static T ConvertFromProviderCore(string v)
            => JsonSerializer.Deserialize<T>(v);
    }
}
