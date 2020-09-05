using System.IO;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

// ReSharper disable once CheckNamespace
namespace Pomelo.EntityFrameworkCore.MySql.Json.Microsoft.Storage.ValueConversion.Internal
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
        {
            using var stream = new MemoryStream();
            using var writer = new Utf8JsonWriter(stream);
            JsonDocument.Parse(v)
                .WriteTo(writer);
            writer.Flush();
            return Encoding.UTF8.GetString(stream.ToArray());
        }
    }
}
