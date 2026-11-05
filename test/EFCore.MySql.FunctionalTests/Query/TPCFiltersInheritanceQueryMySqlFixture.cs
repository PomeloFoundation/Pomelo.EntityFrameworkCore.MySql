namespace Microting.EntityFrameworkCore.MySql.FunctionalTests.Query;

public class TPCFiltersInheritanceQueryMySqlFixture : TPCInheritanceQueryMySqlFixture
{
    public override bool EnableFilters
        => true;
}
