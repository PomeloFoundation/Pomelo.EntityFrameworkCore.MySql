namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.BulkUpdates;

public class TPHFiltersInheritanceBulkUpdatesMySqlFixture : TPHInheritanceBulkUpdatesMySqlFixture
{
    protected override string StoreName
        => "FiltersInheritanceBulkUpdatesTest";

    public override bool EnableFilters
        => true;
}
