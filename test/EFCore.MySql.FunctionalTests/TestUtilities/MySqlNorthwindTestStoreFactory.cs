using Microsoft.EntityFrameworkCore.TestUtilities;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities
{
    public class MySqlNorthwindTestStoreFactory : MySqlTestStoreFactory
    {
        public static new MySqlNorthwindTestStoreFactory Instance => InstanceCi;
        public static MySqlNorthwindTestStoreFactory InstanceCi { get; } = new MySqlNorthwindTestStoreFactory(databaseCollation: MySqlTestStore.ModernCiCollation);
        public static MySqlNorthwindTestStoreFactory InstanceCs { get; } = new MySqlNorthwindTestStoreFactory(databaseCollation: MySqlTestStore.ModernCsCollation);
        public static new MySqlNorthwindTestStoreFactory NoBackslashEscapesInstance { get; } = new MySqlNorthwindTestStoreFactory(true);

        protected MySqlNorthwindTestStoreFactory(bool noBackslashEscapes = false, string databaseCollation = null)
            : base(noBackslashEscapes, databaseCollation)
        {
        }

        public override TestStore GetOrCreate(string storeName)
            => MySqlTestStore.GetOrCreate(storeName, "Northwind.sql", noBackslashEscapes: NoBackslashEscapes, databaseCollation: DatabaseCollation);
    }
}
