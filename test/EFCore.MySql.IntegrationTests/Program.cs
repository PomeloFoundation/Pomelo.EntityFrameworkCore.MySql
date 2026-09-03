using System;
using Pomelo.EntityFrameworkCore.MySql.IntegrationTests.Commands;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pomelo.EntityFrameworkCore.MySql.Tests;

namespace Pomelo.EntityFrameworkCore.MySql.IntegrationTests
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
#pragma warning disable ASPDEPR008 // IWebHost/WebHost are obsolete; this integration-test harness still uses the legacy host.
	            BuildWebHost(args).Run();
#pragma warning restore ASPDEPR008
            }
            else
            {
                var serviceCollection = new ServiceCollection();
                serviceCollection
                    .AddLogging(builder =>
                        builder
                            .AddConfiguration(AppConfig.Config.GetSection("Logging"))
                            .AddConsole()
                    )
                    .AddSingleton<ICommandRunner, CommandRunner>()
                    .AddSingleton<IConnectionStringCommand, ConnectionStringCommand>()
                    .AddSingleton<ITestMigrateCommand, TestMigrateCommand>()
                    .AddSingleton<ITestPerformanceCommand, TestPerformanceCommand>();
                Startup.ConfigureEntityFramework(serviceCollection);

#pragma warning disable ASP0000
                var serviceProvider = serviceCollection.BuildServiceProvider();
#pragma warning restore ASP0000

                var commandRunner = serviceProvider.GetService<ICommandRunner>();

                Environment.Exit(commandRunner.Run(args));
            }
        }

#pragma warning disable ASPDEPR008 // IWebHost/WebHost are obsolete; this integration-test harness still uses the legacy host.
        public static IWebHost BuildWebHost(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseUrls("http://*:5000")
                .UseStartup<Startup>()
                .Build();
        }
#pragma warning restore ASPDEPR008

    }
}
