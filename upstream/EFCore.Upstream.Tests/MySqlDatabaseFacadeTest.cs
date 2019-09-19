// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Xunit;

namespace Microsoft.EntityFrameworkCore
{
    public class MySqlDatabaseFacadeTest
    {
        [ConditionalFact]
        public void IsMySql_when_using_OnConfiguring()
        {
            using (var context = new MySqlOnConfiguringContext())
            {
                Assert.True(context.Database.IsMySql());
            }
        }

        [ConditionalFact]
        public void IsMySql_in_OnModelCreating_when_using_OnConfiguring()
        {
            using (var context = new MySqlOnModelContext())
            {
                var _ = context.Model; // Trigger context initialization
                Assert.True(context.IsMySqlSet);
            }
        }

        [ConditionalFact]
        public void IsMySql_in_constructor_when_using_OnConfiguring()
        {
            using (var context = new MySqlConstructorContext())
            {
                var _ = context.Model; // Trigger context initialization
                Assert.True(context.IsMySqlSet);
            }
        }

        [ConditionalFact]
        public void Cannot_use_IsMySql_in_OnConfiguring()
        {
            using (var context = new MySqlUseInOnConfiguringContext())
            {
                Assert.Equal(
                    CoreStrings.RecursiveOnConfiguring,
                    Assert.Throws<InvalidOperationException>(
                        () =>
                        {
                            var _ = context.Model; // Trigger context initialization
                        }).Message);
            }
        }

        [ConditionalFact]
        public void IsMySql_when_using_constructor()
        {
            using (var context = new ProviderContext(
                new DbContextOptionsBuilder()
                    .UseInternalServiceProvider(MySqlFixture.DefaultServiceProvider)
                    .UseMySql("Database=Maltesers").Options))
            {
                Assert.True(context.Database.IsMySql());
            }
        }

        [ConditionalFact]
        public void IsMySql_in_OnModelCreating_when_using_constructor()
        {
            using (var context = new ProviderOnModelContext(
                new DbContextOptionsBuilder()
                    .UseInternalServiceProvider(MySqlFixture.DefaultServiceProvider)
                    .UseMySql("Database=Maltesers").Options))
            {
                var _ = context.Model; // Trigger context initialization
                Assert.True(context.IsMySqlSet);
            }
        }

        [ConditionalFact]
        public void IsMySql_in_constructor_when_using_constructor()
        {
            using (var context = new ProviderConstructorContext(
                new DbContextOptionsBuilder()
                    .UseInternalServiceProvider(MySqlFixture.DefaultServiceProvider)
                    .UseMySql("Database=Maltesers").Options))
            {
                var _ = context.Model; // Trigger context initialization
                Assert.True(context.IsMySqlSet);
            }
        }

        [ConditionalFact]
        public void Cannot_use_IsMySql_in_OnConfiguring_with_constructor()
        {
            using (var context = new ProviderUseInOnConfiguringContext(
                new DbContextOptionsBuilder()
                    .UseInternalServiceProvider(MySqlFixture.DefaultServiceProvider)
                    .UseMySql("Database=Maltesers").Options))
            {
                Assert.Equal(
                    CoreStrings.RecursiveOnConfiguring,
                    Assert.Throws<InvalidOperationException>(
                        () =>
                        {
                            var _ = context.Model; // Trigger context initialization
                        }).Message);
            }
        }

        [ConditionalFact]
        public void Not_IsMySql_when_using_different_provider()
        {
            using (var context = new ProviderContext(
                new DbContextOptionsBuilder()
                    .UseInternalServiceProvider(InMemoryFixture.DefaultServiceProvider)
                    .UseInMemoryDatabase("Maltesers").Options))
            {
                Assert.False(context.Database.IsMySql());
            }
        }

        private class ProviderContext : DbContext
        {
            protected ProviderContext()
            {
            }

            public ProviderContext(DbContextOptions options)
                : base(options)
            {
            }

            public bool? IsMySqlSet { get; protected set; }
        }

        private class MySqlOnConfiguringContext : ProviderContext
        {
            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder
                    .UseInternalServiceProvider(MySqlFixture.DefaultServiceProvider)
                    .UseMySql("Database=Maltesers");
        }

        private class MySqlOnModelContext : MySqlOnConfiguringContext
        {
            protected override void OnModelCreating(ModelBuilder modelBuilder)
                => IsMySqlSet = Database.IsMySql();
        }

        private class MySqlConstructorContext : MySqlOnConfiguringContext
        {
            public MySqlConstructorContext()
                => IsMySqlSet = Database.IsMySql();
        }

        private class MySqlUseInOnConfiguringContext : MySqlOnConfiguringContext
        {
            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                base.OnConfiguring(optionsBuilder);

                IsMySqlSet = Database.IsMySql();
            }
        }

        private class ProviderOnModelContext : ProviderContext
        {
            public ProviderOnModelContext(DbContextOptions options)
                : base(options)
            {
            }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
                => IsMySqlSet = Database.IsMySql();
        }

        private class ProviderConstructorContext : ProviderContext
        {
            public ProviderConstructorContext(DbContextOptions options)
                : base(options)
                => IsMySqlSet = Database.IsMySql();
        }

        private class ProviderUseInOnConfiguringContext : ProviderContext
        {
            public ProviderUseInOnConfiguringContext(DbContextOptions options)
                : base(options)
            {
            }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => IsMySqlSet = Database.IsMySql();
        }
    }
}
