using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace Pomelo.EntityFrameworkCore.MySql.PerfTests
{
    public static class AppConfig
    {
	    public static readonly bool AppVeyor = !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("APPVEYOR"));
	    public static readonly int EfBatchSize = !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("EF_BATCH_SIZE"))
		    ? Convert.ToInt32(Environment.GetEnvironmentVariable("EF_BATCH_SIZE")) : 1;
	    private static readonly string Ci = Environment.GetEnvironmentVariable("CI")?.ToLower();
	    private static readonly object InitLock = new object();

	    private static IConfigurationRoot _config;
        public static IConfigurationRoot Config
        {
            get
            {
                if (_config == null)
                {
                    lock(InitLock)
                    {
                        if (_config == null)
                        {
                            var pwd = new DirectoryInfo(Directory.GetCurrentDirectory());
                            var basePath = pwd.FullName;
                            if (pwd.Name.StartsWith("netcoreapp"))
                                basePath = pwd.Parent.Parent.Parent.FullName;
                            
                            var builder = new ConfigurationBuilder()
                                .SetBasePath(basePath)
                                .AddJsonFile("appsettings.json")
                                .AddJsonFile("config.json");
                            _config = builder.Build();
                        }
                    }
                }
                return _config;
            }
        }
        
    }
}
