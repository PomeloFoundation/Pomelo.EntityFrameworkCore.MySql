using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities
{
    public class MySqlTestStoreFactory : RelationalTestStoreFactory
    {
        public static MySqlTestStoreFactory Instance { get; } = new MySqlTestStoreFactory();
        public static MySqlTestStoreFactory NoBackslashEscapesInstance { get; } = new MySqlTestStoreFactory(true);

        private readonly bool _noBackslashEscapes;

        protected MySqlTestStoreFactory(bool noBackslashEscapes = false)
        {
            _noBackslashEscapes = noBackslashEscapes;
        }

        public override TestStore Create(string storeName)
            => MySqlTestStore.Create(storeName, noBackslashEscapes:_noBackslashEscapes);

        public override TestStore GetOrCreate(string storeName)
            => MySqlTestStore.GetOrCreate(storeName, noBackslashEscapes: _noBackslashEscapes);

        public override IServiceCollection AddProviderServices(IServiceCollection serviceCollection)
            => serviceCollection.AddEntityFrameworkMySql();
    }
}
