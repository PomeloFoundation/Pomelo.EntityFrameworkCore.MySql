// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore.Storage;

namespace Pomelo.EntityFrameworkCore.MySql.Storage.Internal
{
    public interface IDefaultValueCompatibilityAware
    {
        /// <summary>
        ///     Creates a copy of this mapping.
        /// </summary>
        /// <param name="isDefaultValueCompatible"> Use a default value compatible syntax, or not. </param>
        /// <returns> The newly created mapping. </returns>
        RelationalTypeMapping Clone(bool isDefaultValueCompatible = false);
    }
}
