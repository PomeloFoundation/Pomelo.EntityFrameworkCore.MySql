// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Json;
using MySqlConnector;

namespace Pomelo.EntityFrameworkCore.MySql.Storage.Internal
{
    public class MySqlYearTypeMapping : MySqlTypeMapping
    {
        public static MySqlYearTypeMapping Default { get; } = new("year");

        public MySqlYearTypeMapping([NotNull] string storeType)
            : base(
                storeType,
                typeof(short),
                MySqlDbType.Year,
                System.Data.DbType.Int16,
                jsonValueReaderWriter: JsonInt16ReaderWriter.Instance)
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
