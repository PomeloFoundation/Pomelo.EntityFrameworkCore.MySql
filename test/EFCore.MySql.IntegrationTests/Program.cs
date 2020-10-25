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
                var serviceProvider = serviceCollection.BuildServiceProvider();

                var commandRunner = serviceProvider.GetService<ICommandRunner>();

                Environment.Exit(commandRunner.Run(args));
            }
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseUrls("http://*:5000")
                .UseStartup<Startup>()
                .Build();
        }

    }
}
