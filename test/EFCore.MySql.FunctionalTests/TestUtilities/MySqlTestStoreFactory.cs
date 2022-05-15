using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities
{
    public class MySqlTestStoreFactory : RelationalTestStoreFactory
    {
        public static MySqlTestStoreFactory Instance { get; } = new MySqlTestStoreFactory();
        public static MySqlTestStoreFactory NoBackslashEscapesInstance { get; } = new MySqlTestStoreFactory(noBackslashEscapes: true);
        public static MySqlTestStoreFactory GuidBinary16Instance { get; } = new MySqlTestStoreFactory(guidFormat: MySqlGuidFormat.Binary16);

        protected bool NoBackslashEscapes { get; }
        protected string DatabaseCollation { get; }
        protected MySqlGuidFormat GuidFormat { get; }

        protected MySqlTestStoreFactory(
            bool noBackslashEscapes = false,
            string databaseCollation = null,
            MySqlGuidFormat guidFormat = MySqlGuidFormat.Default)
        {
            NoBackslashEscapes = noBackslashEscapes;
            DatabaseCollation = databaseCollation;
            GuidFormat = guidFormat;
        }

        public override TestStore Create(string storeName)
            => MySqlTestStore.Create(storeName, noBackslashEscapes: NoBackslashEscapes, databaseCollation: DatabaseCollation, guidFormat: GuidFormat);

        public override TestStore GetOrCreate(string storeName)
            => MySqlTestStore.GetOrCreate(storeName, noBackslashEscapes: NoBackslashEscapes, databaseCollation: DatabaseCollation, guidFormat: GuidFormat);

        public override IServiceCollection AddProviderServices(IServiceCollection serviceCollection)
            => serviceCollection.AddEntityFrameworkMySql();
    }
}
