// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Text.Json;

namespace Pomelo.EntityFrameworkCore.MySql.Json.Microsoft.Infrastructure;

public interface IMysqlJsonOptions
{
    JsonSerializerOptions JsonSerializerOptions { get; }
}
