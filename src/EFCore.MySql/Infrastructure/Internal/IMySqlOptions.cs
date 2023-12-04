// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal
{
    public interface IMySqlOptions : ISingletonOptions
    {
        MySqlConnectionSettings ConnectionSettings { get; }

        /// <remarks>
        /// If null, there might still be a `DbDataSource` in the ApplicationServiceProvider.
        /// </remarks>>
        DbDataSource DataSource { get; }

        ServerVersion ServerVersion { get; }
        CharSet DefaultCharSet { get; }
        CharSet NationalCharSet { get; }
        string DefaultGuidCollation { get; }
        bool NoBackslashEscapes { get; }
        bool ReplaceLineBreaksWithCharFunction { get; }
        MySqlDefaultDataTypeMappings DefaultDataTypeMappings { get; }
        MySqlSchemaNameTranslator SchemaNameTranslator { get; }
        bool IndexOptimizedBooleanColumns { get; }
        MySqlJsonChangeTrackingOptions JsonChangeTrackingOptions { get; }
        bool LimitKeyedOrIndexedStringColumnLength { get; }
        bool StringComparisonTranslations { get; }
        bool PrimitiveCollectionsSupport { get; }
    }
}
