using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.BulkUpdates;
using Microsoft.EntityFrameworkCore.TestModels.Northwind;
using Microsoft.EntityFrameworkCore.TestUtilities;
using MySqlConnector;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Tests;
using Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.BulkUpdates;

public class NorthwindBulkUpdatesMySqlTest : NorthwindBulkUpdatesTestBase<NorthwindBulkUpdatesMySqlFixture<NoopModelCustomizer>>
{
    public NorthwindBulkUpdatesMySqlTest(
        NorthwindBulkUpdatesMySqlFixture<NoopModelCustomizer> fixture,
        ITestOutputHelper testOutputHelper)
        : base(fixture, testOutputHelper)
    {
        ClearLog();
        Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
    }

    [ConditionalFact]
    public virtual void Check_all_tests_overridden()
        => TestHelpers.AssertAllMethodsOverridden(GetType());

    public override async Task Delete_Where_TagWith(bool async)
    {
        await base.Delete_Where_TagWith(async);

        AssertSql(
"""
-- MyDelete

DELETE `o`
FROM `Order Details` AS `o`
WHERE `o`.`OrderID` < 10300
""");
    }

    public override async Task Delete_Where(bool async)
    {
        await base.Delete_Where(async);

        AssertSql(
"""
DELETE `o`
FROM `Order Details` AS `o`
WHERE `o`.`OrderID` < 10300
""");
    }

    public override async Task Delete_Where_parameter(bool async)
    {
        await base.Delete_Where_parameter(async);

        AssertSql(
"""
@__quantity_0='1' (Nullable = true) (DbType = Int16)

DELETE `o`
FROM `Order Details` AS `o`
WHERE `o`.`Quantity` = @__quantity_0
""",
                //
                """
DELETE `o`
FROM `Order Details` AS `o`
WHERE FALSE
""");
    }

    public override async Task Delete_Where_OrderBy(bool async)
    {
        await base.Delete_Where_OrderBy(async);

        AssertSql(
"""
DELETE
FROM `Order Details`
WHERE `OrderID` < 10300
ORDER BY `OrderID`
""");
    }

    public override async Task Delete_Where_OrderBy_Skip(bool async)
    {
        await base.Delete_Where_OrderBy_Skip(async);

        AssertSql(
"""
@__p_0='100'

DELETE `o`
FROM `Order Details` AS `o`
WHERE EXISTS (
    SELECT 1
    FROM (
        SELECT `o0`.`OrderID`, `o0`.`ProductID`
        FROM `Order Details` AS `o0`
        WHERE `o0`.`OrderID` < 10300
        ORDER BY `o0`.`OrderID`
        LIMIT 18446744073709551610 OFFSET @__p_0
    ) AS `o1`
    WHERE (`o1`.`OrderID` = `o`.`OrderID`) AND (`o1`.`ProductID` = `o`.`ProductID`))
""");
    }

    public override async Task Delete_Where_OrderBy_Take(bool async)
    {
        await base.Delete_Where_OrderBy_Take(async);

        AssertSql(
"""
@__p_0='100'

DELETE
FROM `Order Details`
WHERE `OrderID` < 10300
ORDER BY `OrderID`
LIMIT @__p_0
""");
    }

    public override async Task Delete_Where_OrderBy_Skip_Take(bool async)
    {
        await base.Delete_Where_OrderBy_Skip_Take(async);

        AssertSql(
"""
@__p_0='100'

DELETE `o`
FROM `Order Details` AS `o`
WHERE EXISTS (
    SELECT 1
    FROM (
        SELECT `o0`.`OrderID`, `o0`.`ProductID`
        FROM `Order Details` AS `o0`
        WHERE `o0`.`OrderID` < 10300
        ORDER BY `o0`.`OrderID`
        LIMIT @__p_0 OFFSET @__p_0
    ) AS `o1`
    WHERE (`o1`.`OrderID` = `o`.`OrderID`) AND (`o1`.`ProductID` = `o`.`ProductID`))
""");
    }

    public override async Task Delete_Where_Skip(bool async)
    {
        await base.Delete_Where_Skip(async);

        AssertSql(
"""
@__p_0='100'

DELETE `o`
FROM `Order Details` AS `o`
WHERE EXISTS (
    SELECT 1
    FROM (
        SELECT `o0`.`OrderID`, `o0`.`ProductID`
        FROM `Order Details` AS `o0`
        WHERE `o0`.`OrderID` < 10300
        LIMIT 18446744073709551610 OFFSET @__p_0
    ) AS `o1`
    WHERE (`o1`.`OrderID` = `o`.`OrderID`) AND (`o1`.`ProductID` = `o`.`ProductID`))
""");
    }

    public override async Task Delete_Where_Take(bool async)
    {
        await base.Delete_Where_Take(async);

        AssertSql(
"""
@__p_0='100'

DELETE
FROM `Order Details`
WHERE `OrderID` < 10300
LIMIT @__p_0
""");
    }

    public override async Task Delete_Where_Skip_Take(bool async)
    {
        await base.Delete_Where_Skip_Take(async);

        AssertSql(
"""
@__p_0='100'

DELETE `o`
FROM `Order Details` AS `o`
WHERE EXISTS (
    SELECT 1
    FROM (
        SELECT `o0`.`OrderID`, `o0`.`ProductID`
        FROM `Order Details` AS `o0`
        WHERE `o0`.`OrderID` < 10300
        LIMIT @__p_0 OFFSET @__p_0
    ) AS `o1`
    WHERE (`o1`.`OrderID` = `o`.`OrderID`) AND (`o1`.`ProductID` = `o`.`ProductID`))
""");
    }

    public override async Task Delete_Where_predicate_with_GroupBy_aggregate(bool async)
    {
        await base.Delete_Where_predicate_with_GroupBy_aggregate(async);

        AssertSql(
"""
DELETE `o`
FROM `Order Details` AS `o`
WHERE `o`.`OrderID` < (
    SELECT (
        SELECT `o1`.`OrderID`
        FROM `Orders` AS `o1`
        WHERE (`o0`.`CustomerID` = `o1`.`CustomerID`) OR (`o0`.`CustomerID` IS NULL AND (`o1`.`CustomerID` IS NULL))
        LIMIT 1)
    FROM `Orders` AS `o0`
    GROUP BY `o0`.`CustomerID`
    HAVING COUNT(*) > 11
    LIMIT 1)
""");
    }

    public override async Task Delete_Where_predicate_with_GroupBy_aggregate_2(bool async)
    {
        await base.Delete_Where_predicate_with_GroupBy_aggregate_2(async);

        AssertSql(
"""
DELETE `o`
FROM `Order Details` AS `o`
INNER JOIN `Orders` AS `o0` ON `o`.`OrderID` = `o0`.`OrderID`
WHERE `o0`.`OrderID` IN (
    SELECT (
        SELECT `o2`.`OrderID`
        FROM `Orders` AS `o2`
        WHERE (`o1`.`CustomerID` = `o2`.`CustomerID`) OR (`o1`.`CustomerID` IS NULL AND (`o2`.`CustomerID` IS NULL))
        LIMIT 1)
    FROM `Orders` AS `o1`
    GROUP BY `o1`.`CustomerID`
    HAVING COUNT(*) > 9
)
""");
    }

    public override async Task Delete_GroupBy_Where_Select(bool async)
    {
        await base.Delete_GroupBy_Where_Select(async);

        AssertSql();
    }

    public override async Task Delete_GroupBy_Where_Select_2(bool async)
    {
        await base.Delete_GroupBy_Where_Select_2(async);

        AssertSql();
    }

    public override async Task Delete_Where_Skip_Take_Skip_Take_causing_subquery(bool async)
    {
        await base.Delete_Where_Skip_Take_Skip_Take_causing_subquery(async);

        AssertSql(
"""
@__p_0='100'
@__p_2='5'
@__p_1='20'

DELETE `o`
FROM `Order Details` AS `o`
WHERE EXISTS (
    SELECT 1
    FROM (
        SELECT `o0`.`OrderID`, `o0`.`ProductID`
        FROM (
            SELECT `o1`.`OrderID`, `o1`.`ProductID`
            FROM `Order Details` AS `o1`
            WHERE `o1`.`OrderID` < 10300
            LIMIT @__p_0 OFFSET @__p_0
        ) AS `o0`
        LIMIT @__p_2 OFFSET @__p_1
    ) AS `o2`
    WHERE (`o2`.`OrderID` = `o`.`OrderID`) AND (`o2`.`ProductID` = `o`.`ProductID`))
""");
    }

    public override async Task Delete_Where_Distinct(bool async)
    {
        await base.Delete_Where_Distinct(async);

        AssertSql(
"""
DELETE `o`
FROM `Order Details` AS `o`
WHERE `o`.`OrderID` < 10300
""");
    }

    public override async Task Delete_SelectMany(bool async)
    {
        await base.Delete_SelectMany(async);

        AssertSql(
"""
DELETE `o0`
FROM `Orders` AS `o`
INNER JOIN `Order Details` AS `o0` ON `o`.`OrderID` = `o0`.`OrderID`
WHERE `o`.`OrderID` < 10250
""");
    }

    public override async Task Delete_SelectMany_subquery(bool async)
    {
        await base.Delete_SelectMany_subquery(async);

        AssertSql(
"""
DELETE `o`
FROM `Order Details` AS `o`
WHERE EXISTS (
    SELECT 1
    FROM `Orders` AS `o0`
    INNER JOIN (
        SELECT `o2`.`OrderID`, `o2`.`ProductID`
        FROM `Order Details` AS `o2`
        WHERE `o2`.`ProductID` > 0
    ) AS `o1` ON `o0`.`OrderID` = `o1`.`OrderID`
    WHERE (`o0`.`OrderID` < 10250) AND ((`o1`.`OrderID` = `o`.`OrderID`) AND (`o1`.`ProductID` = `o`.`ProductID`)))
""");
    }

    public override async Task Delete_Where_using_navigation(bool async)
    {
        await base.Delete_Where_using_navigation(async);

        AssertSql(
"""
DELETE `o`
FROM `Order Details` AS `o`
INNER JOIN `Orders` AS `o0` ON `o`.`OrderID` = `o0`.`OrderID`
WHERE EXTRACT(year FROM `o0`.`OrderDate`) = 2000
""");
    }

    public override async Task Delete_Where_using_navigation_2(bool async)
    {
        await base.Delete_Where_using_navigation_2(async);
        AssertSql(
"""
DELETE `o`
FROM `Order Details` AS `o`
INNER JOIN `Orders` AS `o0` ON `o`.`OrderID` = `o0`.`OrderID`
LEFT JOIN `Customers` AS `c` ON `o0`.`CustomerID` = `c`.`CustomerID`
WHERE `c`.`CustomerID` LIKE 'F%'
""");
    }

    public override async Task Delete_Union(bool async)
    {
        await base.Delete_Union(async);

        AssertSql(
"""
DELETE `o`
FROM `Order Details` AS `o`
WHERE EXISTS (
    SELECT 1
    FROM (
        SELECT `o0`.`OrderID`, `o0`.`ProductID`, `o0`.`Discount`, `o0`.`Quantity`, `o0`.`UnitPrice`
        FROM `Order Details` AS `o0`
        WHERE `o0`.`OrderID` < 10250
        UNION
        SELECT `o1`.`OrderID`, `o1`.`ProductID`, `o1`.`Discount`, `o1`.`Quantity`, `o1`.`UnitPrice`
        FROM `Order Details` AS `o1`
        WHERE `o1`.`OrderID` > 11250
    ) AS `u`
    WHERE (`u`.`OrderID` = `o`.`OrderID`) AND (`u`.`ProductID` = `o`.`ProductID`))
""");
    }

    public override async Task Delete_Concat(bool async)
    {
        await base.Delete_Concat(async);

        AssertSql(
"""
DELETE `o`
FROM `Order Details` AS `o`
WHERE EXISTS (
    SELECT 1
    FROM (
        SELECT `o0`.`OrderID`, `o0`.`ProductID`
        FROM `Order Details` AS `o0`
        WHERE `o0`.`OrderID` < 10250
        UNION ALL
        SELECT `o1`.`OrderID`, `o1`.`ProductID`
        FROM `Order Details` AS `o1`
        WHERE `o1`.`OrderID` > 11250
    ) AS `u`
    WHERE (`u`.`OrderID` = `o`.`OrderID`) AND (`u`.`ProductID` = `o`.`ProductID`))
""");
    }

    public override async Task Delete_Intersect(bool async)
    {
        await base.Delete_Intersect(async);

        AssertSql(
"""
DELETE `o`
FROM `Order Details` AS `o`
WHERE EXISTS (
    SELECT 1
    FROM (
        SELECT `o0`.`OrderID`, `o0`.`ProductID`, `o0`.`Discount`, `o0`.`Quantity`, `o0`.`UnitPrice`
        FROM `Order Details` AS `o0`
        WHERE `o0`.`OrderID` < 10250
        INTERSECT
        SELECT `o1`.`OrderID`, `o1`.`ProductID`, `o1`.`Discount`, `o1`.`Quantity`, `o1`.`UnitPrice`
        FROM `Order Details` AS `o1`
        WHERE `o1`.`OrderID` > 11250
    ) AS `i`
    WHERE (`i`.`OrderID` = `o`.`OrderID`) AND (`i`.`ProductID` = `o`.`ProductID`))
""");
    }

    public override async Task Delete_Except(bool async)
    {
        await base.Delete_Except(async);

        AssertSql(
"""
DELETE `o`
FROM `Order Details` AS `o`
WHERE EXISTS (
    SELECT 1
    FROM (
        SELECT `o0`.`OrderID`, `o0`.`ProductID`, `o0`.`Discount`, `o0`.`Quantity`, `o0`.`UnitPrice`
        FROM `Order Details` AS `o0`
        WHERE `o0`.`OrderID` < 10250
        EXCEPT
        SELECT `o1`.`OrderID`, `o1`.`ProductID`, `o1`.`Discount`, `o1`.`Quantity`, `o1`.`UnitPrice`
        FROM `Order Details` AS `o1`
        WHERE `o1`.`OrderID` > 11250
    ) AS `e`
    WHERE (`e`.`OrderID` = `o`.`OrderID`) AND (`e`.`ProductID` = `o`.`ProductID`))
""");
    }

    public override async Task Delete_non_entity_projection(bool async)
    {
        await base.Delete_non_entity_projection(async);

        AssertSql();
    }

    public override async Task Delete_non_entity_projection_2(bool async)
    {
        await base.Delete_non_entity_projection_2(async);

        AssertSql();
    }

    public override async Task Delete_non_entity_projection_3(bool async)
    {
        await base.Delete_non_entity_projection_3(async);

        AssertSql();
    }

    public override async Task Delete_FromSql_converted_to_subquery(bool async)
    {
        await base.Delete_FromSql_converted_to_subquery(async);

        AssertSql(
"""
DELETE `o`
FROM `Order Details` AS `o`
WHERE EXISTS (
    SELECT 1
    FROM (
        SELECT `OrderID`, `ProductID`, `UnitPrice`, `Quantity`, `Discount`
        FROM `Order Details`
        WHERE `OrderID` < 10300
    ) AS `m`
    WHERE (`m`.`OrderID` = `o`.`OrderID`) AND (`m`.`ProductID` = `o`.`ProductID`))
""");
    }

    public override async Task Delete_Where_optional_navigation_predicate(bool async)
    {
        await base.Delete_Where_optional_navigation_predicate(async);
        AssertSql(
"""
DELETE `o`
FROM `Order Details` AS `o`
INNER JOIN `Orders` AS `o0` ON `o`.`OrderID` = `o0`.`OrderID`
LEFT JOIN `Customers` AS `c` ON `o0`.`CustomerID` = `c`.`CustomerID`
WHERE `c`.`City` LIKE 'Se%'
""");
    }

    public override async Task Delete_with_join(bool async)
    {
        await base.Delete_with_join(async);

        AssertSql(
"""
@__p_1='100'
@__p_0='0'

DELETE `o`
FROM `Order Details` AS `o`
INNER JOIN (
    SELECT `o0`.`OrderID`
    FROM `Orders` AS `o0`
    WHERE `o0`.`OrderID` < 10300
    ORDER BY `o0`.`OrderID`
    LIMIT @__p_1 OFFSET @__p_0
) AS `o1` ON `o`.`OrderID` = `o1`.`OrderID`
""");
    }

    public override async Task Delete_with_left_join(bool async)
    {
        await base.Delete_with_left_join(async);

        AssertSql(
"""
@__p_1='100'
@__p_0='0'

DELETE `o`
FROM `Order Details` AS `o`
LEFT JOIN (
    SELECT `o0`.`OrderID`
    FROM `Orders` AS `o0`
    WHERE `o0`.`OrderID` < 10300
    ORDER BY `o0`.`OrderID`
    LIMIT @__p_1 OFFSET @__p_0
) AS `o1` ON `o`.`OrderID` = `o1`.`OrderID`
WHERE `o`.`OrderID` < 10276
""");
    }

    public override async Task Delete_with_cross_join(bool async)
    {
        await base.Delete_with_cross_join(async);

        AssertSql(
"""
DELETE `o`
FROM `Order Details` AS `o`
CROSS JOIN (
    SELECT 1
    FROM `Orders` AS `o0`
    WHERE `o0`.`OrderID` < 10300
    ORDER BY `o0`.`OrderID`
    LIMIT 100 OFFSET 0
) AS `o1`
WHERE `o`.`OrderID` < 10276
""");
    }

    public override async Task Delete_with_cross_apply(bool async)
    {
        await base.Delete_with_cross_apply(async);

        AssertSql(
"""
DELETE `o`
FROM `Order Details` AS `o`
JOIN LATERAL (
    SELECT 1
    FROM `Orders` AS `o0`
    WHERE `o0`.`OrderID` < `o`.`OrderID`
    ORDER BY `o0`.`OrderID`
    LIMIT 100 OFFSET 0
) AS `o1` ON TRUE
WHERE `o`.`OrderID` < 10276
""");
    }

    public override async Task Delete_with_outer_apply(bool async)
    {
        await base.Delete_with_outer_apply(async);

        AssertSql(
"""
DELETE `o`
FROM `Order Details` AS `o`
LEFT JOIN LATERAL (
    SELECT 1
    FROM `Orders` AS `o0`
    WHERE `o0`.`OrderID` < `o`.`OrderID`
    ORDER BY `o0`.`OrderID`
    LIMIT 100 OFFSET 0
) AS `o1` ON TRUE
WHERE `o`.`OrderID` < 10276
""");
    }

    public override async Task Update_Where_set_constant_TagWith(bool async)
    {
        await base.Update_Where_set_constant_TagWith(async);

        AssertExecuteUpdateSql(
"""
-- MyUpdate

UPDATE `Customers` AS `c`
SET `c`.`ContactName` = 'Updated'
WHERE `c`.`CustomerID` LIKE 'F%'
""");
    }

    public override async Task Update_Where_set_constant(bool async)
    {
        await base.Update_Where_set_constant(async);

        AssertExecuteUpdateSql(
"""
UPDATE `Customers` AS `c`
SET `c`.`ContactName` = 'Updated'
WHERE `c`.`CustomerID` LIKE 'F%'
""");
    }

    public override async Task Update_Where_parameter_set_constant(bool async)
    {
        await base.Update_Where_parameter_set_constant(async);
        AssertExecuteUpdateSql(
"""
@__customer_0='ALFKI' (Size = 5) (DbType = StringFixedLength)

UPDATE `Customers` AS `c`
SET `c`.`ContactName` = 'Updated'
WHERE `c`.`CustomerID` = @__customer_0
""",
                //
                """
@__customer_0='ALFKI' (Size = 5) (DbType = StringFixedLength)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` = @__customer_0
""",
                //
                """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE FALSE
""",
                //
                """
UPDATE `Customers` AS `c`
SET `c`.`ContactName` = 'Updated'
WHERE FALSE
""");
    }

    public override async Task Update_Where_set_parameter(bool async)
    {
        await base.Update_Where_set_parameter(async);
        AssertExecuteUpdateSql(
"""
@__value_0='Abc' (Size = 30)

UPDATE `Customers` AS `c`
SET `c`.`ContactName` = @__value_0
WHERE `c`.`CustomerID` LIKE 'F%'
""");
    }

    public override async Task Update_Where_set_parameter_from_closure_array(bool async)
    {
        await base.Update_Where_set_parameter_from_closure_array(async);
        AssertExecuteUpdateSql(
"""
@__p_0='Abc' (Size = 30)

UPDATE `Customers` AS `c`
SET `c`.`ContactName` = @__p_0
WHERE `c`.`CustomerID` LIKE 'F%'
""");
    }

    public override async Task Update_Where_set_parameter_from_inline_list(bool async)
    {
        await base.Update_Where_set_parameter_from_inline_list(async);

        AssertExecuteUpdateSql(
"""
UPDATE `Customers` AS `c`
SET `c`.`ContactName` = 'Abc'
WHERE `c`.`CustomerID` LIKE 'F%'
""");
    }

    public override async Task Update_Where_set_parameter_from_multilevel_property_access(bool async)
    {
        await base.Update_Where_set_parameter_from_multilevel_property_access(async);
        AssertExecuteUpdateSql(
"""
@__container_Containee_Property_0='Abc' (Size = 30)

UPDATE `Customers` AS `c`
SET `c`.`ContactName` = @__container_Containee_Property_0
WHERE `c`.`CustomerID` LIKE 'F%'
""");
    }

    public override async Task Update_Where_Skip_set_constant(bool async)
    {
        await base.Update_Where_Skip_set_constant(async);

        AssertExecuteUpdateSql(
"""
@__p_0='4'

UPDATE `Customers` AS `c0`
INNER JOIN (
    SELECT `c`.`CustomerID`
    FROM `Customers` AS `c`
    WHERE `c`.`CustomerID` LIKE 'F%'
    LIMIT 18446744073709551610 OFFSET @__p_0
) AS `c1` ON `c0`.`CustomerID` = `c1`.`CustomerID`
SET `c0`.`ContactName` = 'Updated'
""");
    }

    public override async Task Update_Where_Take_set_constant(bool async)
    {
        await AssertUpdate(
            async,
            ss => ss.Set<Customer>().Where(c => c.CustomerID.StartsWith("F")).Take(4),
            e => e,
            s => s.SetProperty(c => c.ContactName, "Updated"),
            rowsAffectedCount: 4,
            (b, a) => Assert.All(a, c => Assert.Equal("Updated", c.ContactName)));

        AssertExecuteUpdateSql(
"""
@__p_0='4'

UPDATE `Customers` AS `c`
SET `c`.`ContactName` = 'Updated'
WHERE `c`.`CustomerID` LIKE 'F%'
LIMIT @__p_0
""");
    }

    public override async Task Update_Where_Skip_Take_set_constant(bool async)
    {
        await base.Update_Where_Skip_Take_set_constant(async);

        AssertExecuteUpdateSql(
"""
@__p_1='4'
@__p_0='2'

UPDATE `Customers` AS `c0`
INNER JOIN (
    SELECT `c`.`CustomerID`
    FROM `Customers` AS `c`
    WHERE `c`.`CustomerID` LIKE 'F%'
    LIMIT @__p_1 OFFSET @__p_0
) AS `c1` ON `c0`.`CustomerID` = `c1`.`CustomerID`
SET `c0`.`ContactName` = 'Updated'
""");
    }

    public override async Task Update_Where_OrderBy_set_constant(bool async)
    {
        await base.Update_Where_OrderBy_set_constant(async);

        AssertExecuteUpdateSql(
"""
UPDATE `Customers` AS `c0`
INNER JOIN (
    SELECT `c`.`CustomerID`
    FROM `Customers` AS `c`
    WHERE `c`.`CustomerID` LIKE 'F%'
) AS `c1` ON `c0`.`CustomerID` = `c1`.`CustomerID`
SET `c0`.`ContactName` = 'Updated'
""");
    }

    public override async Task Update_Where_OrderBy_Skip_set_constant(bool async)
    {
        await base.Update_Where_OrderBy_Skip_set_constant(async);

        AssertExecuteUpdateSql(
"""
@__p_0='4'

UPDATE `Customers` AS `c0`
INNER JOIN (
    SELECT `c`.`CustomerID`
    FROM `Customers` AS `c`
    WHERE `c`.`CustomerID` LIKE 'F%'
    ORDER BY `c`.`City`
    LIMIT 18446744073709551610 OFFSET @__p_0
) AS `c1` ON `c0`.`CustomerID` = `c1`.`CustomerID`
SET `c0`.`ContactName` = 'Updated'
""");
    }

    public override async Task Update_Where_OrderBy_Take_set_constant(bool async)
    {
        await base.Update_Where_OrderBy_Take_set_constant(async);

        AssertExecuteUpdateSql(
"""
@__p_0='4'

UPDATE `Customers` AS `c0`
INNER JOIN (
    SELECT `c`.`CustomerID`
    FROM `Customers` AS `c`
    WHERE `c`.`CustomerID` LIKE 'F%'
    ORDER BY `c`.`City`
    LIMIT @__p_0
) AS `c1` ON `c0`.`CustomerID` = `c1`.`CustomerID`
SET `c0`.`ContactName` = 'Updated'
""");
    }

    public override async Task Update_Where_OrderBy_Skip_Take_set_constant(bool async)
    {
        await base.Update_Where_OrderBy_Skip_Take_set_constant(async);

        AssertExecuteUpdateSql(
"""
@__p_1='4'
@__p_0='2'

UPDATE `Customers` AS `c0`
INNER JOIN (
    SELECT `c`.`CustomerID`
    FROM `Customers` AS `c`
    WHERE `c`.`CustomerID` LIKE 'F%'
    ORDER BY `c`.`City`
    LIMIT @__p_1 OFFSET @__p_0
) AS `c1` ON `c0`.`CustomerID` = `c1`.`CustomerID`
SET `c0`.`ContactName` = 'Updated'
""");
    }

    public override async Task Update_Where_OrderBy_Skip_Take_Skip_Take_set_constant(bool async)
    {
        await base.Update_Where_OrderBy_Skip_Take_Skip_Take_set_constant(async);

        AssertExecuteUpdateSql(
"""
@__p_1='6'
@__p_0='2'

UPDATE `Customers` AS `c1`
INNER JOIN (
    SELECT `c0`.`CustomerID`
    FROM (
        SELECT `c`.`CustomerID`, `c`.`City`
        FROM `Customers` AS `c`
        WHERE `c`.`CustomerID` LIKE 'F%'
        ORDER BY `c`.`City`
        LIMIT @__p_1 OFFSET @__p_0
    ) AS `c0`
    ORDER BY `c0`.`City`
    LIMIT @__p_0 OFFSET @__p_0
) AS `c2` ON `c1`.`CustomerID` = `c2`.`CustomerID`
SET `c1`.`ContactName` = 'Updated'
""");
    }

    public override async Task Update_Where_GroupBy_aggregate_set_constant(bool async)
    {
        await base.Update_Where_GroupBy_aggregate_set_constant(async);

        AssertExecuteUpdateSql(
"""
UPDATE `Customers` AS `c`
SET `c`.`ContactName` = 'Updated'
WHERE `c`.`CustomerID` = (
    SELECT `o`.`CustomerID`
    FROM `Orders` AS `o`
    GROUP BY `o`.`CustomerID`
    HAVING COUNT(*) > 11
    LIMIT 1)
""");
    }

    public override async Task Update_Where_GroupBy_First_set_constant(bool async)
    {
        await base.Update_Where_GroupBy_First_set_constant(async);

        AssertExecuteUpdateSql(
"""
UPDATE `Customers` AS `c`
SET `c`.`ContactName` = 'Updated'
WHERE `c`.`CustomerID` = (
    SELECT (
        SELECT `o0`.`CustomerID`
        FROM `Orders` AS `o0`
        WHERE (`o`.`CustomerID` = `o0`.`CustomerID`) OR (`o`.`CustomerID` IS NULL AND (`o0`.`CustomerID` IS NULL))
        LIMIT 1)
    FROM `Orders` AS `o`
    GROUP BY `o`.`CustomerID`
    HAVING COUNT(*) > 11
    LIMIT 1)
""");
    }

    public override async Task Update_Where_GroupBy_First_set_constant_2(bool async)
    {
        await base.Update_Where_GroupBy_First_set_constant_2(async);

        AssertExecuteUpdateSql();
    }

    public override async Task Update_Where_GroupBy_First_set_constant_3(bool async)
    {
        if (AppConfig.ServerVersion.Type == ServerType.MySql)
        {
            // Not supported by MySQL:
            //     Error Code: 1093. You can't specify target table 'c' for update in FROM clause
            await Assert.ThrowsAsync<MySqlException>(
                () => base.Update_Where_GroupBy_First_set_constant_3(async));
        }
        else
        {
            // Works as expected in MariaDB.
            await base.Update_Where_GroupBy_First_set_constant_3(async);

            AssertExecuteUpdateSql(
"""
UPDATE `Customers` AS `c`
SET `c`.`ContactName` = 'Updated'
WHERE `c`.`CustomerID` IN (
    SELECT (
        SELECT `c0`.`CustomerID`
        FROM `Orders` AS `o0`
        LEFT JOIN `Customers` AS `c0` ON `o0`.`CustomerID` = `c0`.`CustomerID`
        WHERE (`o`.`CustomerID` = `o0`.`CustomerID`) OR (`o`.`CustomerID` IS NULL AND (`o0`.`CustomerID` IS NULL))
        LIMIT 1)
    FROM `Orders` AS `o`
    GROUP BY `o`.`CustomerID`
    HAVING COUNT(*) > 11
)
""");
        }
    }

    public override async Task Update_Where_Distinct_set_constant(bool async)
    {
        await base.Update_Where_Distinct_set_constant(async);

        AssertExecuteUpdateSql(
"""
UPDATE `Customers` AS `c0`
INNER JOIN (
    SELECT DISTINCT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
    FROM `Customers` AS `c`
    WHERE `c`.`CustomerID` LIKE 'F%'
) AS `c1` ON `c0`.`CustomerID` = `c1`.`CustomerID`
SET `c0`.`ContactName` = 'Updated'
""");
    }

    public override async Task Update_Where_using_navigation_set_null(bool async)
    {
        await base.Update_Where_using_navigation_set_null(async);

        AssertExecuteUpdateSql(
"""
UPDATE `Orders` AS `o`
LEFT JOIN `Customers` AS `c` ON `o`.`CustomerID` = `c`.`CustomerID`
SET `o`.`OrderDate` = NULL
WHERE `c`.`City` = 'Seattle'
""");
    }

    public override async Task Update_Where_using_navigation_2_set_constant(bool async)
    {
        await base.Update_Where_using_navigation_2_set_constant(async);

        AssertExecuteUpdateSql(
"""
UPDATE `Order Details` AS `o`
INNER JOIN `Orders` AS `o0` ON `o`.`OrderID` = `o0`.`OrderID`
LEFT JOIN `Customers` AS `c` ON `o0`.`CustomerID` = `c`.`CustomerID`
SET `o`.`Quantity` = CAST(1 AS signed)
WHERE `c`.`City` = 'Seattle'
""");
    }

    public override async Task Update_Where_SelectMany_set_null(bool async)
    {
        await base.Update_Where_SelectMany_set_null(async);

        AssertExecuteUpdateSql(
"""
UPDATE `Customers` AS `c`
INNER JOIN `Orders` AS `o` ON `c`.`CustomerID` = `o`.`CustomerID`
SET `o`.`OrderDate` = NULL
WHERE `c`.`CustomerID` LIKE 'F%'
""");
    }

    public override async Task Update_Where_set_property_plus_constant(bool async)
    {
        await base.Update_Where_set_property_plus_constant(async);

        AssertExecuteUpdateSql(
"""
UPDATE `Customers` AS `c`
SET `c`.`ContactName` = CONCAT(COALESCE(`c`.`ContactName`, ''), 'Abc')
WHERE `c`.`CustomerID` LIKE 'F%'
""");
    }

    public override async Task Update_Where_set_property_plus_parameter(bool async)
    {
        await base.Update_Where_set_property_plus_parameter(async);

        AssertExecuteUpdateSql(
"""
@__value_0='Abc' (Size = 4000)

UPDATE `Customers` AS `c`
SET `c`.`ContactName` = CONCAT(COALESCE(`c`.`ContactName`, ''), @__value_0)
WHERE `c`.`CustomerID` LIKE 'F%'
""");
    }

    public override async Task Update_Where_set_property_plus_property(bool async)
    {
        await base.Update_Where_set_property_plus_property(async);

        AssertExecuteUpdateSql(
"""
UPDATE `Customers` AS `c`
SET `c`.`ContactName` = CONCAT(COALESCE(`c`.`ContactName`, ''), `c`.`CustomerID`)
WHERE `c`.`CustomerID` LIKE 'F%'
""");
    }

    public override async Task Update_Where_set_constant_using_ef_property(bool async)
    {
        await base.Update_Where_set_constant_using_ef_property(async);

        AssertExecuteUpdateSql(
"""
UPDATE `Customers` AS `c`
SET `c`.`ContactName` = 'Updated'
WHERE `c`.`CustomerID` LIKE 'F%'
""");
    }

    public override async Task Update_Where_set_null(bool async)
    {
        await base.Update_Where_set_null(async);

        AssertExecuteUpdateSql(
"""
UPDATE `Customers` AS `c`
SET `c`.`ContactName` = NULL
WHERE `c`.`CustomerID` LIKE 'F%'
""");
    }

    public override async Task Update_without_property_to_set_throws(bool async)
    {
        await base.Update_without_property_to_set_throws(async);

        AssertExecuteUpdateSql();
    }

    public override async Task Update_with_invalid_lambda_throws(bool async)
    {
        await base.Update_with_invalid_lambda_throws(async);

        AssertExecuteUpdateSql();
    }

    public override async Task Update_Where_multiple_set(bool async)
    {
        await base.Update_Where_multiple_set(async);

        AssertExecuteUpdateSql(
"""
@__value_0='Abc' (Size = 30)

UPDATE `Customers` AS `c`
SET `c`.`City` = 'Seattle',
    `c`.`ContactName` = @__value_0
WHERE `c`.`CustomerID` LIKE 'F%'
""");
    }

    public override async Task Update_with_invalid_lambda_in_set_property_throws(bool async)
    {
        await base.Update_with_invalid_lambda_in_set_property_throws(async);

        AssertExecuteUpdateSql();
    }

    public override async Task Update_unmapped_property_throws(bool async)
    {
        await base.Update_unmapped_property_throws(async);

        AssertExecuteUpdateSql();
    }

    public override async Task Update_Union_set_constant(bool async)
    {
        await base.Update_Union_set_constant(async);

        AssertExecuteUpdateSql(
"""
UPDATE `Customers` AS `c1`
INNER JOIN (
    SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
    FROM `Customers` AS `c`
    WHERE `c`.`CustomerID` LIKE 'F%'
    UNION
    SELECT `c0`.`CustomerID`, `c0`.`Address`, `c0`.`City`, `c0`.`CompanyName`, `c0`.`ContactName`, `c0`.`ContactTitle`, `c0`.`Country`, `c0`.`Fax`, `c0`.`Phone`, `c0`.`PostalCode`, `c0`.`Region`
    FROM `Customers` AS `c0`
    WHERE `c0`.`CustomerID` LIKE 'A%'
) AS `u` ON `c1`.`CustomerID` = `u`.`CustomerID`
SET `c1`.`ContactName` = 'Updated'
""");
    }

    public override async Task Update_Concat_set_constant(bool async)
    {
        await base.Update_Concat_set_constant(async);

        AssertExecuteUpdateSql(
"""
UPDATE `Customers` AS `c1`
INNER JOIN (
    SELECT `c`.`CustomerID`
    FROM `Customers` AS `c`
    WHERE `c`.`CustomerID` LIKE 'F%'
    UNION ALL
    SELECT `c0`.`CustomerID`
    FROM `Customers` AS `c0`
    WHERE `c0`.`CustomerID` LIKE 'A%'
) AS `u` ON `c1`.`CustomerID` = `u`.`CustomerID`
SET `c1`.`ContactName` = 'Updated'
""");
    }

    public override async Task Update_Except_set_constant(bool async)
    {
        await base.Update_Except_set_constant(async);

        AssertExecuteUpdateSql(
"""
UPDATE `Customers` AS `c1`
INNER JOIN (
    SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
    FROM `Customers` AS `c`
    WHERE `c`.`CustomerID` LIKE 'F%'
    EXCEPT
    SELECT `c0`.`CustomerID`, `c0`.`Address`, `c0`.`City`, `c0`.`CompanyName`, `c0`.`ContactName`, `c0`.`ContactTitle`, `c0`.`Country`, `c0`.`Fax`, `c0`.`Phone`, `c0`.`PostalCode`, `c0`.`Region`
    FROM `Customers` AS `c0`
    WHERE `c0`.`CustomerID` LIKE 'A%'
) AS `e` ON `c1`.`CustomerID` = `e`.`CustomerID`
SET `c1`.`ContactName` = 'Updated'
""");
    }

    public override async Task Update_Intersect_set_constant(bool async)
    {
        await base.Update_Intersect_set_constant(async);

        AssertExecuteUpdateSql(
"""
UPDATE `Customers` AS `c1`
INNER JOIN (
    SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
    FROM `Customers` AS `c`
    WHERE `c`.`CustomerID` LIKE 'F%'
    INTERSECT
    SELECT `c0`.`CustomerID`, `c0`.`Address`, `c0`.`City`, `c0`.`CompanyName`, `c0`.`ContactName`, `c0`.`ContactTitle`, `c0`.`Country`, `c0`.`Fax`, `c0`.`Phone`, `c0`.`PostalCode`, `c0`.`Region`
    FROM `Customers` AS `c0`
    WHERE `c0`.`CustomerID` LIKE 'A%'
) AS `i` ON `c1`.`CustomerID` = `i`.`CustomerID`
SET `c1`.`ContactName` = 'Updated'
""");
    }

    public override async Task Update_with_join_set_constant(bool async)
    {
        await base.Update_with_join_set_constant(async);

        AssertExecuteUpdateSql(
"""
UPDATE `Customers` AS `c`
INNER JOIN (
    SELECT `o`.`CustomerID`
    FROM `Orders` AS `o`
    WHERE `o`.`OrderID` < 10300
) AS `o0` ON `c`.`CustomerID` = `o0`.`CustomerID`
SET `c`.`ContactName` = 'Updated'
WHERE `c`.`CustomerID` LIKE 'F%'
""");
    }

    public override async Task Update_with_left_join_set_constant(bool async)
    {
        await base.Update_with_left_join_set_constant(async);

        AssertExecuteUpdateSql(
"""
UPDATE `Customers` AS `c`
LEFT JOIN (
    SELECT `o`.`CustomerID`
    FROM `Orders` AS `o`
    WHERE `o`.`OrderID` < 10300
) AS `o0` ON `c`.`CustomerID` = `o0`.`CustomerID`
SET `c`.`ContactName` = 'Updated'
WHERE `c`.`CustomerID` LIKE 'F%'
""");
    }

    public override async Task Update_with_cross_join_set_constant(bool async)
    {
        await base.Update_with_cross_join_set_constant(async);

        AssertExecuteUpdateSql(
"""
UPDATE `Customers` AS `c`
CROSS JOIN (
    SELECT 1
    FROM `Orders` AS `o`
    WHERE `o`.`OrderID` < 10300
) AS `o0`
SET `c`.`ContactName` = 'Updated'
WHERE `c`.`CustomerID` LIKE 'F%'
""");
    }

    public override async Task Update_with_cross_apply_set_constant(bool async)
    {
        await base.Update_with_cross_apply_set_constant(async);

        AssertExecuteUpdateSql(
"""
UPDATE `Customers` AS `c`
JOIN LATERAL (
    SELECT 1
    FROM `Orders` AS `o`
    WHERE (`o`.`OrderID` < 10300) AND (EXTRACT(year FROM `o`.`OrderDate`) < CHAR_LENGTH(`c`.`ContactName`))
) AS `o0` ON TRUE
SET `c`.`ContactName` = 'Updated'
WHERE `c`.`CustomerID` LIKE 'F%'
""");
    }

    public override async Task Update_with_outer_apply_set_constant(bool async)
    {
        await base.Update_with_outer_apply_set_constant(async);

        AssertExecuteUpdateSql(
"""
UPDATE `Customers` AS `c`
LEFT JOIN LATERAL (
    SELECT 1
    FROM `Orders` AS `o`
    WHERE (`o`.`OrderID` < 10300) AND (EXTRACT(year FROM `o`.`OrderDate`) < CHAR_LENGTH(`c`.`ContactName`))
) AS `o0` ON TRUE
SET `c`.`ContactName` = 'Updated'
WHERE `c`.`CustomerID` LIKE 'F%'
""");
    }

    public override async Task Update_with_cross_join_left_join_set_constant(bool async)
    {
        await base.Update_with_cross_join_left_join_set_constant(async);

        AssertExecuteUpdateSql(
"""
UPDATE `Customers` AS `c`
CROSS JOIN (
    SELECT 1
    FROM `Customers` AS `c0`
    WHERE `c0`.`City` LIKE 'S%'
) AS `c1`
LEFT JOIN (
    SELECT `o`.`CustomerID`
    FROM `Orders` AS `o`
    WHERE `o`.`OrderID` < 10300
) AS `o0` ON `c`.`CustomerID` = `o0`.`CustomerID`
SET `c`.`ContactName` = 'Updated'
WHERE `c`.`CustomerID` LIKE 'F%'
""");
    }

    public override async Task Update_with_cross_join_cross_apply_set_constant(bool async)
    {
        await base.Update_with_cross_join_cross_apply_set_constant(async);

        AssertExecuteUpdateSql(
"""
UPDATE `Customers` AS `c`
CROSS JOIN (
    SELECT 1
    FROM `Customers` AS `c0`
    WHERE `c0`.`City` LIKE 'S%'
) AS `c1`
JOIN LATERAL (
    SELECT 1
    FROM `Orders` AS `o`
    WHERE (`o`.`OrderID` < 10300) AND (EXTRACT(year FROM `o`.`OrderDate`) < CHAR_LENGTH(`c`.`ContactName`))
) AS `o0` ON TRUE
SET `c`.`ContactName` = 'Updated'
WHERE `c`.`CustomerID` LIKE 'F%'
""");
    }

    public override async Task Update_with_cross_join_outer_apply_set_constant(bool async)
    {
        await base.Update_with_cross_join_outer_apply_set_constant(async);

        AssertExecuteUpdateSql(
"""
UPDATE `Customers` AS `c`
CROSS JOIN (
    SELECT 1
    FROM `Customers` AS `c0`
    WHERE `c0`.`City` LIKE 'S%'
) AS `c1`
LEFT JOIN LATERAL (
    SELECT 1
    FROM `Orders` AS `o`
    WHERE (`o`.`OrderID` < 10300) AND (EXTRACT(year FROM `o`.`OrderDate`) < CHAR_LENGTH(`c`.`ContactName`))
) AS `o0` ON TRUE
SET `c`.`ContactName` = 'Updated'
WHERE `c`.`CustomerID` LIKE 'F%'
""");
    }

    public override async Task Update_FromSql_set_constant(bool async)
    {
        await base.Update_FromSql_set_constant(async);

        AssertExecuteUpdateSql();
    }

    public override async Task Update_Where_SelectMany_subquery_set_null(bool async)
    {
        await base.Update_Where_SelectMany_subquery_set_null(async);

        AssertExecuteUpdateSql(
"""
UPDATE `Orders` AS `o1`
INNER JOIN (
    SELECT `o0`.`OrderID`
    FROM `Customers` AS `c`
    INNER JOIN (
        SELECT `o`.`OrderID`, `o`.`CustomerID`
        FROM `Orders` AS `o`
        WHERE EXTRACT(year FROM `o`.`OrderDate`) = 1997
    ) AS `o0` ON `c`.`CustomerID` = `o0`.`CustomerID`
    WHERE `c`.`CustomerID` LIKE 'F%'
) AS `s` ON `o1`.`OrderID` = `s`.`OrderID`
SET `o1`.`OrderDate` = NULL
""");
    }

    public override async Task Update_Where_Join_set_property_from_joined_single_result_table(bool async)
    {
        await base.Update_Where_Join_set_property_from_joined_single_result_table(async);

        AssertExecuteUpdateSql(
"""
UPDATE `Customers` AS `c`
SET `c`.`City` = CAST(EXTRACT(year FROM (
    SELECT `o`.`OrderDate`
    FROM `Orders` AS `o`
    WHERE `c`.`CustomerID` = `o`.`CustomerID`
    ORDER BY `o`.`OrderDate` DESC
    LIMIT 1)) AS char)
WHERE `c`.`CustomerID` LIKE 'F%'
""");
    }

    public override async Task Update_Where_Join_set_property_from_joined_table(bool async)
    {
        await base.Update_Where_Join_set_property_from_joined_table(async);

        AssertExecuteUpdateSql(
"""
UPDATE `Customers` AS `c`
CROSS JOIN (
    SELECT `c0`.`City`
    FROM `Customers` AS `c0`
    WHERE `c0`.`CustomerID` = 'ALFKI'
) AS `c1`
SET `c`.`City` = `c1`.`City`
WHERE `c`.`CustomerID` LIKE 'F%'
""");
    }

    public override async Task Update_Where_Join_set_property_from_joined_single_result_scalar(bool async)
    {
        await base.Update_Where_Join_set_property_from_joined_single_result_scalar(async);

        AssertExecuteUpdateSql(
"""
UPDATE `Customers` AS `c`
SET `c`.`City` = CAST(EXTRACT(year FROM (
    SELECT `o`.`OrderDate`
    FROM `Orders` AS `o`
    WHERE `c`.`CustomerID` = `o`.`CustomerID`
    ORDER BY `o`.`OrderDate` DESC
    LIMIT 1)) AS char)
WHERE `c`.`CustomerID` LIKE 'F%'
""");
    }

    public override async Task Update_multiple_tables_throws(bool async)
    {
        await base.Update_multiple_tables_throws(async);

        AssertSql(
"""
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Orders` AS `o`
LEFT JOIN `Customers` AS `c` ON `o`.`CustomerID` = `c`.`CustomerID`
WHERE `o`.`CustomerID` LIKE 'F%'
""");
    }

    public override async Task Update_with_two_inner_joins(bool async)
    {
        await base.Update_with_two_inner_joins(async);

        AssertSql(
"""
SELECT `o`.`OrderID`, `o`.`ProductID`, `o`.`Discount`, `o`.`Quantity`, `o`.`UnitPrice`
FROM `Order Details` AS `o`
INNER JOIN `Products` AS `p` ON `o`.`ProductID` = `p`.`ProductID`
INNER JOIN `Orders` AS `o0` ON `o`.`OrderID` = `o0`.`OrderID`
WHERE `p`.`Discontinued` AND (`o0`.`OrderDate` > TIMESTAMP '1990-01-01 00:00:00')
""",
                //
                """
UPDATE `Order Details` AS `o`
INNER JOIN `Products` AS `p` ON `o`.`ProductID` = `p`.`ProductID`
INNER JOIN `Orders` AS `o0` ON `o`.`OrderID` = `o0`.`OrderID`
SET `o`.`Quantity` = CAST(1 AS signed)
WHERE `p`.`Discontinued` AND (`o0`.`OrderDate` > TIMESTAMP '1990-01-01 00:00:00')
""",
                //
                """
SELECT `o`.`OrderID`, `o`.`ProductID`, `o`.`Discount`, `o`.`Quantity`, `o`.`UnitPrice`
FROM `Order Details` AS `o`
INNER JOIN `Products` AS `p` ON `o`.`ProductID` = `p`.`ProductID`
INNER JOIN `Orders` AS `o0` ON `o`.`OrderID` = `o0`.`OrderID`
WHERE `p`.`Discontinued` AND (`o0`.`OrderDate` > TIMESTAMP '1990-01-01 00:00:00')
""");
    }

    private void AssertSql(params string[] expected)
        => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);

    private void AssertExecuteUpdateSql(params string[] expected)
        => Fixture.TestSqlLoggerFactory.AssertBaseline(expected, forUpdate: true);
}
