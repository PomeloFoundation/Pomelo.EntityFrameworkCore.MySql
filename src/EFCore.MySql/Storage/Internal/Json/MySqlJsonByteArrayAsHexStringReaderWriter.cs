// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage.Json;

namespace Pomelo.EntityFrameworkCore.MySql.Storage.Internal.Json;

public sealed class MySqlJsonByteArrayAsHexStringReaderWriter : JsonValueReaderWriter<byte[]>
{
    public static readonly PropertyInfo InstanceProperty =
        typeof(MySqlJsonByteArrayAsHexStringReaderWriter).GetProperty(nameof(Instance));

    public static MySqlJsonByteArrayAsHexStringReaderWriter Instance { get; } = new();

    private MySqlJsonByteArrayAsHexStringReaderWriter()
    {
    }

    public override byte[] FromJsonTyped(ref Utf8JsonReaderManager manager, object existingObject = null)
        => Convert.FromHexString(manager.CurrentReader.GetString()!);

    public override void ToJsonTyped(Utf8JsonWriter writer, byte[] value)
        => writer.WriteStringValue(Convert.ToHexString(value));

    public override Expression ConstructorExpression
        => Expression.Property(null, InstanceProperty);
}
