namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class TPTFiltersInheritanceQueryMySqlFixture : TPTInheritanceQueryMySqlFixture
    {
        protected override bool EnableFilters
            => true;
    }
}
