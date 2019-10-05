using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities
{
    public class MySqlConnectionStringTestStoreFactory : RelationalTestStoreFactory
    {
        public static MySqlConnectionStringTestStoreFactory Instance { get; } = new MySqlConnectionStringTestStoreFactory();

        protected MySqlConnectionStringTestStoreFactory()
        {
        }

        public override TestStore Create(string storeName)
            => MySqlTestStore.Create(storeName, useConnectionString: true);

        public override TestStore GetOrCreate(string storeName)
            => MySqlTestStore.GetOrCreate(storeName, useConnectionString: true);

        public override IServiceCollection AddProviderServices(IServiceCollection serviceCollection)
            => serviceCollection.AddEntityFrameworkMySql();
    }
}
