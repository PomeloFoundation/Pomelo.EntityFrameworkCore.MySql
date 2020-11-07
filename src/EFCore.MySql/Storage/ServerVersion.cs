// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore.Utilities;
using MySqlConnector;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore
{
    public partial class ServerVersion
    {
        private static readonly Regex _versionRegex = new Regex(@"\d+\.\d+\.?(?:\d+)?");

        public static readonly ServerVersion LatestSupportedMySqlVersion = new ServerVersion(new Version(8, 0, 21), ServerType.MySql);
        public static readonly ServerVersion LatestSupportedMariaDbVersion = new ServerVersion(new Version(10, 5, 5), ServerType.MariaDb);

        public ServerVersion()
            : this(null)
        {
        }

        public ServerVersion(string versionString)
        {
            Check.NotEmpty(versionString, nameof(versionString));

            Type = versionString.ToLower().Contains("mariadb") ? ServerType.MariaDb : ServerType.MySql;
            var semanticVersion = _versionRegex.Matches(versionString);
            if (semanticVersion.Count > 0)
            {
                Version = Type == ServerType.MariaDb && semanticVersion.Count > 1
                    ? Version.Parse(semanticVersion[1].Value)
                    : Version.Parse(semanticVersion[0].Value);
            }
            else
            {
                throw new InvalidOperationException(
                    $"Unable to determine server version from version string '${versionString}'");
            }
        }

        public ServerVersion(Version version, ServerType type = ServerType.MySql)
        {
            Version = version;
            Type = type;
        }

        public virtual Version Version { get; }
        public virtual ServerType Type { get; }

        public override bool Equals(object obj)
            => obj is ServerVersion version &&
               Equals(version);

        private bool Equals(ServerVersion other)
            => Version.Equals(other.Version)
               && Type == other.Type;

        public override int GetHashCode()
            => HashCode.Combine(Version, Type);

        public override string ToString()
            => $"{Version}-{(Type == ServerType.MariaDb ? "mariadb" : "mysql")}";

        public static ServerVersion AutoDetect(string connectionString)
        {
            using var connection = new MySqlConnection(
                new MySqlConnectionStringBuilder(connectionString)
                {
                    Database = string.Empty
                }.ConnectionString);
            connection.Open();
            return new ServerVersion(connection.ServerVersion);
        }

        public static ServerVersion AutoDetect(MySqlConnection connection)
        {
            using var clonedConnection = connection.CloneWith(
                new MySqlConnectionStringBuilder(connection.ConnectionString)
                {
                    Database = string.Empty
                }.ConnectionString);
            clonedConnection.Open();
            return new ServerVersion(clonedConnection.ServerVersion);
        }
    }
}
