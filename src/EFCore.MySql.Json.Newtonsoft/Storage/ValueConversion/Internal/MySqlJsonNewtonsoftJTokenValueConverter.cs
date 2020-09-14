// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// ReSharper disable once CheckNamespace
namespace Pomelo.EntityFrameworkCore.MySql.Json.Newtonsoft.Storage.ValueConversion.Internal
{
    public class MySqlJsonNewtonsoftJTokenValueConverter : ValueConverter<JToken, string>
    {
        public MySqlJsonNewtonsoftJTokenValueConverter()
            : base(
                v => ConvertToProviderCore(v),
                v => ConvertFromProviderCore(v))
        {
        }

        private static string ConvertToProviderCore(JToken v)
            => v.ToString(Formatting.None);

        private static JToken ConvertFromProviderCore(string v)
            => JToken.Parse(v);
    }
}
