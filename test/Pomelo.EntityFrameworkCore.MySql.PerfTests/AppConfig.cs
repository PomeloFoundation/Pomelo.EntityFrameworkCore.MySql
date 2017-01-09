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

                            if (Ci != null && Ci == "true" && !File.Exists(Path.Combine(basePath, "config.json"))){
                                InitCi(basePath);
                            }
                            
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

        private static void InitCi(string basePath){
            File.Copy(
                Path.Combine(basePath, "config.json.example"),
                Path.Combine(basePath, "config.json"));

            var processInfo = new ProcessStartInfo{
                FileName = "dotnet",
                Arguments = "ef -e \"Ef\" migrations add initial",
                WorkingDirectory = basePath
            };
            var process = Process.Start(processInfo);
            process.WaitForExit();

            foreach (var filePath in Directory.GetFiles(Path.Combine(basePath, "Migrations"))){
                if (filePath.EndsWith(".cs")){
                    var data = File.ReadAllText(filePath);
	                if (!data.Contains("using System.Collections.Generic;"))
	                {
		                File.WriteAllText(filePath, "using System.Collections.Generic;" + Environment.NewLine + data);
	                }
                }
            }

            processInfo = new ProcessStartInfo{
                FileName = "dotnet",
                Arguments = "ef -e \"Ef\" database update",
                WorkingDirectory = basePath
            };
            process = Process.Start(processInfo);
            process.WaitForExit();
        }
        
    }
}
