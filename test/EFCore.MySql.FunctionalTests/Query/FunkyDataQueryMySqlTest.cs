using System.Linq;
using System.Threading.Tasks;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestModels.FunkyDataModel;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query;

public class FunkyDataQueryMySqlTest : FunkyDataQueryTestBase<FunkyDataQueryMySqlTest.FunkyDataQueryMySqlFixture>
{
    public FunkyDataQueryMySqlTest(FunkyDataQueryMySqlFixture fixture)
        : base(fixture)
    {
        ClearLog();
        //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
    }

    public override async Task String_contains_on_argument_with_wildcard_constant(bool async)
    {
        await base.String_contains_on_argument_with_wildcard_constant(async);

        AssertSql(
"""
SELECT `f`.`FirstName`
FROM `FunkyCustomers` AS `f`
WHERE `f`.`FirstName` LIKE '%\\%B%'
""",
            //
            """
SELECT `f`.`FirstName`
FROM `FunkyCustomers` AS `f`
WHERE `f`.`FirstName` LIKE '%a\\_%'
""",
            //
            """
SELECT `f`.`FirstName`
FROM `FunkyCustomers` AS `f`
WHERE FALSE
""",
            //
            """
SELECT `f`.`FirstName`
FROM `FunkyCustomers` AS `f`
WHERE `f`.`FirstName` IS NOT NULL
""",
            //
            """
SELECT `f`.`FirstName`
FROM `FunkyCustomers` AS `f`
WHERE `f`.`FirstName` LIKE '%\\_Ba\\_%'
""",
            //
            """
SELECT `f`.`FirstName`
FROM `FunkyCustomers` AS `f`
WHERE `f`.`FirstName` NOT LIKE '%\\%B\\%a\\%r%' OR (`f`.`FirstName` IS NULL)
""",
            //
            """
SELECT `f`.`FirstName`
FROM `FunkyCustomers` AS `f`
WHERE `f`.`FirstName` IS NULL
""",
            //
            """
SELECT `f`.`FirstName`
FROM `FunkyCustomers` AS `f`
""");
    }

    public override async Task String_starts_with_on_argument_with_wildcard_constant(bool async)
    {
        await base.String_starts_with_on_argument_with_wildcard_constant(async);

        AssertSql(
"""
SELECT `f`.`FirstName`
FROM `FunkyCustomers` AS `f`
WHERE `f`.`FirstName` LIKE '\\%B%'
""",
            //
"""
SELECT `f`.`FirstName`
FROM `FunkyCustomers` AS `f`
WHERE `f`.`FirstName` LIKE '\\_B%'
""",
            //
"""
SELECT `f`.`FirstName`
FROM `FunkyCustomers` AS `f`
WHERE FALSE
""",
            //
"""
SELECT `f`.`FirstName`
FROM `FunkyCustomers` AS `f`
WHERE `f`.`FirstName` IS NOT NULL
""",
            //
"""
SELECT `f`.`FirstName`
FROM `FunkyCustomers` AS `f`
WHERE `f`.`FirstName` LIKE '\\_Ba\\_%'
""",
            //
"""
SELECT `f`.`FirstName`
FROM `FunkyCustomers` AS `f`
WHERE `f`.`FirstName` NOT LIKE '\\%B\\%a\\%r%' OR (`f`.`FirstName` IS NULL)
""",
            //
"""
SELECT `f`.`FirstName`
FROM `FunkyCustomers` AS `f`
WHERE `f`.`FirstName` IS NULL
""",
            //
"""
SELECT `f`.`FirstName`
FROM `FunkyCustomers` AS `f`
""");
    }

    public override async Task String_ends_with_on_argument_with_wildcard_constant(bool async)
    {
        await base.String_ends_with_on_argument_with_wildcard_constant(async);

        AssertSql(
"""
SELECT `f`.`FirstName`
FROM `FunkyCustomers` AS `f`
WHERE `f`.`FirstName` LIKE '%\\%r'
""",
            //
"""
SELECT `f`.`FirstName`
FROM `FunkyCustomers` AS `f`
WHERE `f`.`FirstName` LIKE '%r\\_'
""",
            //
"""
SELECT `f`.`FirstName`
FROM `FunkyCustomers` AS `f`
WHERE FALSE
""",
            //
"""
SELECT `f`.`FirstName`
FROM `FunkyCustomers` AS `f`
WHERE `f`.`FirstName` IS NOT NULL
""",
            //
"""
SELECT `f`.`FirstName`
FROM `FunkyCustomers` AS `f`
WHERE `f`.`FirstName` LIKE '%\\_r\\_'
""",
            //
"""
SELECT `f`.`FirstName`
FROM `FunkyCustomers` AS `f`
WHERE `f`.`FirstName` NOT LIKE '%a\\%r\\%' OR (`f`.`FirstName` IS NULL)
""",
            //
"""
SELECT `f`.`FirstName`
FROM `FunkyCustomers` AS `f`
WHERE `f`.`FirstName` IS NULL
""",
            //
"""
SELECT `f`.`FirstName`
FROM `FunkyCustomers` AS `f`
""");
    }

    public override Task String_Contains_and_StartsWith_with_same_parameter(bool async)
    {
        var s = "B";

        return AssertQuery(
            async,
            ss => ss.Set<FunkyCustomer>().Where(
                c => c.FirstName.Contains(s) || c.LastName.StartsWith(s)),
            ss => ss.Set<FunkyCustomer>().Where(
                c => c.FirstName.MaybeScalar(f => f.Contains(s)) == true || c.LastName.MaybeScalar(l => l.StartsWith(s)) == true));
    }

    protected override void ClearLog()
        => Fixture.TestSqlLoggerFactory.Clear();

    private void AssertSql(params string[] expected)
        => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);

    public class FunkyDataQueryMySqlFixture : FunkyDataQueryFixtureBase, ITestSqlLoggerFactory
    {
        public TestSqlLoggerFactory TestSqlLoggerFactory
            => (TestSqlLoggerFactory)ServiceProvider.GetRequiredService<ILoggerFactory>();

        protected override ITestStoreFactory TestStoreFactory
            => MySqlTestStoreFactory.Instance;
    }
}
