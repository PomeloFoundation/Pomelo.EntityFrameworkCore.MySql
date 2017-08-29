using System;
using System.Buffers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Data.Common;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
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

	        services.AddIdentity<AppIdentityUser, IdentityRole>()
		        .AddEntityFrameworkStores<AppDb>()
		        .AddDefaultTokenProviders();
        }

        public static void ConfigureEntityFramework(IServiceCollection services, DbConnection connection = null)
        {
            if (connection == null)
            {
                services.AddDbContextPool<AppDb>(
                    options => options.UseMySql(AppConfig.Config["Data:ConnectionString"],
                        mysqlOptions => mysqlOptions.MaxBatchSize(AppConfig.EfBatchSize)));
            }
            else
            {
                services.AddDbContext<AppDb>(
                    options => options.UseMySql(connection,
                        mysqlOptions => mysqlOptions.MaxBatchSize(AppConfig.EfBatchSize)));
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
