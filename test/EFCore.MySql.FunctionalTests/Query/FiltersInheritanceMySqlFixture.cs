namespace EFCore.MySql.UpstreamFunctionalTests.Query
{
    public class FiltersInheritanceMySqlFixture : InheritanceMySqlFixture
    {
        protected override bool EnableFilters => true;
    }
}
