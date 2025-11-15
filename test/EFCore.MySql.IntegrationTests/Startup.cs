using System;
using Pomelo.EntityFrameworkCore.MySql.IntegrationTests.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using Newtonsoft.Json;
using Pomelo.EntityFrameworkCore.MySql.Tests;

namespace Pomelo.EntityFrameworkCore.MySql.IntegrationTests
{
    public static class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        public static void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services
                .AddMvc()
                .AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

            ConfigureEntityFramework(services);

            services
                .AddLogging(builder =>
                    builder
                        .AddConfiguration(AppConfig.Config.GetSection("Logging"))
                        .AddConsole()
                        .AddDebug()
                )
                .AddIdentity<AppIdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDb>()
                .AddDefaultTokenProviders();

            services.AddControllers();
        }

        public static void ConfigureEntityFramework(IServiceCollection services)
        {
            services.AddDbContextPool<AppDb>(
                options => options.UseMySql(
                    GetConnectionString(),
                    AppConfig.ServerVersion,
                    mysqlOptions =>
                    {
                        mysqlOptions.MaxBatchSize(AppConfig.EfBatchSize);
                        mysqlOptions.UseNewtonsoftJson();

                        if (AppConfig.EfRetryOnFailure > 0)
                        {
                            mysqlOptions.EnableRetryOnFailure(AppConfig.EfRetryOnFailure, TimeSpan.FromSeconds(5), null);
                        }
                    }
            ));
        }

        private static string GetConnectionString()
        {
            var csb = new MySqlConnectionStringBuilder(AppConfig.ConnectionString);

            if (AppConfig.EfDatabase != null)
            {
                csb.Database = AppConfig.EfDatabase;
            }

            return csb.ConnectionString;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
