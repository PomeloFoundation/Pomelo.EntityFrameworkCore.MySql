using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
{
    public abstract class ServiceProviderPerContextFixtureBase<TContext> : ServiceProviderFixtureBase, IAsyncLifetime
        where TContext : DbContext
    {
        protected abstract string StoreName { get; }
        public TestStore TestStore { get; private set; }
        public TestSqlLoggerFactory TestSqlLoggerFactory => (TestSqlLoggerFactory)ListLoggerFactory;

        public async Task InitializeAsync()
        {
            TestStore = TestStoreFactory.GetOrCreate(StoreName);

            // Setup database.
            var serviceProvider = SetupServices();
            await TestStore.InitializeAsync(serviceProvider, () => CreateContextFromServiceProvider(serviceProvider), c => SeedAsync((TContext)c), CleanAsync);
        }

        public Task DisposeAsync()
            => Task.CompletedTask;

        // We cannot use ServiceProviderFixtureBase.CreateOptions() here, because it does not accept an existing
        // DbContextOptionsBuilder or DbContextOptions object as a parameter, and we might already have one setup.
        protected virtual DbContextOptionsBuilder ConfigureOptions(IServiceProvider serviceProvider, DbContextOptionsBuilder optionsBuilder)
            => AddOptions(TestStore.AddProviderOptions(optionsBuilder))
                .EnableDetailedErrors()
                .UseInternalServiceProvider(serviceProvider)
                .EnableServiceProviderCaching(false);

        public virtual TContext CreateContext(
            Func<IServiceCollection, IServiceCollection> addServices = null,
            Func<DbContextOptionsBuilder, DbContextOptionsBuilder> addOptions = null,
            IServiceCollection serviceCollection = null)
        {
            var serviceProvider = SetupServices(addServices, addOptions, serviceCollection);
            return CreateContextFromServiceProvider(serviceProvider);
        }

        protected virtual IServiceProvider SetupServices(
            Func<IServiceCollection, IServiceCollection> addServices = null,
            Func<DbContextOptionsBuilder, DbContextOptionsBuilder> addOptions = null,
            IServiceCollection serviceCollection = null)
        {
            serviceCollection = AddServices(TestStoreFactory.AddProviderServices(serviceCollection ?? new ServiceCollection()))
                .AddDbContext(
                    typeof(TContext),
                    optionsAction: (s, b) =>
                    {
                        var optionsBuilder = ConfigureOptions(s, b);
                        addOptions?.Invoke(optionsBuilder);
                    },
                    ServiceLifetime.Transient,
                    ServiceLifetime.Transient);

            serviceCollection = addServices != null
                ? addServices.Invoke(serviceCollection)
                : serviceCollection;

            return serviceCollection.BuildServiceProvider(validateScopes: true);
        }

        public virtual TContext CreateContextFromServiceProvider(IServiceProvider serviceProvider)
            => (TContext)serviceProvider.GetRequiredService(typeof(TContext));

        protected virtual Task CleanAsync(DbContext context)
            => Task.CompletedTask;

        protected virtual Task SeedAsync(TContext context)
            => Task.CompletedTask;
    }
}
