// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace Pomelo.EntityFrameworkCore.MySql.Storage.Internal;

public class MySqlUShortTypeMapping : UShortTypeMapping
{
    public static new MySqlUShortTypeMapping Default { get; } = new("smallint unsigned");

    public MySqlUShortTypeMapping(
        string storeType,
        DbType? dbType = System.Data.DbType.UInt16)
        : base(storeType, dbType)
    {
    }

    protected MySqlUShortTypeMapping(RelationalTypeMappingParameters parameters)
        : base(parameters)
    {
    }

    protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
        => new MySqlUShortTypeMapping(parameters);
}
