﻿using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
{
    public static class AppConfig
    {
	    public static readonly bool AppVeyor = !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("APPVEYOR"));

	    public static readonly int EfBatchSize = !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("EF_BATCH_SIZE"))
		    ? Convert.ToInt32(Environment.GetEnvironmentVariable("EF_BATCH_SIZE")) : 1;

        public static readonly int EfRetryOnFailure = !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("EF_RETRY_ON_FAILURE"))
		    ? Convert.ToInt32(Environment.GetEnvironmentVariable("EF_RETRY_ON_FAILURE")) : 0;

        public static readonly string EfSchema = !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("EF_SCHEMA"))
            ? Environment.GetEnvironmentVariable("EF_SCHEMA") : null;

        public static IConfigurationRoot Config => _lazyConfig.Value;

        private static readonly Lazy<IConfigurationRoot> _lazyConfig = new Lazy<IConfigurationRoot>(() => new ConfigurationBuilder()
            .SetBasePath(Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath))
            .AddJsonFile("config.json")
            .Build());
    }
}
