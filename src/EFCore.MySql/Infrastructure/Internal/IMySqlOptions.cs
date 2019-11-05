// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Internal;
using Pomelo.EntityFrameworkCore.MySql.Storage;

namespace Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal
{
    public interface IMySqlOptions : ISingletonOptions
    {
        MySqlConnectionSettings ConnectionSettings { get; }
        ServerVersion ServerVersion { get; }
        CharSetBehavior CharSetBehavior { get; }
        CharSet CharSet { get; }
        CharSet NationalCharSet { get; }
        bool NoBackslashEscapes { get; }
        bool ReplaceLineBreaksWithCharFunction { get; }
        MySqlDefaultDataTypeMappings DefaultDataTypeMappings { get; }
    }
}
