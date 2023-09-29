// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace Pomelo.EntityFrameworkCore.MySql.Storage.Internal;

public class MySqlUIntTypeMapping : UIntTypeMapping
{
    public static new MySqlUIntTypeMapping Default { get; } = new("int unsigned");

    public MySqlUIntTypeMapping(
        string storeType,
        DbType? dbType = System.Data.DbType.UInt32)
        : base(storeType, dbType)
    {
    }

    protected MySqlUIntTypeMapping(RelationalTypeMappingParameters parameters)
        : base(parameters)
    {
    }

    protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
        => new MySqlUIntTypeMapping(parameters);
}
