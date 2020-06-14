using System.IO;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Pomelo.EntityFrameworkCore.MySql.Storage.ValueConversion.Internal
{
    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public class JsonDocumentValueConverter : ValueConverter<JsonDocument, string>
    {
        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public JsonDocumentValueConverter()
            : base(
                v => ConvertToProviderCore(v),
                v => ConvertFromProviderCore(v))
        {
        }

        private static string ConvertToProviderCore(JsonDocument v)
        {
            using var stream = new MemoryStream();
            using var writer = new Utf8JsonWriter(stream);
            v.WriteTo(writer);
            writer.Flush();
            return Encoding.UTF8.GetString(stream.ToArray());
        }

        private static JsonDocument ConvertFromProviderCore(string v)
            => JsonDocument.Parse(v);
    }
}
