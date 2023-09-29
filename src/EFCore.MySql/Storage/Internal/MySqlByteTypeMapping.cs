// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace Pomelo.EntityFrameworkCore.MySql.Storage.Internal;

public class MySqlByteTypeMapping : ByteTypeMapping
{
    public static new MySqlByteTypeMapping Default { get; } = new("tinyint unsigned");

    public MySqlByteTypeMapping(
        string storeType,
        DbType? dbType = System.Data.DbType.Byte)
        : base(storeType, dbType)
    {
    }

    protected MySqlByteTypeMapping(RelationalTypeMappingParameters parameters)
        : base(parameters)
    {
    }

    protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
        => new MySqlByteTypeMapping(parameters);
}
