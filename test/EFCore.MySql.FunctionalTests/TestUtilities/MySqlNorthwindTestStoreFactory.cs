using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.Tests;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities
{
    public class MySqlNorthwindTestStoreFactory : MySqlTestStoreFactory
    {
        public const string DefaultName = "Northwind";

        public static new MySqlNorthwindTestStoreFactory Instance => InstanceCi;
        public static new MySqlNorthwindTestStoreFactory InstanceCi { get; } = new MySqlNorthwindTestStoreFactory(databaseCollation: AppConfig.ServerVersion.DefaultUtf8CiCollation);
        public static new MySqlNorthwindTestStoreFactory InstanceCs { get; } = new MySqlNorthwindTestStoreFactory(databaseCollation: AppConfig.ServerVersion.DefaultUtf8CsCollation);
        public static new MySqlNorthwindTestStoreFactory NoBackslashEscapesInstance { get; } = new MySqlNorthwindTestStoreFactory(true);

        protected MySqlNorthwindTestStoreFactory(bool noBackslashEscapes = false, string databaseCollation = null)
            : base(noBackslashEscapes, databaseCollation)
        {
        }

        public override TestStore GetOrCreate(string storeName)
            => MySqlTestStore.GetOrCreate(storeName ?? DefaultName, "Northwind.sql", noBackslashEscapes: NoBackslashEscapes, databaseCollation: DatabaseCollation);
    }
}
