using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using MySqlConnector;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.Tests;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query;

public class AdHocQueryFiltersQueryMySqlTest : AdHocQueryFiltersQueryRelationalTestBase
{
    public override async Task Group_by_multiple_aggregate_joining_different_tables(bool async)
    {
        if (!AppConfig.ServerVersion.Supports.OuterReferenceInMultiLevelSubquery)
        {
            await Assert.ThrowsAsync<MySqlException>(() => base.Group_by_multiple_aggregate_joining_different_tables(async));
            return;
        }

        await base.Group_by_multiple_aggregate_joining_different_tables(async);

        AssertSql(
"""
SELECT (
    SELECT COUNT(*)
    FROM (
        SELECT DISTINCT `c`.`Value1`
        FROM (
            SELECT `p2`.`Child1Id`, 1 AS `Key`
            FROM `Parents` AS `p2`
        ) AS `p1`
        LEFT JOIN `Child1` AS `c` ON `p1`.`Child1Id` = `c`.`Id`
        WHERE `p0`.`Key` = `p1`.`Key`
    ) AS `s`) AS `Test1`, (
    SELECT COUNT(*)
    FROM (
        SELECT DISTINCT `c0`.`Value2`
        FROM (
            SELECT `p4`.`Child2Id`, 1 AS `Key`
            FROM `Parents` AS `p4`
        ) AS `p3`
        LEFT JOIN `Child2` AS `c0` ON `p3`.`Child2Id` = `c0`.`Id`
        WHERE `p0`.`Key` = `p3`.`Key`
    ) AS `s0`) AS `Test2`
FROM (
    SELECT 1 AS `Key`
    FROM `Parents` AS `p`
) AS `p0`
GROUP BY `p0`.`Key`
""");
    }

    public override async Task Group_by_multiple_aggregate_joining_different_tables_with_query_filter(bool async)
    {
        if (!AppConfig.ServerVersion.Supports.OuterReferenceInMultiLevelSubquery)
        {
            await Assert.ThrowsAsync<MySqlException>(() => base.Group_by_multiple_aggregate_joining_different_tables_with_query_filter(async));
            return;
        }

        await base.Group_by_multiple_aggregate_joining_different_tables_with_query_filter(async);

        AssertSql(
"""
SELECT (
    SELECT COUNT(*)
    FROM (
        SELECT DISTINCT `c0`.`Value1`
        FROM (
            SELECT `p2`.`ChildFilter1Id`, 1 AS `Key`
            FROM `Parents` AS `p2`
        ) AS `p1`
        LEFT JOIN (
            SELECT `c`.`Id`, `c`.`Value1`
            FROM `ChildFilter1` AS `c`
            WHERE `c`.`Filter1` = 'Filter1'
        ) AS `c0` ON `p1`.`ChildFilter1Id` = `c0`.`Id`
        WHERE `p0`.`Key` = `p1`.`Key`
    ) AS `s`) AS `Test1`, (
    SELECT COUNT(*)
    FROM (
        SELECT DISTINCT `c2`.`Value2`
        FROM (
            SELECT `p4`.`ChildFilter2Id`, 1 AS `Key`
            FROM `Parents` AS `p4`
        ) AS `p3`
        LEFT JOIN (
            SELECT `c1`.`Id`, `c1`.`Value2`
            FROM `ChildFilter2` AS `c1`
            WHERE `c1`.`Filter2` = 'Filter2'
        ) AS `c2` ON `p3`.`ChildFilter2Id` = `c2`.`Id`
        WHERE `p0`.`Key` = `p3`.`Key`
    ) AS `s0`) AS `Test2`
FROM (
    SELECT 1 AS `Key`
    FROM `Parents` AS `p`
) AS `p0`
GROUP BY `p0`.`Key`
""");
    }

    protected override ITestStoreFactory TestStoreFactory
        => MySqlTestStoreFactory.Instance;
}
