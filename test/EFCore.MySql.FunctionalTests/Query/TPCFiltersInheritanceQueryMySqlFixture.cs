namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query;

public class TPCFiltersInheritanceQueryMySqlFixture : TPCInheritanceQueryMySqlFixture
{
    protected override bool EnableFilters
        => true;
}
