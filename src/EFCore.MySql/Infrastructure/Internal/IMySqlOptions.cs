// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

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
