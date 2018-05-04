using Microsoft.EntityFrameworkCore.Query;

namespace EFCore.MySql.UpstreamFunctionalTests.Query
{
    public class FiltersInheritanceMySqlTest : FiltersInheritanceTestBase<FiltersInheritanceMySqlFixture>
    {
        public FiltersInheritanceMySqlTest(FiltersInheritanceMySqlFixture fixture)
            : base(fixture)
        {
        }
    }
}
