using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;
using Pomelo.EntityFrameworkCore.MySql.Tests;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities
{
    public class MySqlTestStoreFactory : RelationalTestStoreFactory
    {
        public static MySqlTestStoreFactory Instance => InstanceCi;
        public static MySqlTestStoreFactory InstanceCi { get; } = new MySqlTestStoreFactory(databaseCollation: AppConfig.ServerVersion.DefaultUtf8CiCollation);
        public static MySqlTestStoreFactory InstanceCs { get; } = new MySqlTestStoreFactory(databaseCollation: AppConfig.ServerVersion.DefaultUtf8CsCollation);

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
            => serviceCollection
                .AddEntityFrameworkMySql()
                .AddEntityFrameworkMySqlNetTopologySuite();
    }
}
