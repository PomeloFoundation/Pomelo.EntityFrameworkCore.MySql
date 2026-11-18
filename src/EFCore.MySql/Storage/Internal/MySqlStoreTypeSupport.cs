// Copyright (c) Microting. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;

namespace Microting.EntityFrameworkCore.MySql.Storage.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public static class MySqlStoreTypeSupport
    {
        private static readonly HashSet<string> _charSetAndCollationSupportingStoreTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "char",
            "character",
            "nchar",
            "varchar",
            "nvarchar",
            "tinytext",
            "text",
            "mediumtext",
            "longtext",
            "long",
            "enum",
            "set",
        };

        /// <summary>
        ///     Returns <see langword="true" />, if the given store type is a character based one, that supports the
        ///     `CHARACTER SET` and `COLLATE` column attributes.
        ///     Non-character based store types (e.g. `vector`, `uuid`, `inet6`, `json`, numeric and date/time types) reject
        ///     those attributes and will result in a server error, if they are being applied to them.
        /// </summary>
        public static bool SupportsCharSetAndCollation(string storeType)
        {
            if (string.IsNullOrWhiteSpace(storeType))
            {
                return false;
            }

            // Remove any size/precision specification (e.g. `varchar(255)`), as well as any attributes that might follow it
            // (e.g. `varchar(255) CHARACTER SET utf8mb4`).
            var parenthesisIndex = storeType.IndexOf('(');
            var typeName = parenthesisIndex >= 0
                ? storeType.Substring(0, parenthesisIndex)
                : storeType;

            var tokens = typeName.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);

            if (tokens.Length <= 0)
            {
                return false;
            }

            // `national` is a prefix for character based store types (e.g. `national character varying`), while `long` is an
            // alias for `mediumtext` on its own, but also a prefix (e.g. `long varchar`).
            var baseTypeName = tokens.Length > 1 &&
                               string.Equals(tokens[0], "national", StringComparison.OrdinalIgnoreCase)
                ? tokens[1]
                : tokens[0];

            return _charSetAndCollationSupportingStoreTypes.Contains(baseTypeName);
        }
    }
}
