// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;
using MySqlConnector;

namespace Pomelo.EntityFrameworkCore.MySql.Storage.Internal
{
    public class MySqlByteTypeMapping : MySqlTypeMapping
    {
        public MySqlByteTypeMapping(
            [NotNull] string storeType)
            : base(storeType, typeof(byte), MySqlDbType.UByte, System.Data.DbType.Byte)
        {
        }

        protected MySqlByteTypeMapping(RelationalTypeMappingParameters parameters, MySqlDbType mySqlDbType)
            : base(parameters, mySqlDbType)
        {
        }

        protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
            => new MySqlByteTypeMapping(parameters, MySqlDbType);

        protected override string SqlLiteralFormatString => "0x{0:X2}";
    }
}
