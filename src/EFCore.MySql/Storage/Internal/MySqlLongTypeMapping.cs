// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace Pomelo.EntityFrameworkCore.MySql.Storage.Internal;

public class MySqlLongTypeMapping : LongTypeMapping
{
    public static new MySqlLongTypeMapping Default { get; } = new("bigint");

    public MySqlLongTypeMapping(
        string storeType,
        DbType? dbType = System.Data.DbType.Int64)
        : base(storeType, dbType)
    {
    }

    protected MySqlLongTypeMapping(RelationalTypeMappingParameters parameters)
        : base(parameters)
    {
    }

    protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
        => new MySqlLongTypeMapping(parameters);
}
