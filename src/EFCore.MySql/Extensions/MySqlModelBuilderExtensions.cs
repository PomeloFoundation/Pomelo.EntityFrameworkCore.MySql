// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Utilities;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace Pomelo.EntityFrameworkCore.MySql.Extensions
{
    public static class MySqlModelBuilderExtensions
    {
        /// <summary>
        /// Sets the default character set to use for the model/database.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        /// <param name="charSet">The name of the character set to use.</param>
        /// <returns>The same builder instance so that multiple calls can be chained.</returns>
        public static ModelBuilder HasCharSet([NotNull] this ModelBuilder modelBuilder, string charSet)
        {
            Check.NotNull(modelBuilder, nameof(modelBuilder));

            modelBuilder.Model.SetCharSet(charSet);

            return modelBuilder;
        }

        /// <summary>
        /// Sets the default character set to use for the model/database.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        /// <param name="charSet">The name of the character set to use.</param>
        /// <returns>The same builder instance so that multiple calls can be chained.</returns>
        public static ModelBuilder HasCharSet([NotNull] this ModelBuilder modelBuilder, CharSet charSet)
        {
            Check.NotNull(modelBuilder, nameof(modelBuilder));

            modelBuilder.Model.SetCharSet(charSet?.Name);

            return modelBuilder;
        }
    }
}
