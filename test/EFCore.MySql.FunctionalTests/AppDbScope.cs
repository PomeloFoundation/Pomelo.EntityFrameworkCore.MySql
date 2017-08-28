using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
{
    public class AppDbScope : IDisposable
    {
        private static ServiceProvider CreateServiceProvider(DbConnection connection = null)
        {
            var serviceCollection = new ServiceCollection();
                serviceCollection
                    .AddLogging();
                Startup.ConfigureEntityFramework(serviceCollection);

                var serviceProvider = serviceCollection.BuildServiceProvider();
                serviceProvider
                    .GetService<ILoggerFactory>()
                    .AddConsole(AppConfig.Config.GetSection("Logging"));
                return serviceProvider;
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
