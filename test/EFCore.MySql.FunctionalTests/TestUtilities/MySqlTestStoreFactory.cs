using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities
{
    public class MySqlTestStoreFactory : RelationalTestStoreFactory
    {
        public static MySqlTestStoreFactory Instance { get; } = new MySqlTestStoreFactory();
        public static MySqlTestStoreFactory NoBackslashEscapesInstance { get; } = new MySqlTestStoreFactory(true);

        protected bool NoBackslashEscapes { get; }

        protected MySqlTestStoreFactory(bool noBackslashEscapes = false)
        {
            NoBackslashEscapes = noBackslashEscapes;
        }

        public override TestStore Create(string storeName)
            => MySqlTestStore.Create(storeName, noBackslashEscapes:NoBackslashEscapes);

        public override TestStore GetOrCreate(string storeName)
            => MySqlTestStore.GetOrCreate(storeName, noBackslashEscapes: NoBackslashEscapes);

        public override IServiceCollection AddProviderServices(IServiceCollection serviceCollection)
            => serviceCollection.AddEntityFrameworkMySql();
    }
}
