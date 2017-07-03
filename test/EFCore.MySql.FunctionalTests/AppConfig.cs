using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
{
    public static class AppConfig
    {
	    public static readonly bool AppVeyor = !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("APPVEYOR"));

	    public static readonly int EfBatchSize = !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("EF_BATCH_SIZE"))
		    ? Convert.ToInt32(Environment.GetEnvironmentVariable("EF_BATCH_SIZE")) : 1;

        public static readonly string EfSchema = !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("EF_SCHEMA"))
            ? Environment.GetEnvironmentVariable("EF_SCHEMA") : null;

        public static IConfigurationRoot Config => LazyConfig.Value;

        private static readonly Lazy<IConfigurationRoot> LazyConfig = new Lazy<IConfigurationRoot>(() =>
        {
            var pwd = new DirectoryInfo(Directory.GetCurrentDirectory());
            var basePath = pwd.FullName;
            if (pwd.Name.StartsWith("netcoreapp"))
                basePath = pwd.Parent.Parent.Parent.FullName;

            return new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json")
                .AddJsonFile("config.json")
                .Build();
        });
        
    }
}
