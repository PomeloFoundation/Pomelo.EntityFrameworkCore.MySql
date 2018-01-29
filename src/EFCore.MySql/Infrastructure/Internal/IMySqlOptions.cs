using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;

//ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Infrastructure.Internal
{
    public interface IMySqlOptions : ISingletonOptions
    {
        MySqlConnectionSettings ConnectionSettings { get; }

        string GetCreateTable(ISqlGenerationHelper sqlGenerationHelper, string schema, string table);
    }
}
