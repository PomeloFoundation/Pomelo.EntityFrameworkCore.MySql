// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.EntityFrameworkCore.Query;

public class MyEntity29219
{
    public int Id { get; set; }
    public MyJsonEntity29219 Reference { get; set; }
    public List<MyJsonEntity29219> Collection { get; set; }
}

public class MyJsonEntity29219
{
    public int NonNullableScalar { get; set; }
    public int? NullableScalar { get; set; }
}
