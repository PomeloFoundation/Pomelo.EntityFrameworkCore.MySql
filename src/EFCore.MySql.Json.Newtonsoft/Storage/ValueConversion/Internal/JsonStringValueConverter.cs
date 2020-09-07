using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// ReSharper disable once CheckNamespace
namespace Pomelo.EntityFrameworkCore.MySql.Json.Newtonsoft.Storage.ValueConversion.Internal
{
    public class JsonStringValueConverter : ValueConverter<string, string>
    {
        public JsonStringValueConverter()
            : base(
                v => ConvertToProviderCore(v),
                v => ConvertFromProviderCore(v))
        {
        }

        private static string ConvertToProviderCore(string v)
            => ProcessString(v);

        private static string ConvertFromProviderCore(string v)
            => ProcessString(v);

        private static string ProcessString(string v)
            => JToken.Parse(v).ToString(Formatting.None);
    }
}
