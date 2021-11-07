using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class ManyToManyHeterogeneousQueryMySqlTest : ManyToManyHeterogeneousQueryRelationalTestBase
    {
        protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;

        public override async Task Many_to_many_load_works_when_join_entity_has_custom_key(bool async)
        {
            await base.Many_to_many_load_works_when_join_entity_has_custom_key(async);

            AssertSql(
                @"@__p_0='1'

SELECT `m`.`Id`
FROM `ManyM_DB` AS `m`
WHERE `m`.`Id` = @__p_0
LIMIT 1",
                //
                @"@__p_0='1'

SELECT `t`.`Id`, `m`.`Id`, `t`.`Id0`, `t0`.`Id`, `t0`.`ManyM_Id`, `t0`.`ManyN_Id`, `t0`.`Id0`
FROM `ManyM_DB` AS `m`
INNER JOIN (
    SELECT `m1`.`Id`, `m0`.`Id` AS `Id0`, `m0`.`ManyM_Id`
    FROM `ManyMN_DB` AS `m0`
    LEFT JOIN `ManyN_DB` AS `m1` ON `m0`.`ManyN_Id` = `m1`.`Id`
) AS `t` ON `m`.`Id` = `t`.`ManyM_Id`
LEFT JOIN (
    SELECT `m2`.`Id`, `m2`.`ManyM_Id`, `m2`.`ManyN_Id`, `m3`.`Id` AS `Id0`
    FROM `ManyMN_DB` AS `m2`
    INNER JOIN `ManyM_DB` AS `m3` ON `m2`.`ManyM_Id` = `m3`.`Id`
    WHERE `m3`.`Id` = @__p_0
) AS `t0` ON `t`.`Id` = `t0`.`ManyN_Id`
WHERE `m`.`Id` = @__p_0
ORDER BY `m`.`Id`, `t`.`Id0`, `t`.`Id`, `t0`.`Id`",
                //
                @"@__p_0='1'

SELECT `m`.`Id`
FROM `ManyN_DB` AS `m`
WHERE `m`.`Id` = @__p_0
LIMIT 1",
                //
                @"@__p_0='1'

SELECT `t`.`Id`, `m`.`Id`, `t`.`Id0`, `t0`.`Id`, `t0`.`ManyM_Id`, `t0`.`ManyN_Id`, `t0`.`Id0`
FROM `ManyN_DB` AS `m`
INNER JOIN (
    SELECT `m1`.`Id`, `m0`.`Id` AS `Id0`, `m0`.`ManyN_Id`
    FROM `ManyMN_DB` AS `m0`
    INNER JOIN `ManyM_DB` AS `m1` ON `m0`.`ManyM_Id` = `m1`.`Id`
) AS `t` ON `m`.`Id` = `t`.`ManyN_Id`
LEFT JOIN (
    SELECT `m2`.`Id`, `m2`.`ManyM_Id`, `m2`.`ManyN_Id`, `m3`.`Id` AS `Id0`
    FROM `ManyMN_DB` AS `m2`
    INNER JOIN `ManyN_DB` AS `m3` ON `m2`.`ManyN_Id` = `m3`.`Id`
    WHERE `m3`.`Id` = @__p_0
) AS `t0` ON `t`.`Id` = `t0`.`ManyM_Id`
WHERE `m`.`Id` = @__p_0
ORDER BY `m`.`Id`, `t`.`Id0`, `t`.`Id`, `t0`.`Id`");
        }
    }
}
