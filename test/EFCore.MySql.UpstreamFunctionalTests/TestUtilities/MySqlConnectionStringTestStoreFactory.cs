using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EFCore.MySql.UpstreamFunctionalTests.TestUtilities
{
    public class MySqlConnectionStringTestStoreFactory : ITestStoreFactory
    {
        public static MySqlConnectionStringTestStoreFactory Instance { get; } = new MySqlConnectionStringTestStoreFactory();

        protected MySqlConnectionStringTestStoreFactory()
        {
        }

        public virtual TestStore Create(string storeName)
            => MySqlTestStore.Create(storeName, useConnectionString: true);

        public virtual TestStore GetOrCreate(string storeName)
            => MySqlTestStore.GetOrCreate(storeName, useConnectionString: true);

        public IServiceCollection AddProviderServices(IServiceCollection serviceCollection)
            => serviceCollection.AddEntityFrameworkMySql()
                .AddSingleton<ILoggerFactory>(new TestSqlLoggerFactory());
    }
}
