namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.BulkUpdates;

public class TPCFiltersInheritanceBulkUpdatesMySqlFixture : TPCInheritanceBulkUpdatesMySqlFixture
{
    protected override string StoreName
        => "TPCFiltersInheritanceBulkUpdatesTest";

    protected override bool EnableFilters
        => true;
}
