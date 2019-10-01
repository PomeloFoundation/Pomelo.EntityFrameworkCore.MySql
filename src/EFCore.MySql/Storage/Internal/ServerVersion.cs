// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Linq;
using System.Text.RegularExpressions;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace Pomelo.EntityFrameworkCore.MySql.Storage.Internal
{
    public class ServerVersion
    {
        public static Regex ReVersion = new Regex(@"\d+\.\d+\.?(?:\d+)?");
        private static readonly Version _defaultVersion = new Version(8, 0, 0);

        public ServerVersion(string versionString)
        {
            if (versionString != null)
            {
                Type = versionString.ToLower().Contains("mariadb") ? ServerType.MariaDb : ServerType.MySql;
                var semanticVersion = ReVersion.Matches(versionString);
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
                Version = _defaultVersion;
            }
        }

        public ServerVersion(Version version, ServerType type = ServerType.MySql)
        {
            Version = version;
            Type = type;
        }

        public readonly ServerType Type;
        public readonly Version Version;

        public bool SupportsDateTime6 => DateTime6VersionSupport.IsSupported(this);
        public bool SupportsRenameIndex => RenameIndexVersionSupport.IsSupported(this);
        public bool SupportsFloatCast => FloatCastVersionSupport.IsSupported(this);
        public bool SupportsDoubleCast => DoubleCastVersionSupport.IsSupported(this);
        public bool SupportsOuterApply => OuterApplyVersionSupport.IsSupported(this);
        public bool SupportsCrossApply => CrossApplyVersionSupport.IsSupported(this);
        public bool SupportsRenameColumn => RenameColumnVersionSupport.IsSupported(this);

        protected static readonly ServerVersionSupport DateTime6VersionSupport = new ServerVersionSupport(new ServerVersion(DateTime6SupportVersionString));
        protected static readonly ServerVersionSupport RenameIndexVersionSupport = new ServerVersionSupport(new ServerVersion(RenameIndexSupportVersionString));
        protected static readonly ServerVersionSupport RenameColumnVersionSupport = new ServerVersionSupport(new ServerVersion(RenameColumnSupportVersionString));
        protected static readonly ServerVersionSupport OuterApplyVersionSupport = new ServerVersionSupport(new ServerVersion(OuterApplySupportVersionString));
        protected static readonly ServerVersionSupport CrossApplyVersionSupport = new ServerVersionSupport(new ServerVersion(CrossApplySupportVersionString));
        protected static readonly ServerVersionSupport FloatCastVersionSupport = new ServerVersionSupport(new ServerVersion(FloatCastSupportVersionString));
        protected static readonly ServerVersionSupport DoubleCastVersionSupport = new ServerVersionSupport(new ServerVersion(DoubleCastSupportVersionString));

        public const string DateTime6SupportVersionString = "5.6.0-mysql";
        public const string RenameIndexSupportVersionString = "5.7.0-mysql";
        public const string RenameColumnSupportVersionString = "8.0.0-mysql";
        public const string OuterApplySupportVersionString = "8.0.14-mysql";
        public const string CrossApplySupportVersionString = "8.0.14-mysql";
        public const string FloatCastSupportVersionString = "8.0.17-mysql";
        public const string DoubleCastSupportVersionString = "8.0.17-mysql";

        public int IndexMaxBytes =>
            (Type == ServerType.MySql && Version >= new Version(5, 7, 7))
            || (Type == ServerType.MariaDb && Version >= new Version(10, 2, 2))
                ? 3072
                : 767;

        public override bool Equals(object obj)
            => !(obj is null)
               && obj is ServerVersion version
               && Equals(version);

        private bool Equals(ServerVersion other)
            => Version.Equals(other.Version)
               && Type == other.Type;

        public override int GetHashCode()
            => (Version.GetHashCode() * 397) ^ Type.GetHashCode();
        
        protected class ServerVersionSupport
        {
            public ServerVersion[] SupportedServerVersions { get; }

            public ServerVersionSupport(params ServerVersion[] supportedServerVersions)
            {
                SupportedServerVersions = supportedServerVersions;
            }

            public bool IsSupported(ServerVersion serverVersion)
            {
                if (SupportedServerVersions.Length <= 0)
                {
                    return false;
                }

                return SupportedServerVersions.Any(s => serverVersion.Type == s.Type && serverVersion.Version >= s.Version);
            }
        }
    }
}
