// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Data;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore.Utilities;
using MySqlConnector;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore
{
    public abstract class ServerVersion
    {
        private static readonly Regex _versionRegex = new Regex(@"\d+\.\d+\.?(?:\d+)?");

        protected ServerVersion(Version version, ServerType type, string typeIdentifier = null)
        {
            Version = version;
            Type = type;
            TypeIdentifier = typeIdentifier ?? Enum.GetName(typeof(ServerType), type)
                .ToLowerInvariant();
        }

        public virtual Version Version { get; }
        public virtual ServerType Type { get; }
        public virtual string TypeIdentifier { get; }

        public abstract ServerVersionSupport Supports { get; }

        public virtual int MaxKeyLength => Supports.LargerKeyLength ? 3072 : 767;
        public virtual CharSet DefaultCharSet => Supports.DefaultCharSetUtf8Mb4 ? CharSet.Utf8Mb4 : CharSet.Latin1;

        public override bool Equals(object obj)
            => obj is ServerVersion version &&
               Equals(version);

        private bool Equals(ServerVersion other)
            => Version.Equals(other.Version) &&
               Type.Equals(other.Type) &&
               Equals(TypeIdentifier, other.TypeIdentifier);

        public override int GetHashCode()
            => HashCode.Combine(Version, Type, TypeIdentifier);

        public override string ToString()
            => $"{Version}-{TypeIdentifier}";

        public static ServerVersion AutoDetect(string connectionString)
        {
            using var connection = new MySqlConnection(
                new MySqlConnectionStringBuilder(connectionString) {Database = string.Empty}.ConnectionString);
            connection.Open();
            return FromString(connection.ServerVersion);
        }

        public static ServerVersion AutoDetect(MySqlConnection connection)
        {
            string serverVersion;

            if (connection.State != ConnectionState.Open)
            {
                using var clonedConnection = connection.CloneWith(
                    new MySqlConnectionStringBuilder(connection.ConnectionString) {Database = string.Empty}.ConnectionString);
                clonedConnection.Open();
                serverVersion = clonedConnection.ServerVersion;
            }
            else
            {
                serverVersion = connection.ServerVersion;
            }

            return FromString(serverVersion);
        }

        public static ServerVersion FromString(string versionString, ServerType? serverType = null)
        {
            Check.NotEmpty(versionString, nameof(versionString));

            if (!TryFromString(versionString, serverType, out var serverVersion))
            {
                throw new InvalidOperationException($"Unable to determine server version from version string '${versionString}'.");
            }

            return serverVersion;
        }

        public static bool TryFromString(string versionString, out ServerVersion serverVersion)
            => TryFromString(versionString, null, out serverVersion);

        public static bool TryFromString(string versionString, ServerType? serverType, out ServerVersion serverVersion)
        {
            Check.NotEmpty(versionString, nameof(versionString));

            serverVersion = null;

            var semanticVersion = _versionRegex.Matches(versionString);
            if (semanticVersion.Count > 0)
            {
                var type = serverType ??
                           (versionString.ToLower().Contains(MariaDbServerVersion.MariaDbTypeIdentifier)
                               ? ServerType.MariaDb
                               : ServerType.MySql);

                var version = type == ServerType.MariaDb &&
                              semanticVersion.Count > 1
                    ? Version.Parse(semanticVersion[1].Value)
                    : Version.Parse(semanticVersion[0].Value);

                serverVersion = type switch
                {
                    ServerType.MySql => new MySqlServerVersion(version),
                    ServerType.MariaDb => new MariaDbServerVersion(version),
                    _ => null
                };
            }

            return serverVersion != null;
        }
    }
}
