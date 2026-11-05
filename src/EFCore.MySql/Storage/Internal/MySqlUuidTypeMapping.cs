// Copyright (c) Microting. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Json;

namespace Microting.EntityFrameworkCore.MySql.Storage.Internal
{
    /// <summary>
    ///     Maps <see cref="Guid" /> to the native `uuid` store type, that has been introduced in MariaDB 10.7.
    ///     Unlike the `char` and `binary` based Guid store types, `uuid` does not support a length/size specification.
    /// </summary>
    public class MySqlUuidTypeMapping : GuidTypeMapping
    {
        public static new MySqlUuidTypeMapping Default { get; } = new();

        public MySqlUuidTypeMapping()
            : this(new RelationalTypeMappingParameters(
                new CoreTypeMappingParameters(
                    typeof(Guid),
                    jsonValueReaderWriter: JsonGuidReaderWriter.Instance),
                "uuid",
                StoreTypePostfix.None,
                System.Data.DbType.Guid))
        {
        }

        protected MySqlUuidTypeMapping(RelationalTypeMappingParameters parameters)
            : base(parameters)
        {
        }

        protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
            => new MySqlUuidTypeMapping(parameters);

        protected override string GenerateNonNullSqlLiteral(object value)
            => $"'{value:D}'";
    }
}
