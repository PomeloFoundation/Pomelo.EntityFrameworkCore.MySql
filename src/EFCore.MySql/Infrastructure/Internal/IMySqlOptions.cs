// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal
{
    public interface IMySqlOptions : ISingletonOptions
    {
        MySqlConnectionSettings ConnectionSettings { get; }
        ServerVersion ServerVersion { get; }
        CharSetBehavior CharSetBehavior { get; }
        CharSetInfo AnsiCharSetInfo { get; }
        CharSetInfo UnicodeCharSetInfo { get; }
        bool NoBackslashEscapes { get; }
    }
}
