// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using EFCore.MySql.Storage.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EFCore.MySql.Infrastructure.Internal
{
    public interface IMySqlOptions : ISingletonOptions
    {
        MySqlConnectionSettings ConnectionSettings { get; }
        ServerVersion ServerVersion { get; }
    }
}
