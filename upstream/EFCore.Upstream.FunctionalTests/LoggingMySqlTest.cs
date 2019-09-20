// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

// ReSharper disable InconsistentNaming
namespace Microsoft.EntityFrameworkCore
{
    public class LoggingMySqlTest : LoggingRelationalTestBase<MySqlDbContextOptionsBuilder, MySqlOptionsExtension>
    {
        [ConditionalFact]
        public void Logs_context_initialization_row_number_paging()
        {
            Assert.Equal(
                ExpectedMessage("RowNumberPaging " + DefaultOptions),
                ActualMessage(s => CreateOptionsBuilder(s, b => ((MySqlDbContextOptionsBuilder)b).UseRowNumberForPaging())));
        }

        protected override DbContextOptionsBuilder CreateOptionsBuilder(
            IServiceCollection services,
            Action<RelationalDbContextOptionsBuilder<MySqlDbContextOptionsBuilder, MySqlOptionsExtension>> relationalAction)
            => new DbContextOptionsBuilder()
                .UseInternalServiceProvider(services.AddEntityFrameworkMySql().BuildServiceProvider())
                .UseMySql("Data Source=LoggingMySqlTest.db", relationalAction);

        protected override string ProviderName => "Pomelo.EntityFrameworkCore.MySql";
    }
}
