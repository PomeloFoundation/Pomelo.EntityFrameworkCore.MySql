namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class FiltersInheritanceMySqlFixture : InheritanceMySqlFixture
    {
        protected override bool EnableFilters => true;
    }
}
