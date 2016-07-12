// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;
using Pomelo.Data.MySql;


namespace Microsoft.EntityFrameworkCore.Storage.Internal
{
    public class MySqlTypeMapping : RelationalTypeMapping
    {
        public new MySqlDbType? StoreType { get; }

        internal MySqlTypeMapping([NotNull] string defaultTypeName, [NotNull] Type clrType, MySqlDbType storeType)
            : base(defaultTypeName, clrType)
        {
            StoreType = storeType;
            
        }

        internal MySqlTypeMapping([NotNull] string defaultTypeName, [NotNull] Type clrType)
            : base(defaultTypeName, clrType)
        { }

        protected override void ConfigureParameter([NotNull] DbParameter parameter)
        {
            if (StoreType.HasValue)
            {
                ((MySqlParameter) parameter).MySqlDbType = StoreType.Value;
            }
        }
    }
}
