using Microsoft.EntityFrameworkCore.TestUtilities;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities
{
    public class MySqlNorthwindTestStoreFactory : MySqlTestStoreFactory
    {
        public static new MySqlNorthwindTestStoreFactory Instance { get; } = new MySqlNorthwindTestStoreFactory();
        public static new MySqlNorthwindTestStoreFactory NoBackslashEscapesInstance { get; } = new MySqlNorthwindTestStoreFactory(true);

        protected MySqlNorthwindTestStoreFactory(bool noBackslashEscapes = false)
            : base(noBackslashEscapes)
        {
        }

        public override TestStore GetOrCreate(string storeName)
            => MySqlTestStore.GetOrCreate(storeName, "Northwind.sql", noBackslashEscapes: NoBackslashEscapes);
    }
}
