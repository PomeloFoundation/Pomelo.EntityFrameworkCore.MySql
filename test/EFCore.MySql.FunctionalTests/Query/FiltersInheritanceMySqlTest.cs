using Microsoft.EntityFrameworkCore.Query;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class FiltersInheritanceMySqlTest : FiltersInheritanceTestBase<FiltersInheritanceMySqlFixture>
    {
        public FiltersInheritanceMySqlTest(FiltersInheritanceMySqlFixture fixture)
            : base(fixture)
        {
        }
    }
}
