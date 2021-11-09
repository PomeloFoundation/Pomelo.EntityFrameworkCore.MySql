using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;

// ReSharper disable InconsistentNaming
namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class SimpleQueryMySqlTest : SimpleQueryRelationalTestBase
    {
        protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;

        public override async Task Multiple_nested_reference_navigations(bool async)
        {
            await base.Multiple_nested_reference_navigations(async);

            AssertSql(
                @"@__p_0='3'

SELECT `s`.`Id`, `s`.`Email`, `s`.`Logon`, `s`.`ManagerId`, `s`.`Name`, `s`.`SecondaryManagerId`
FROM `Staff` AS `s`
WHERE `s`.`Id` = @__p_0
LIMIT 1",
                //
                @"@__id_0='1'

SELECT `a`.`Id`, `a`.`Complete`, `a`.`Deleted`, `a`.`PeriodEnd`, `a`.`PeriodStart`, `a`.`StaffId`, `s`.`Id`, `s`.`Email`, `s`.`Logon`, `s`.`ManagerId`, `s`.`Name`, `s`.`SecondaryManagerId`, `s0`.`Id`, `s0`.`Email`, `s0`.`Logon`, `s0`.`ManagerId`, `s0`.`Name`, `s0`.`SecondaryManagerId`, `s1`.`Id`, `s1`.`Email`, `s1`.`Logon`, `s1`.`ManagerId`, `s1`.`Name`, `s1`.`SecondaryManagerId`
FROM `Appraisals` AS `a`
INNER JOIN `Staff` AS `s` ON `a`.`StaffId` = `s`.`Id`
LEFT JOIN `Staff` AS `s0` ON `s`.`ManagerId` = `s0`.`Id`
LEFT JOIN `Staff` AS `s1` ON `s`.`SecondaryManagerId` = `s1`.`Id`
WHERE `a`.`Id` = @__id_0
LIMIT 2");
        }

        public override async Task Multiple_different_entity_type_from_different_namespaces(bool async)
        {
            var contextFactory = await InitializeAsync<Context23981>();
            using var context = contextFactory.CreateContext();
            var bad = context.Set<NameSpace1.TestQuery>().FromSqlRaw(@"SELECT cast(null as signed) AS MyValue").ToList(); // <-- MySQL uses `signed` instead of `int` in CAST() expressions
        }
    }
}
