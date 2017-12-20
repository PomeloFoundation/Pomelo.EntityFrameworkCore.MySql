using System;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;

namespace Microsoft.EntityFrameworkCore.Infrastructure.Internal
{
    public interface IMySqlOptions : ISingletonOptions
    {
        MySqlConnectionSettings ConnectionSettings { get; }
        bool SelectForUpdate { get; }

        string GetCreateTable(ISqlGenerationHelper sqlGenerationHelper, string schema, string table);
    }
}
