namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class FiltersInheritanceQueryMySqlFixture : InheritanceQueryMySqlFixture
    {
        protected override bool EnableFilters => true;
    }
}
