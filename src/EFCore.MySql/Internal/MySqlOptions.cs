// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Storage.Internal;

namespace Microsoft.EntityFrameworkCore.Internal
{
    public class MySqlOptions : IMySqlOptions
    {
        public virtual void Initialize(IDbContextOptions options)
        {
            var relationalOptions = options.FindExtension<MySqlOptionsExtension>() ?? new MySqlOptionsExtension();
            if (relationalOptions.ConnectionString != null)
                ConnectionSettings = MySqlConnectionSettings.GetSettings(relationalOptions.ConnectionString);
            else if (relationalOptions.Connection != null)
                ConnectionSettings = MySqlConnectionSettings.GetSettings(relationalOptions.Connection);
        }

        public virtual void Validate(IDbContextOptions options)
        {
            if (ConnectionSettings == null)
                throw new InvalidOperationException(RelationalStrings.NoConnectionOrConnectionString);
        }

        public virtual MySqlConnectionSettings ConnectionSettings { get; private set; }
    }
}
