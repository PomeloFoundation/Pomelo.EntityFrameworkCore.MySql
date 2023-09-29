namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class TPTFiltersInheritanceQueryMySqlFixture : TPTInheritanceQueryMySqlFixture
    {
        public override bool EnableFilters
            => true;
    }
}
