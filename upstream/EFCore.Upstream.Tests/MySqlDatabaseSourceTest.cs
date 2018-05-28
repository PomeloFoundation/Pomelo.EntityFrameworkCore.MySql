// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Reflection;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Xunit;

namespace Microsoft.EntityFrameworkCore
{
    public class MySqlDatabaseSourceTest
    {
        [Fact]
        public void Returns_appropriate_name()
        {
            Assert.Equal(
                typeof(MySqlConnection).GetTypeInfo().Assembly.GetName().Name,
                new DatabaseProvider<MySqlOptionsExtension>(new DatabaseProviderDependencies()).Name);
        }

        [Fact]
        public void Is_configured_when_configuration_contains_associated_extension()
        {
            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseMySql("Database=Crunchie");

            Assert.True(new DatabaseProvider<MySqlOptionsExtension>(new DatabaseProviderDependencies()).IsConfigured(optionsBuilder.Options));
        }

        [Fact]
        public void Is_not_configured_when_configuration_does_not_contain_associated_extension()
        {
            var optionsBuilder = new DbContextOptionsBuilder();

            Assert.False(new DatabaseProvider<MySqlOptionsExtension>(new DatabaseProviderDependencies()).IsConfigured(optionsBuilder.Options));
        }
    }
}
