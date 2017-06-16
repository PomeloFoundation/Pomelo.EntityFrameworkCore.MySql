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

        private MySqlOptionsExtension _relationalOptions;

        private readonly Lazy<MySqlConnectionSettings> _lazyConnectionSettings;
        
        public MySqlOptions()
        {
            _lazyConnectionSettings = new Lazy<MySqlConnectionSettings>(() => {
                if (_relationalOptions.Connection != null)
                    return MySqlConnectionSettings.GetSettings(_relationalOptions.Connection);
                return MySqlConnectionSettings.GetSettings(_relationalOptions.ConnectionString);
            });
        }

        public virtual void Initialize(IDbContextOptions options)
        {
            _relationalOptions = options.FindExtension<MySqlOptionsExtension>() ?? new MySqlOptionsExtension();
            
        }

        public virtual void Validate(IDbContextOptions options)
        {
            if (_relationalOptions.ConnectionString == null && _relationalOptions.Connection == null)
                throw new InvalidOperationException(RelationalStrings.NoConnectionOrConnectionString);
        }

        public virtual MySqlConnectionSettings ConnectionSettings => _lazyConnectionSettings.Value;
    }
}
