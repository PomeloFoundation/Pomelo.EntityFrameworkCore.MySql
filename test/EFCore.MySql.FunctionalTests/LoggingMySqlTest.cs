using System;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.MySql.Tests;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
{
    public class LoggingMySqlTest : LoggingRelationalTestBase<MySqlDbContextOptionsBuilder, MySqlOptionsExtension>
    {
        protected override DbContextOptionsBuilder CreateOptionsBuilder(
            IServiceCollection services,
            Action<RelationalDbContextOptionsBuilder<MySqlDbContextOptionsBuilder, MySqlOptionsExtension>> relationalAction)
            => new DbContextOptionsBuilder()
                .UseInternalServiceProvider(services.AddEntityFrameworkMySql().BuildServiceProvider())
                .UseMySql("Database=DummyDatabase", AppConfig.ServerVersion, relationalAction);

        protected override string ProviderName => "Pomelo.EntityFrameworkCore.MySql";
        protected override string DefaultOptions => $"ServerVersion {AppConfig.ServerVersion}";
    }
}
