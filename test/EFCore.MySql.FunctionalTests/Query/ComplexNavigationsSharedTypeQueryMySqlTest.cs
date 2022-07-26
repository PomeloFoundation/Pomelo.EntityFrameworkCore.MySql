using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestModels.ComplexNavigationsModel;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Tests.TestUtilities.Attributes;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class ComplexNavigationsSharedTypeQueryMySqlTest : ComplexNavigationsSharedTypeQueryRelationalTestBase<
        ComplexNavigationsSharedTypeQueryMySqlTest.ComplexNavigationsSharedTypeQueryMySqlFixture>
    {
        // ReSharper disable once UnusedParameter.Local
        public ComplexNavigationsSharedTypeQueryMySqlTest(
            ComplexNavigationsSharedTypeQueryMySqlFixture fixture,
            ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            // Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterReferenceInMultiLevelSubquery))]
        public override Task Contains_with_subquery_optional_navigation_and_constant_item(bool async)
        {
            return base.Contains_with_subquery_optional_navigation_and_constant_item(async);
        }

        public override Task Distinct_take_without_orderby(bool async)
        {
            return AssertQuery(
                async,
                ss => from l1 in ss.Set<Level1>()
                    where l1.Id < 3
                    select (from l3 in ss.Set<Level3>()
                        select l3).Distinct().OrderBy(e => e.Id).Take(1).FirstOrDefault().Name); // Apply OrderBy before Skip
        }

        public override Task Distinct_skip_without_orderby(bool async)
        {
            return AssertQuery(
                async,
                ss => from l1 in ss.Set<Level1>()
                    where l1.Id < 3
                    select (from l3 in ss.Set<Level3>()
                        select l3).Distinct().OrderBy(e => e.Id).Skip(1).FirstOrDefault().Name); // Apply OrderBy before Skip
        }

        public override async Task SelectMany_subquery_with_custom_projection(bool async)
        {
            // TODO: Fix test in EF Core upstream.
            //           ORDER BY `l`.`Id`
            //       is ambiguous, since all 5 queried rows have an `Id` of `1`.
            await AssertQuery(
                async,
                ss => ss.Set<Level1>()/*.OrderBy(l1 => l1.Id)*/.SelectMany( // <-- has no effect anymore
                        l1 => l1.OneToMany_Optional1.Select(
                            l2 => new { l2.Name }))
                    .OrderBy(e => e.Name) // <-- fix
                    .Take(1));

            AssertSql(
                @"@__p_0='1'

SELECT `t`.`Name`
FROM `Level1` AS `l`
INNER JOIN (
    SELECT `l0`.`Level2_Name` AS `Name`, `l0`.`OneToMany_Optional_Inverse2Id`
    FROM `Level1` AS `l0`
    INNER JOIN `Level1` AS `l1` ON `l0`.`Id` = `l1`.`Id`
    WHERE (`l0`.`OneToOne_Required_PK_Date` IS NOT NULL AND (`l0`.`Level1_Required_Id` IS NOT NULL)) AND `l0`.`OneToMany_Required_Inverse2Id` IS NOT NULL
) AS `t` ON `l`.`Id` = `t`.`OneToMany_Optional_Inverse2Id`
ORDER BY `t`.`Name`
LIMIT @__p_0");
        }

        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);

        public class ComplexNavigationsSharedTypeQueryMySqlFixture : ComplexNavigationsSharedTypeQueryRelationalFixtureBase
        {
            protected override ITestStoreFactory TestStoreFactory
                => MySqlTestStoreFactory.Instance;
        }
    }
}
