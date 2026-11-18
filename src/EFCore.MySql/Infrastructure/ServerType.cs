// Copyright (c) Microting. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

namespace Microting.EntityFrameworkCore.MySql.Infrastructure
{
    public enum ServerType
    {
        /// <summary>
        /// Custom server implementation
        /// </summary>
        Custom = -1,

        /// <summary>
        /// MySQL server
        /// </summary>
        MySql,

        /// <summary>
        /// MariaDB server
        /// </summary>
        MariaDb
    }
}
