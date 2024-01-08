using System.Text.Json;

namespace Pomelo.EntityFrameworkCore.MySql.Json.Microsoft.Infrastructure.Internal;

public sealed class DefaultMysqlJsonOptions : IMysqlJsonOptions
{
    public JsonSerializerOptions JsonSerializerOptions { get; init; } = new();
}
