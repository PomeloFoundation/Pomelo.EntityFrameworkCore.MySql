namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.BulkUpdates;

public class TPTFiltersInheritanceBulkUpdatesMySqlFixture : TPTInheritanceBulkUpdatesMySqlFixture
{
    protected override string StoreName
        => "TPTFiltersInheritanceBulkUpdatesTest";

    protected override bool EnableFilters
        => true;
}
