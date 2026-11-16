using System;
using System.Linq;
using Microting.EntityFrameworkCore.MySql.IntegrationTests.Commands;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microting.EntityFrameworkCore.MySql.Tests;
using Xunit.MicrosoftTestingPlatform;
using Xunit.Runner.InProc.SystemConsole;

namespace Microting.EntityFrameworkCore.MySql.IntegrationTests
{
    public class Program
    {
        /// <summary>
        ///     The commands understood by <see cref="CommandRunner" />. Anything else is assumed to be an invocation by the
        ///     test platform.
        /// </summary>
        private static readonly string[] Commands = { "connectionString", "testMigrate", "testPerformance", "-h", "--help" };

        public static int Main(string[] args)
        {
            if (args.Length == 0 ||
                args.Any(arg => string.Equals(arg, "--applicationName", StringComparison.OrdinalIgnoreCase)))
            {
                BuildWebApplication(args)
                    .Run();

                return 0;
            }

            if (!Commands.Contains(args[0], StringComparer.Ordinal))
            {
                return RunTests(args);
            }

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

            // Never reached, because Environment.Exit() terminates the process.
            return 0;
        }

        /// <summary>
        ///     Hands control over to the xUnit test platform.
        /// </summary>
        /// <remarks>
        ///     This project defines its own entry point (see the StartupObject property of the project file), so the entry
        ///     point that xUnit generates is never invoked. Since xUnit v3 runs on top of Microsoft.Testing.Platform, which
        ///     runs tests by launching the test assembly as an executable, the entry point has to do this itself. This mirrors
        ///     what the generated entry point would have done.
        /// </remarks>
        private static int RunTests(string[] args)
            => args.Any(arg => arg == "--server" || arg == "--internal-msbuild-node")
                ? TestPlatformTestFramework.RunAsync(args, SelfRegisteredExtensions.AddSelfRegisteredExtensions)
                    .GetAwaiter()
                    .GetResult()
                : ConsoleRunner.Run(args)
                    .GetAwaiter()
                    .GetResult();

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
