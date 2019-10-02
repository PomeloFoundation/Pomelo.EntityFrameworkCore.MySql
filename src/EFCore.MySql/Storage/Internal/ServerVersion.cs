// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
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
        public bool SupportsRenameColumn => RenameColumnVersionSupport.IsSupported(this);
        public bool SupportsWindowFunctions => WindowFunctionsVersionSupport.IsSupported(this);
        public bool SupportsFloatCast => FloatCastVersionSupport.IsSupported(this);
        public bool SupportsDoubleCast => DoubleCastVersionSupport.IsSupported(this);
        public bool SupportsOuterApply => OuterApplyVersionSupport.IsSupported(this);
        public bool SupportsCrossApply => CrossApplyVersionSupport.IsSupported(this);

        public static readonly ServerVersionSupport DateTime6VersionSupport = new ServerVersionSupport(new ServerVersion(DateTime6MySqlSupportVersionString), new ServerVersion(DateTime6MariaDbSupportVersionString));
        public static readonly ServerVersionSupport RenameIndexVersionSupport = new ServerVersionSupport(new ServerVersion(RenameIndexMySqlSupportVersionString));
        public static readonly ServerVersionSupport RenameColumnVersionSupport = new ServerVersionSupport(new ServerVersion(RenameColumnMySqlSupportVersionString));
        public static readonly ServerVersionSupport WindowFunctionsVersionSupport = new ServerVersionSupport(new ServerVersion(WindowFunctionsMySqlSupportVersionString));
        public static readonly ServerVersionSupport OuterApplyVersionSupport = new ServerVersionSupport(new ServerVersion(OuterApplyMySqlSupportVersionString));
        public static readonly ServerVersionSupport CrossApplyVersionSupport = new ServerVersionSupport(new ServerVersion(CrossApplyMySqlSupportVersionString));
        public static readonly ServerVersionSupport FloatCastVersionSupport = new ServerVersionSupport(new ServerVersion(FloatCastMySqlSupportVersionString));
        public static readonly ServerVersionSupport DoubleCastVersionSupport = new ServerVersionSupport(new ServerVersion(DoubleCastMySqlSupportVersionString));

        public const string DateTime6MySqlSupportVersionString = "5.6.0-mysql";
        public const string DateTime6MariaDbSupportVersionString = "10.1.2-mariadb";
        public const string RenameIndexMySqlSupportVersionString = "5.7.0-mysql";
        public const string RenameColumnMySqlSupportVersionString = "8.0.0-mysql";
        public const string WindowFunctionsMySqlSupportVersionString = "8.0.0-mysql";
        public const string OuterApplyMySqlSupportVersionString = "8.0.14-mysql";
        public const string CrossApplyMySqlSupportVersionString = "8.0.14-mysql";
        public const string FloatCastMySqlSupportVersionString = "8.0.17-mysql";
        public const string DoubleCastMySqlSupportVersionString = "8.0.17-mysql";

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

        public override string ToString()
            => Version + "-" + (Type == ServerType.MariaDb ? "mariadb" : "mysql");
    }
}
