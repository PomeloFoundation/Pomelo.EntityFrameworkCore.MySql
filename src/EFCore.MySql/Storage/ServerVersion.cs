// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Text.RegularExpressions;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace Pomelo.EntityFrameworkCore.MySql.Storage
{
    public partial class ServerVersion
    {
        private static readonly Regex _versionRegex = new Regex(@"\d+\.\d+\.?(?:\d+)?");

        public static ServerVersion Default = new ServerVersion(new Version(8, 0, 18), ServerType.MySql);

        public ServerVersion()
            : this(null)
        {
        }

        public ServerVersion(string versionString)
        {
            if (versionString != null)
            {
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
            else
            {
                Version = Default.Version;
                Type = Default.Type;
                IsDefault = true;
            }
        }

        public ServerVersion(Version version, ServerType type = ServerType.MySql)
        {
            Version = version;
            Type = type;
        }

        public virtual Version Version { get; }
        public virtual ServerType Type { get; }
        public virtual bool IsDefault { get; }

        public override bool Equals(object obj)
            => !(obj is null)
               && obj is ServerVersion version
               && Equals(version);

        private bool Equals(ServerVersion other)
            => Version.Equals(other.Version)
               && Type == other.Type;

        public override int GetHashCode()
            => (Version.GetHashCode() * 397) ^ Type.GetHashCode();

        public override string ToString()
            => Version + "-" + (Type == ServerType.MariaDb ? "mariadb" : "mysql");
    }
}
