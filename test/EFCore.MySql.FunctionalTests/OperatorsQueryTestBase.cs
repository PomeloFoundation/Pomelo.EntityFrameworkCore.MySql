using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using System.Threading.Tasks;

namespace Microsoft.EntityFrameworkCore.Query;

public class OperatorsQueryMySqlTest : OperatorsQueryTestBase
{
    protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;

    public override async Task Negate_on_binary_expression(bool async)
    {
        await base.Negate_on_binary_expression(async);

        AssertSql(
"""
SELECT `s`.`Id`, `s`.`Banner`, `s`.`Banner5`, `s`.`InternalNumber`, `s`.`Name`
FROM `Squads` AS `s`
WHERE `s`.`Id` = -(`s`.`Id` + `s`.`Id`)
""");
    }

    public override async Task Negate_on_column(bool async)
    {
        await base.Negate_on_column(async);

        AssertSql(
"""
SELECT `s`.`Id`, `s`.`Banner`, `s`.`Banner5`, `s`.`InternalNumber`, `s`.`Name`
FROM `Squads` AS `s`
WHERE `s`.`Id` = -`s`.`Id`
""");
    }

    public override async Task Double_negate_on_column()
    {
        await base.Double_negate_on_column();

        AssertSql(
"""
SELECT `s`.`Id`, `s`.`Banner`, `s`.`Banner5`, `s`.`InternalNumber`, `s`.`Name`
FROM `Squads` AS `s`
WHERE -(-`s`.`Id`) = `s`.`Id`
""");
    }

    public override async Task Negate_on_like_expression(bool async)
    {
        await base.Negate_on_like_expression(async);

        AssertSql(
"""
SELECT `s`.`Id`, `s`.`Banner`, `s`.`Banner5`, `s`.`InternalNumber`, `s`.`Name`
FROM `Squads` AS `s`
WHERE `s`.`Name` IS NOT NULL AND NOT (`s`.`Name` LIKE 'us%')
""");
    }

    protected void AssertSql(params string[] expected)
        => TestSqlLoggerFactory.AssertBaseline(expected);

}
