// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Globalization;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Storage.Internal
{
    public static class ServerVersion
    {
        // Original File: https://github.com/mysql-net/MySqlConnector/blob/master/src/MySqlConnector/MySqlClient/ServerVersion.cs
        public static Version ParseVersion(string versionString)
        {
            var last = 0;
            var index = versionString.IndexOf('.', last);
            var major = int.Parse(versionString.Substring(last, index - last), CultureInfo.InvariantCulture);
            last = index + 1;

            index = versionString.IndexOf('.', last);
            var minor = int.Parse(versionString.Substring(last, index - last), CultureInfo.InvariantCulture);
            last = index + 1;

            do
            {
                index++;
            } while (index < versionString.Length && versionString[index] >= '0' && versionString[index] <= '9');
            var build = int.Parse(versionString.Substring(last, index - last), CultureInfo.InvariantCulture);

            return new Version(major, minor, build);
        }
    }
}