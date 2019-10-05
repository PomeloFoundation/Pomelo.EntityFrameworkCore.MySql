// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Migrations;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.Tests
{
    public class MySqlMigrationBuilderTest
    {
        [Fact]
        public void IsMySql_when_using_MySql()
        {
            var migrationBuilder = new MigrationBuilder("Pomelo.EntityFrameworkCore.MySql");
            Assert.True(migrationBuilder.IsMySql());
        }

        [Fact]
        public void Not_IsMySql_when_using_different_provider()
        {
            var migrationBuilder = new MigrationBuilder("Microsoft.EntityFrameworkCore.InMemory");
            Assert.False(migrationBuilder.IsMySql());
        }

    }
}
