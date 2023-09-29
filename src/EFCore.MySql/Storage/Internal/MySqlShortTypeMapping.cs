// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace Pomelo.EntityFrameworkCore.MySql.Storage.Internal;

public class MySqlShortTypeMapping : ShortTypeMapping
{
    public static new MySqlShortTypeMapping Default { get; } = new("smallint");

    public MySqlShortTypeMapping(
        string storeType,
        DbType? dbType = System.Data.DbType.Int16)
        : base(storeType, dbType)
    {
    }

    protected MySqlShortTypeMapping(RelationalTypeMappingParameters parameters)
        : base(parameters)
    {
    }

    protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
        => new MySqlShortTypeMapping(parameters);
}
