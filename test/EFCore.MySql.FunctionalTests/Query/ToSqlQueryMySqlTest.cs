using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query;

public class ToSqlQueryMySqlTest : ToSqlQueryTestBase
{
    protected override ITestStoreFactory TestStoreFactory
        => MySqlTestStoreFactory.Instance;

    [ConditionalFact]
    public virtual void Check_all_tests_overridden()
        => TestHelpers.AssertAllMethodsOverridden(GetType());

    public override async Task Entity_type_with_navigation_mapped_to_SqlQuery(bool async)
    {
        await base.Entity_type_with_navigation_mapped_to_SqlQuery(async);

        AssertSql(
"""
SELECT `a`.`Id`, `a`.`Name`, `a`.`PostStatAuthorId`, `m`.`Count` AS `PostCount`
FROM `Authors` AS `a`
LEFT JOIN (
    SELECT * FROM PostStats
) AS `m` ON `a`.`PostStatAuthorId` = `m`.`AuthorId`
""");
    }

    private void AssertSql(params string[] expected)
        => TestSqlLoggerFactory.AssertBaseline(expected);
}
