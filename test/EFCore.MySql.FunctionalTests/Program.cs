using System;
using Microsoft.AspNetCore.Hosting;
using Pomelo.EntityFrameworkCore.MySql.PerfTests.Commands;

namespace Pomelo.EntityFrameworkCore.MySql.PerfTests
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
                Environment.Exit(CommandRunner.Run(args));
            }
        }
    }
}
