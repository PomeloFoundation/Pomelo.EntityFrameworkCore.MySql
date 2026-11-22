using System;
using System.Linq;
using Pomelo.EntityFrameworkCore.MySql.IntegrationTests.Commands;
using Microsoft.AspNetCore.Builder;
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
            if (args.Length == 0 ||
                args.Any(arg => string.Equals(arg, "--applicationName", StringComparison.OrdinalIgnoreCase)))
            {
                BuildWebApplication(args)
                    .Run();
            }
            else
            {
                Console.WriteLine("Args:");
                for (var i = 0; i < args.Length; i++)
                {
                    Console.WriteLine($"{i}: {args[i]}");
                }

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

        private static WebApplication BuildWebApplication(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.WebHost
                .UseUrls("http://*:5000");
            Startup.ConfigureServices(builder.Services);

            var app = builder.Build();
            Startup.Configure(app, app.Environment);

            return app;
        }
    }
}
