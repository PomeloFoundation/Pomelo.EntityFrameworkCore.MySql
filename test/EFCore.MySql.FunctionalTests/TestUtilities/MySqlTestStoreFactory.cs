using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities
{
    public class MySqlTestStoreFactory : ITestStoreFactory
    {
        public static MySqlTestStoreFactory Instance { get; } = new MySqlTestStoreFactory();
        public static MySqlTestStoreFactory NoBackslashEscapesInstance { get; } = new MySqlTestStoreFactory(true);

        private readonly bool _noBackslashEscapes;

        protected MySqlTestStoreFactory(bool noBackslashEscapes = false)
        {
            _noBackslashEscapes = noBackslashEscapes;
        }

        public virtual TestStore Create(string storeName)
            => MySqlTestStore.Create(storeName, noBackslashEscapes:_noBackslashEscapes);

        public virtual TestStore GetOrCreate(string storeName)
            => MySqlTestStore.GetOrCreate(storeName, noBackslashEscapes: _noBackslashEscapes);

        public IServiceCollection AddProviderServices(IServiceCollection serviceCollection)
            => serviceCollection.AddEntityFrameworkMySql()
                .AddSingleton<ILoggerFactory>(new TestSqlLoggerFactory());
    }
}
