// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Microsoft.EntityFrameworkCore
{
    public class MySqlOptionsExtensionTest
    {
        [Fact]
        public void ApplyServices_adds_SQL_server_services()
        {
            var services = new ServiceCollection();

            new MySqlOptionsExtension().ApplyServices(services);

            Assert.True(services.Any(sd => sd.ServiceType == typeof(IMySqlConnection)));
        }

        [Fact]
        public void Changing_RowNumberPagingEnabled_causes_new_service_provider_to_be_built()
        {
            IMySqlOptions singletonOptions;

            using (var context = new ChangedRowNumberContext(rowNumberPagingEnabled: false, setInternalServiceProvider: false))
            {
                _ = context.Model;
                singletonOptions = context.GetService<IMySqlOptions>();
                Assert.False(singletonOptions.RowNumberPagingEnabled);
            }

            using (var context = new ChangedRowNumberContext(rowNumberPagingEnabled: true, setInternalServiceProvider: false))
            {
                _ = context.Model;
                var newOptions = context.GetService<IMySqlOptions>();
                Assert.True(newOptions.RowNumberPagingEnabled);
                Assert.NotSame(newOptions, singletonOptions);
            }
        }

        [Fact]
        public void Changing_RowNumberPagingEnabled_when_UseInternalServiceProvider_throws()
        {
            using (var context = new ChangedRowNumberContext(rowNumberPagingEnabled: false, setInternalServiceProvider: true))
            {
                _ = context.Model;
            }

            using (var context = new ChangedRowNumberContext(rowNumberPagingEnabled: true, setInternalServiceProvider: true))
            {
                Assert.Equal(
                    CoreStrings.SingletonOptionChanged(
                        nameof(MySqlDbContextOptionsBuilder.UseRowNumberForPaging),
                        nameof(DbContextOptionsBuilder.UseInternalServiceProvider)),
                    Assert.Throws<InvalidOperationException>(() => context.Model).Message);
            }
        }

        private class ChangedRowNumberContext : DbContext
        {
            private static readonly IServiceProvider _serviceProvider
                = new ServiceCollection()
                    .AddEntityFrameworkMySql()
                    .BuildServiceProvider();

            private readonly bool _rowNumberPagingEnabled;
            private readonly bool _setInternalServiceProvider;

            public ChangedRowNumberContext(bool rowNumberPagingEnabled, bool setInternalServiceProvider)
            {
                _rowNumberPagingEnabled = rowNumberPagingEnabled;
                _setInternalServiceProvider = setInternalServiceProvider;
            }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                if (_setInternalServiceProvider)
                {
                    optionsBuilder.UseInternalServiceProvider(_serviceProvider);
                }

                optionsBuilder
                    .UseMySql(
                        "Database=Maltesers",
                        b =>
                        {
                            if (_rowNumberPagingEnabled)
                            {
                                b.UseRowNumberForPaging();
                            }
                        });
            }
        }
    }
}
