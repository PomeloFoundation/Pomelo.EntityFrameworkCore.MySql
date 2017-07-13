// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Text.RegularExpressions;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Storage.Internal
{
    public class ServerVersion
    {
        public static Regex ReVersion = new Regex(@"\d+\.\d+\.?(?:\d+)?");

        public ServerVersion(string versionString)
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
                throw new InvalidOperationException($"Unable to determine server version from version string '${versionString}'");
            }
        }

        public readonly ServerType Type;

        public readonly Version Version;

        public bool SupportsDateTime6 => Version >= new Version(5,6);

        public bool SupportsRenameIndex
        {
            get
            {
                if (Type == ServerType.MySql)
                {
                    return Version >= new Version(5,7);
                }
                
                // TODO Awaiting feedback from Mariadb on when they will support rename index!
                return false;
            }
        }
    }

    public enum ServerType
    {
        MySql,
        MariaDb
    }
}
