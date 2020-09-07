using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// ReSharper disable once CheckNamespace
namespace Pomelo.EntityFrameworkCore.MySql.Json.Newtonsoft.Storage.ValueConversion.Internal
{
    public class JTokenValueConverter : ValueConverter<JToken, string>
    {
        public JTokenValueConverter()
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
