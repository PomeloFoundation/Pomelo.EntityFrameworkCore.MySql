// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.EntityFrameworkCore.Query;

public class MyEntityArrayOfPrimitives
{
    public int Id { get; set; }
    public MyJsonEntityArrayOfPrimitives Reference { get; set; }
    public List<MyJsonEntityArrayOfPrimitives> Collection { get; set; }
}

public class MyJsonEntityArrayOfPrimitives
{
    public int[] IntArray { get; set; }
    public List<string> ListOfString { get; set; }
}
