// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Storage;

namespace Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal
{
    internal class MySqlCodeGenerationServerVersionCreation
    {
        public ServerVersion ServerVersion { get; }

        public MySqlCodeGenerationServerVersionCreation(ServerVersion serverVersion)
        {
            ServerVersion = serverVersion;
        }
    }
}
