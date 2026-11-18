namespace Microting.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class TPTFiltersInheritanceQueryMySqlFixture : TPTInheritanceQueryMySqlFixture
    {
        public override bool EnableFilters
            => true;
    }
}
