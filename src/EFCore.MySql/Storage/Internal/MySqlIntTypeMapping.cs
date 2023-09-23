// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace Pomelo.EntityFrameworkCore.MySql.Storage.Internal;

public class MySqlIntTypeMapping : IntTypeMapping
{
    public static new MySqlIntTypeMapping Default { get; } = new("int");

    public MySqlIntTypeMapping(
        string storeType,
        DbType? dbType = System.Data.DbType.Int32)
        : base(storeType, dbType)
    {
    }

    protected MySqlIntTypeMapping(RelationalTypeMappingParameters parameters)
        : base(parameters)
    {
    }

    protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
        => new MySqlIntTypeMapping(parameters);
}
