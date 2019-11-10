using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Utilities;
using Pomelo.EntityFrameworkCore.MySql.Extensions;
using Pomelo.EntityFrameworkCore.MySql.Storage;

namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    ///     MySQL specific extension methods for <see cref="ModelBuilder" />.
    /// </summary>
    public static class MySqlModelBuilderExtensions
    {
        /// <summary>
        /// Configures the server version the model is intended for.
        /// </summary>
        /// <param name="modelBuilder">The builder for the model being configured.</param>
        /// <param name="serverVersion">The server version as a string that the model is intended for.</param>
        /// <returns>The same builder instance so that multiple calls can be chained.</returns>
        public static ModelBuilder ForServerVersion(
            [NotNull] this ModelBuilder modelBuilder,
            string serverVersion)
        {
            Check.NotNull(modelBuilder, nameof(modelBuilder));

            modelBuilder.Model.SetServerVersion(serverVersion);

            return modelBuilder;
        }

        /// <summary>
        /// Configures the server version the model is intended for.
        /// </summary>
        /// <param name="modelBuilder">The builder for the model being configured.</param>
        /// <param name="serverVersion">The <see cref="ServerVersion"/> that the model is intended for.</param>
        /// <returns>The same builder instance so that multiple calls can be chained.</returns>
        public static ModelBuilder ForServerVersion(
            [NotNull] this ModelBuilder modelBuilder,
            ServerVersion serverVersion)
        {
            Check.NotNull(modelBuilder, nameof(modelBuilder));

            modelBuilder.Model.SetServerVersion(serverVersion.ToString());

            return modelBuilder;
        }
    }
}
