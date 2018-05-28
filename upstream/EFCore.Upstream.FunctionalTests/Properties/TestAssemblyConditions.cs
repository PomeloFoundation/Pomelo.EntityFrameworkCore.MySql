// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit;

[assembly: TestFramework("Microsoft.EntityFrameworkCore.TestUtilities.Xunit.ConditionalTestFramework", "Microsoft.EntityFrameworkCore.Specification.Tests")]

// Skip the entire assembly if not on Windows and no external MySql is configured

[assembly: MySqlConfiguredCondition]
