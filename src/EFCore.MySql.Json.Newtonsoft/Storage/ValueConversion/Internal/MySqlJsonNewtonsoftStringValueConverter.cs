using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// ReSharper disable once CheckNamespace
namespace Pomelo.EntityFrameworkCore.MySql.Json.Newtonsoft.Storage.ValueConversion.Internal
{
    public class MySqlJsonNewtonsoftStringValueConverter : ValueConverter<string, string>
    {
        public MySqlJsonNewtonsoftStringValueConverter()
            : base(
                v => ConvertToProviderCore(v),
                v => ConvertFromProviderCore(v))
        {
        }

        private static string ConvertToProviderCore(string v)
            => ProcessJsonString(v);

        private static string ConvertFromProviderCore(string v)
            => ProcessJsonString(v);

        internal static string ProcessJsonString(string v)
            => JToken.Parse(v).ToString(Formatting.None);
    }
}
