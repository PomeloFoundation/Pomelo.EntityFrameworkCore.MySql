// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore.Design;

[assembly: AssemblyTitle("Pomelo.EntityFrameworkCore.MySql")]
[assembly: AssemblyDescription("MySQL provider for Entity Framework Core")]
[assembly: DesignTimeProviderServices("Pomelo.EntityFrameworkCore.MySql.Design.Internal.MySqlDesignTimeServices")]
[assembly: InternalsVisibleTo("Pomelo.EntityFrameworkCore.Tests")]
