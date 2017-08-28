using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Commands;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
	            var host = new WebHostBuilder()
                    .UseUrls("http://*:5000")
                    .UseKestrel()
                    .UseStartup<Startup>()
                    .Build();
                host.Run();
            }
            else
            {
                var serviceCollection = new ServiceCollection();
                serviceCollection
                    .AddLogging()
                    .AddSingleton<ICommandRunner, CommandRunner>()
                    .AddSingleton<IConnectionStringCommand, ConnectionStringCommand>()
                    .AddSingleton<ITestMigrateCommand, TestMigrateCommand>()
                    .AddSingleton<ITestPerformanceCommand, TestPerformanceCommand>();
                Startup.ConfigureEntityFramework(serviceCollection);
                var serviceProvider = serviceCollection.BuildServiceProvider();

                serviceProvider
                    .GetService<ILoggerFactory>()
                    .AddConsole(AppConfig.Config.GetSection("Logging"));

                var commandRunner = serviceProvider.GetService<ICommandRunner>();

                Environment.Exit(commandRunner.Run(args));
            }
        }
    }
}
