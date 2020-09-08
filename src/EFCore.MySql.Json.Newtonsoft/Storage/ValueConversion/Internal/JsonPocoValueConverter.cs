using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;

// ReSharper disable once CheckNamespace
namespace Pomelo.EntityFrameworkCore.MySql.Json.Newtonsoft.Storage.ValueConversion.Internal
{
    public class JsonPocoValueConverter<T> : ValueConverter<T, string>
    {
        public JsonPocoValueConverter()
            : base(
                v => ConvertToProviderCore(v),
                v => ConvertFromProviderCore(v))
        {
        }

        private static string ConvertToProviderCore(T v)
            => JsonConvert.SerializeObject(v);

        private static T ConvertFromProviderCore(string v)
            => JsonConvert.DeserializeObject<T>(v);
    }
}
