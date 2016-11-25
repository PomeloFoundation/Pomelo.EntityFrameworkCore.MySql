// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Reflection;
using Microsoft.EntityFrameworkCore.Infrastructure;

[assembly: AssemblyTitle("Pomelo.EntityFrameworkCore.MySql")]
[assembly: AssemblyDescription("MySQL provider for Entity Framework Core")]
[assembly: DesignTimeProviderServices(
    typeName: "Microsoft.EntityFrameworkCore.Scaffolding.Internal.MySqlDesignTimeServices",
    assemblyName: "Pomelo.EntityFrameworkCore.MySql.Design, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null",
    packageName: "Pomelo.EntityFrameworkCore.MySql.Design")]
