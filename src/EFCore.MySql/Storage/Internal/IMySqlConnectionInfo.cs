namespace Pomelo.EntityFrameworkCore.MySql.Storage.Internal
{
    public interface IMySqlConnectionInfo
    {
        /// <summary>
        /// The actual ServerVersion of the connection or IMySqlOptions.ServerVersion,
        /// if the latter was set explicitly.
        /// </summary>
        /// <remarks>If the property is retrieved before the connection was opened, it
        /// returns the default ServerVersion of IMySqlOptions with the `IsDefault`
        /// property set to `true`.</remarks>
        ServerVersion ServerVersion { get; }
    }
}
