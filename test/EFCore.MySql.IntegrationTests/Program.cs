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
	            BuildWebHost(args).Run();
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

#pragma warning disable ASPDEPR008 // IWebHost/WebHost obsolete; integration test host retained intentionally.
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
