using System;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Storage.Internal
{
    public class MySqlConnectionInfo : IMySqlConnectionInfo
    {
        private readonly ServerVersion _optionsServerVersion;
        private ServerVersion _connectionServerVersion;

        public MySqlConnectionInfo(IMySqlOptions options)
        {
            _optionsServerVersion = options.ServerVersion;
        }

        /// <summary>
        /// The actual ServerVersion of the connection or IMySqlOptions.ServerVersion,
        /// if the latter was set explicitly.
        /// </summary>
        /// <remarks>If the property is retrieved before the connection was opened, it
        /// returns the default ServerVersion of IMySqlOptions with the `IsDefault`
        /// property set to `true`.</remarks>
        public ServerVersion ServerVersion
        {
            get => _connectionServerVersion ?? _optionsServerVersion;
            internal set => _connectionServerVersion = value;
        }

        internal static void SetServerVersion(MySqlConnection connection, IServiceProvider serviceProvider)
        {
            var connectionInfo = (MySqlConnectionInfo)serviceProvider.GetRequiredService<IMySqlConnectionInfo>();
            var options = serviceProvider.GetRequiredService<IMySqlOptions>();

            if (options.ServerVersion.IsDefault)
            {
                connectionInfo.ServerVersion = new ServerVersion(connection.ServerVersion);
            }
            else
            {
                connectionInfo.ServerVersion = options.ServerVersion;
            }
        }
    }
}
