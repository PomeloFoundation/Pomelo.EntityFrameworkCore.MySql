using System.IO;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

// ReSharper disable once CheckNamespace
namespace Pomelo.EntityFrameworkCore.MySql.Json.Microsoft.Storage.ValueConversion.Internal
{
    public class MySqlJsonMicrosoftJsonElementValueConverter : ValueConverter<JsonElement, string>
    {
        public MySqlJsonMicrosoftJsonElementValueConverter()
            : base(
                v => ConvertToProviderCore(v),
                v => ConvertFromProviderCore(v))
        {
        }

        public static string ConvertToProviderCore(JsonElement v)
        {
            using var stream = new MemoryStream();
            using var writer = new Utf8JsonWriter(stream);
            v.WriteTo(writer);
            writer.Flush();
            return Encoding.UTF8.GetString(stream.ToArray());
        }

        public static JsonElement ConvertFromProviderCore(string v)
            => JsonDocument.Parse(v).RootElement;
    }
}
