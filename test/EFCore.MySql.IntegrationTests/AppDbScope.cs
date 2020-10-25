using System;
using System.Data.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pomelo.EntityFrameworkCore.MySql.Tests;

namespace Pomelo.EntityFrameworkCore.MySql.IntegrationTests
{
    public class AppDbScope : IDisposable
    {
        private static ServiceProvider CreateServiceProvider(DbConnection connection = null)
        {
            var serviceCollection = new ServiceCollection();
                serviceCollection
                    .AddLogging(builder =>
                        builder
                            .AddConfiguration(AppConfig.Config.GetSection("Logging"))
                            .AddConsole()
                    );
                Startup.ConfigureEntityFramework(serviceCollection);

                return serviceCollection.BuildServiceProvider();
        }

        private static Lazy<ServiceProvider> DefaultLazyServiceProvider = new Lazy<ServiceProvider>(() => {
            return CreateServiceProvider();
        });

        private IServiceScope _scope;

        public AppDbScope()
        {
            var serviceProvider = DefaultLazyServiceProvider.Value;
            _scope = serviceProvider.CreateScope();
        }

        public AppDbScope(DbConnection connection = null)
        {
            var serviceProvider = CreateServiceProvider(connection);
            _scope = serviceProvider.CreateScope();
        }

        public AppDb AppDb => _scope.ServiceProvider.GetService<AppDb>();

        public void Dispose()
        {
            if (_scope != null)
            {
                _scope.Dispose();
                _scope = null;
            }
        }
    }
}
