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

        public bool SupportsDateTime6 => Version >= DateTime6SupportVersion;
        public bool SupportsFloatCast => Version >= FloatCastSupportVersion;
        public bool SupportsDoubleCast => Version >= DoubleCastSupportVersion;
        public bool SupportsOuterApply => Version >= OuterApplySupportVersion;
        public bool SupportsCrossApply => Version >= CrossApplySupportVersion;
        public bool SupportsRenameColumn => Version >= RenameColumnSupportVersion;

        public static readonly Version DateTime6SupportVersion = new Version(DateTime6SupportVersionString);
        public static readonly Version FloatCastSupportVersion = new Version(FloatCastSupportVersionString);
        public static readonly Version DoubleCastSupportVersion = new Version(DoubleCastSupportVersionString);
        public static readonly Version OuterApplySupportVersion = new Version(OuterApplySupportVersionString);
        public static readonly Version CrossApplySupportVersion = new Version(CrossApplySupportVersionString);
        public static readonly Version RenameColumnSupportVersion = new Version(RenameColumnSupportVersionString);

        public const string DateTime6SupportVersionString = "5.6";
        public const string FloatCastSupportVersionString = "8.0.17";
        public const string DoubleCastSupportVersionString = "8.0.17";
        public const string OuterApplySupportVersionString = "8.0.14";
        public const string CrossApplySupportVersionString = "8.0.14";
        public const string RenameColumnSupportVersionString = "8.0.0";

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
    }
}
