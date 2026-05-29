// Copyright (c) Microting. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

namespace Microting.EntityFrameworkCore.MySql.Extensions
{
    internal static class StringExtensions
    {
        internal static string NullIfEmpty(this string value)
            => value?.Length > 0
                ? value
                : null;
    }
}
