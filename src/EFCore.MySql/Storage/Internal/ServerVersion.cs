// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Text.RegularExpressions;
using EFCore.MySql.Infrastructure;

namespace EFCore.MySql.Storage.Internal
{
    public class ServerVersion
    {
        public static Regex ReVersion = new Regex(@"\d+\.\d+\.?(?:\d+)?");
        private static readonly Version DefaultVersion = new Version(8, 0);

        public ServerVersion(string versionString)
        {
            if (versionString != null)
            {
                Type = versionString.ToLower().Contains("mariadb") ? ServerType.MariaDb : ServerType.MySql;
                var semanticVersion = ReVersion.Matches(versionString);
                if (semanticVersion.Count > 0)
                {
                    if (Type == ServerType.MariaDb && semanticVersion.Count > 1)
                        Version = Version.Parse(semanticVersion[1].Value);
                    else
                        Version = Version.Parse(semanticVersion[0].Value);
                }
                else
                {
                    throw new InvalidOperationException(
                        $"Unable to determine server version from version string '${versionString}'");
                }
            }
            else
            {
                Version = DefaultVersion;
            }
        }

        public ServerVersion(Version version, ServerType type)
        {
            Version = version;
            Type = type;
        }

        public readonly ServerType Type;

        public readonly Version Version;

        public bool SupportsDateTime6 => Version >= new Version(5, 6);

        public bool SupportsRenameIndex
        {
            get
            {
                if (Type == ServerType.MySql)
                {
                    return Version >= new Version(5, 7);
                }

                // TODO Awaiting feedback from Mariadb on when they will support rename index!
                return false;
            }
        }
        public override bool Equals(object obj)
            => !(obj is null)
               && obj is ServerVersion version
               && Equals(version);

        private bool Equals(ServerVersion other)
            => Version.Equals(other.Version)
               && Type == other.Type;

        public override int GetHashCode()
            => (Version.GetHashCode() * 397) ^ Type.GetHashCode();
    }
}
