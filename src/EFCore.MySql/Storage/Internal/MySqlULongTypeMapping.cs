// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace Pomelo.EntityFrameworkCore.MySql.Storage.Internal;

public class MySqlULongTypeMapping : ULongTypeMapping
{
    public static new MySqlULongTypeMapping Default { get; } = new("bigint unsigned");

    public MySqlULongTypeMapping(
        string storeType,
        DbType? dbType = System.Data.DbType.UInt64)
        : base(storeType, dbType)
    {
    }

    protected MySqlULongTypeMapping(RelationalTypeMappingParameters parameters)
        : base(parameters)
    {
    }

    protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
        => new MySqlULongTypeMapping(parameters);
}
