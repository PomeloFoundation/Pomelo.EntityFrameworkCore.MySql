// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;
using MySqlConnector;

namespace Pomelo.EntityFrameworkCore.MySql.Storage.Internal
{
    public class MySqlYearTypeMapping : MySqlTypeMapping
    {
        public MySqlYearTypeMapping([NotNull] string storeType)
            : base(
                storeType,
                typeof(short),
                MySqlDbType.Year,
                System.Data.DbType.Int16)
        {
        }

        protected MySqlYearTypeMapping(RelationalTypeMappingParameters parameters, MySqlDbType mySqlDbType)
            : base(parameters, mySqlDbType)
        {
        }

        protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
            => new MySqlYearTypeMapping(parameters, MySqlDbType);
    }
}
