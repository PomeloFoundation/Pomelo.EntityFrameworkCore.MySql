// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace Pomelo.EntityFrameworkCore.MySql.Storage.Internal;

public class MySqlSByteTypeMapping : SByteTypeMapping
{
    public static new MySqlSByteTypeMapping Default { get; } = new("tinyint");

    public MySqlSByteTypeMapping(
        string storeType,
        DbType? dbType = System.Data.DbType.SByte)
        : base(storeType, dbType)
    {
    }

    protected MySqlSByteTypeMapping(RelationalTypeMappingParameters parameters)
        : base(parameters)
    {
    }

    protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
        => new MySqlSByteTypeMapping(parameters);
}
