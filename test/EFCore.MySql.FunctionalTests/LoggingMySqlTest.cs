using System;
using System.Reflection;
using Microting.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microting.EntityFrameworkCore.MySql.Diagnostics.Internal;
using Microting.EntityFrameworkCore.MySql.Tests;

namespace Microting.EntityFrameworkCore.MySql.FunctionalTests
{
    // TODO: Reenable once this issue has been fixed in EF Core upstream.
    // Skip because LoggingTestBase uses the wrong order:
    // Wrong:   DefaultOptions + "NoTracking"
    // Correct: "NoTracking" + DefaultOptions
    // The order in LoggingRelationalTestBase<,> is correct though.
    internal class LoggingMySqlTest : LoggingRelationalTestBase<MySqlDbContextOptionsBuilder, MySqlOptionsExtension>
    {
        protected override DbContextOptionsBuilder CreateOptionsBuilder(
            IServiceCollection services,
            Action<RelationalDbContextOptionsBuilder<MySqlDbContextOptionsBuilder, MySqlOptionsExtension>> relationalAction)
            => new DbContextOptionsBuilder()
                .UseInternalServiceProvider(services.AddEntityFrameworkMySql().BuildServiceProvider(validateScopes: true))
                .UseMySql("Database=DummyDatabase", AppConfig.ServerVersion, relationalAction);

        protected override TestLogger CreateTestLogger()
            => new TestLogger<MySqlLoggingDefinitions>();

        protected override string ProviderName => "Microting.EntityFrameworkCore.MySql";

        protected override string ProviderVersion => typeof(MySqlOptionsExtension).Assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion;

        protected override string DefaultOptions => $"ServerVersion {AppConfig.ServerVersion} ";
    }
}
