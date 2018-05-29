using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities
{
    public class MySqlTestStoreFactory : ITestStoreFactory
    {
        public static MySqlTestStoreFactory Instance { get; } = new MySqlTestStoreFactory();

        protected MySqlTestStoreFactory()
        {
        }

        public virtual TestStore Create(string storeName)
            => MySqlTestStore.Create(storeName);

        public virtual TestStore GetOrCreate(string storeName)
            => MySqlTestStore.GetOrCreate(storeName);

        public IServiceCollection AddProviderServices(IServiceCollection serviceCollection)
            => serviceCollection.AddEntityFrameworkMySql()
                .AddSingleton<ILoggerFactory>(new TestSqlLoggerFactory());
    }
}
