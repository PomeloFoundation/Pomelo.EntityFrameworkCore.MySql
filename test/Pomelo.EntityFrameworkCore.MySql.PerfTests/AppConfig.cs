using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace Pomelo.EntityFrameworkCore.MySql.PerfTests
{
    public static class AppConfig
    {
	    public static readonly bool AppVeyor = !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("APPVEYOR"));
        public static readonly string EfProvider = Environment.GetEnvironmentVariable("EF_PROVIDER")?.ToLower();
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
                            if (Ci != null && Ci == "true" && !File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "config.json"))){
                                InitCi();
                            }
                            var builder = new ConfigurationBuilder()
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("appsettings.json")
                                .AddJsonFile("config.json");
                            _config = builder.Build();
                        }
                    }
                }
                return _config;
            }
        }

        private static void InitCi(){
            File.Copy(
                Path.Combine(Directory.GetCurrentDirectory(), "config.json.example"),
                Path.Combine(Directory.GetCurrentDirectory(), "config.json"));

            var processInfo = new ProcessStartInfo{
                FileName = "dotnet",
                Arguments = "ef -c \"Ef\" migrations add initial",
                WorkingDirectory = Directory.GetCurrentDirectory()
            };
            var process = Process.Start(processInfo);
            process.WaitForExit();

            foreach (var filePath in Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "Migrations"))){
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
                Arguments = "ef -c \"Ef\" database update",
                WorkingDirectory = Directory.GetCurrentDirectory()
            };
            process = Process.Start(processInfo);
            process.WaitForExit();
        }
        
    }
}
