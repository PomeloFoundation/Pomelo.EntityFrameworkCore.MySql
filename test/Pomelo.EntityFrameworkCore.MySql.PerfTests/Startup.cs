using System.Buffers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MySQL.Data.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Pomelo.EntityFrameworkCore.MySql.PerfTests
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            Configuration = AppConfig.Config;
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc(options =>
            {
                options.OutputFormatters.Clear();
                options.OutputFormatters.Add(new JsonOutputFormatter(new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                }, ArrayPool<char>.Shared));
            });
            ConfigureEntityFramework(services);
        }

        public void ConfigureEntityFramework(IServiceCollection services)
        {
            if (AppConfig.EfProvider == "oracle")
            {
                // Oracle defines this with a case sensitive "MySQL"
                System.Console.WriteLine("Using Oracle Provider");
                services.AddEntityFrameworkMySQL();
            }
            else
            {
                // Pomelo defines this with a case sensitive "MySql" in Microsoft.Extensions.DependencyInjection
                System.Console.WriteLine("Using Pomelo Provider");
                services.AddEntityFrameworkMySql();
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseMvc();
        }
    }
}