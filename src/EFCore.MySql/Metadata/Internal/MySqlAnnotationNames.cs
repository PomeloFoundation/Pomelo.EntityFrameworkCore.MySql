// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

//ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public static class MySqlAnnotationNames
    {
        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public const string Prefix = "MySql:";

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public const string ValueGenerationStrategy = Prefix + "ValueGenerationStrategy";

        public const string LegacyValueGeneratedOnAdd = Prefix + "ValueGeneratedOnAdd";

        public const string LegacyValueGeneratedOnAddOrUpdate = Prefix + "ValueGeneratedOnAddOrUpdate";

        public const string FullTextIndex = Prefix + "FullTextIndex";

        public const string SpatialIndex = Prefix + "SpatialIndex";
    }
}
