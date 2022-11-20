namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.BulkUpdates;

public class FiltersInheritanceBulkUpdatesMySqlFixture : InheritanceBulkUpdatesMySqlFixture
{
    protected override string StoreName
        => "FiltersInheritanceBulkUpdatesTest";

    protected override bool EnableFilters
        => true;
}
