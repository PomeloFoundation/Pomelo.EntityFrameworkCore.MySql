using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using MySqlConnector;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Tests.TestUtilities.Attributes;
using Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query;

public class SqlQueryMySqlTest : SqlQueryTestBase<NorthwindQueryMySqlFixture<NoopModelCustomizer>>
{
    public SqlQueryMySqlTest(NorthwindQueryMySqlFixture<NoopModelCustomizer> fixture, ITestOutputHelper testOutputHelper)
        : base(fixture)
    {
        Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
    }

    public override async Task SqlQueryRaw_queryable_simple(bool async)
    {
        await base.SqlQueryRaw_queryable_simple(async);

        AssertSql(
"""
SELECT * FROM `Customers` WHERE `ContactName` LIKE '%z%'
""");
    }

    public override async Task SqlQueryRaw_queryable_simple_columns_out_of_order(bool async)
    {
        await base.SqlQueryRaw_queryable_simple_columns_out_of_order(async);

        AssertSql(
"""
SELECT `Region`, `PostalCode`, `Phone`, `Fax`, `CustomerID`, `Country`, `ContactTitle`, `ContactName`, `CompanyName`, `City`, `Address` FROM `Customers`
""");
    }

    public override async Task SqlQueryRaw_queryable_simple_columns_out_of_order_and_extra_columns(bool async)
    {
        await base.SqlQueryRaw_queryable_simple_columns_out_of_order_and_extra_columns(async);

        AssertSql(
"""
SELECT `Region`, `PostalCode`, `PostalCode` AS `Foo`, `Phone`, `Fax`, `CustomerID`, `Country`, `ContactTitle`, `ContactName`, `CompanyName`, `City`, `Address` FROM `Customers`
""");
    }

    public override async Task SqlQueryRaw_queryable_composed(bool async)
    {
        await base.SqlQueryRaw_queryable_composed(async);

        AssertSql(
"""
SELECT `m`.`Address`, `m`.`City`, `m`.`CompanyName`, `m`.`ContactName`, `m`.`ContactTitle`, `m`.`Country`, `m`.`CustomerID`, `m`.`Fax`, `m`.`Phone`, `m`.`Region`, `m`.`PostalCode`
FROM (
    SELECT * FROM `Customers`
) AS `m`
WHERE `m`.`ContactName` LIKE '%z%'
""");
    }

    public override async Task SqlQueryRaw_queryable_composed_after_removing_whitespaces(bool async)
    {
        await base.SqlQueryRaw_queryable_composed_after_removing_whitespaces(async);

        AssertSql(
"SELECT `m`.`Address`, `m`.`City`, `m`.`CompanyName`, `m`.`ContactName`, `m`.`ContactTitle`, `m`.`Country`, `m`.`CustomerID`, `m`.`Fax`, `m`.`Phone`, `m`.`Region`, `m`.`PostalCode`\r\nFROM (\r\n\r\n        \r\n\r\n\r\n    SELECT\r\n    * FROM `Customers`\r\n) AS `m`\r\nWHERE `m`.`ContactName` LIKE '%z%'");
    }

    public override async Task SqlQueryRaw_queryable_composed_compiled(bool async)
    {
        await base.SqlQueryRaw_queryable_composed_compiled(async);

        AssertSql(
"""
SELECT `m`.`Address`, `m`.`City`, `m`.`CompanyName`, `m`.`ContactName`, `m`.`ContactTitle`, `m`.`Country`, `m`.`CustomerID`, `m`.`Fax`, `m`.`Phone`, `m`.`Region`, `m`.`PostalCode`
FROM (
    SELECT * FROM `Customers`
) AS `m`
WHERE `m`.`ContactName` LIKE '%z%'
""");
    }

    public override async Task SqlQueryRaw_queryable_composed_compiled_with_DbParameter(bool async)
    {
        await base.SqlQueryRaw_queryable_composed_compiled_with_DbParameter(async);

        AssertSql(
"""
customer='CONSH' (Nullable = false)

SELECT `m`.`Address`, `m`.`City`, `m`.`CompanyName`, `m`.`ContactName`, `m`.`ContactTitle`, `m`.`Country`, `m`.`CustomerID`, `m`.`Fax`, `m`.`Phone`, `m`.`Region`, `m`.`PostalCode`
FROM (
    SELECT * FROM `Customers` WHERE `CustomerID` = @customer
) AS `m`
WHERE `m`.`ContactName` LIKE '%z%'
""");
    }

    public override async Task SqlQueryRaw_queryable_composed_compiled_with_nameless_DbParameter(bool async)
    {
        await base.SqlQueryRaw_queryable_composed_compiled_with_nameless_DbParameter(async);

        AssertSql(
"""
p0='CONSH' (Nullable = false)

SELECT `m`.`Address`, `m`.`City`, `m`.`CompanyName`, `m`.`ContactName`, `m`.`ContactTitle`, `m`.`Country`, `m`.`CustomerID`, `m`.`Fax`, `m`.`Phone`, `m`.`Region`, `m`.`PostalCode`
FROM (
    SELECT * FROM `Customers` WHERE `CustomerID` = @p0
) AS `m`
WHERE `m`.`ContactName` LIKE '%z%'
""");
    }

    public override async Task SqlQueryRaw_queryable_composed_compiled_with_parameter(bool async)
    {
        await base.SqlQueryRaw_queryable_composed_compiled_with_parameter(async);

        AssertSql(
"""
SELECT `m`.`Address`, `m`.`City`, `m`.`CompanyName`, `m`.`ContactName`, `m`.`ContactTitle`, `m`.`Country`, `m`.`CustomerID`, `m`.`Fax`, `m`.`Phone`, `m`.`Region`, `m`.`PostalCode`
FROM (
    SELECT * FROM `Customers` WHERE `CustomerID` = 'CONSH'
) AS `m`
WHERE `m`.`ContactName` LIKE '%z%'
""");
    }

    public override async Task SqlQueryRaw_composed_contains(bool async)
    {
        await base.SqlQueryRaw_composed_contains(async);

        AssertSql(
"""
SELECT `m`.`Address`, `m`.`City`, `m`.`CompanyName`, `m`.`ContactName`, `m`.`ContactTitle`, `m`.`Country`, `m`.`CustomerID`, `m`.`Fax`, `m`.`Phone`, `m`.`Region`, `m`.`PostalCode`
FROM (
    SELECT * FROM `Customers`
) AS `m`
WHERE `m`.`CustomerID` IN (
    SELECT `m0`.`CustomerID`
    FROM (
        SELECT * FROM `Orders`
    ) AS `m0`
)
""");
    }

    public override async Task SqlQueryRaw_queryable_multiple_composed(bool async)
    {
        await base.SqlQueryRaw_queryable_multiple_composed(async);

        AssertSql(
"""
SELECT `m`.`Address`, `m`.`City`, `m`.`CompanyName`, `m`.`ContactName`, `m`.`ContactTitle`, `m`.`Country`, `m`.`CustomerID`, `m`.`Fax`, `m`.`Phone`, `m`.`Region`, `m`.`PostalCode`, `m0`.`CustomerID`, `m0`.`EmployeeID`, `m0`.`Freight`, `m0`.`OrderDate`, `m0`.`OrderID`, `m0`.`RequiredDate`, `m0`.`ShipAddress`, `m0`.`ShipCity`, `m0`.`ShipCountry`, `m0`.`ShipName`, `m0`.`ShipPostalCode`, `m0`.`ShipRegion`, `m0`.`ShipVia`, `m0`.`ShippedDate`
FROM (
    SELECT * FROM `Customers`
) AS `m`
CROSS JOIN (
    SELECT * FROM `Orders`
) AS `m0`
WHERE `m`.`CustomerID` = `m0`.`CustomerID`
""");
    }

    public override Task SqlQueryRaw_queryable_multiple_composed_with_closure_parameters(bool async)
    {
        // Base implementation sends DateTime with Kind=Unspecified in a SQL query, but MySql rejects it because timestamptz
        return Task.CompletedTask;
    }

    public override Task SqlQueryRaw_queryable_multiple_composed_with_parameters_and_closure_parameters(bool async)
    {
        // Base implementation sends DateTime with Kind=Unspecified in a SQL query, but MySql rejects it because timestamptz
        return Task.CompletedTask;
    }

    public override async Task SqlQueryRaw_queryable_multiple_line_query(bool async)
    {
        await base.SqlQueryRaw_queryable_multiple_line_query(async);

        AssertSql(
"""
SELECT *
FROM `Customers`
WHERE `City` = 'London'
""");
    }

    public override async Task SqlQueryRaw_queryable_composed_multiple_line_query(bool async)
    {
        await base.SqlQueryRaw_queryable_composed_multiple_line_query(async);

        AssertSql(
"""
SELECT `m`.`Address`, `m`.`City`, `m`.`CompanyName`, `m`.`ContactName`, `m`.`ContactTitle`, `m`.`Country`, `m`.`CustomerID`, `m`.`Fax`, `m`.`Phone`, `m`.`Region`, `m`.`PostalCode`
FROM (
    SELECT *
    FROM `Customers`
) AS `m`
WHERE `m`.`City` = 'London'
""");
    }

    public override async Task SqlQueryRaw_queryable_with_parameters(bool async)
    {
        await base.SqlQueryRaw_queryable_with_parameters(async);

        AssertSql(
"""
p0='London' (Size = 4000)
p1='Sales Representative' (Size = 4000)

SELECT * FROM `Customers` WHERE `City` = @p0 AND `ContactTitle` = @p1
""");
    }

    public override async Task SqlQueryRaw_queryable_with_parameters_inline(bool async)
    {
        await base.SqlQueryRaw_queryable_with_parameters_inline(async);

        AssertSql(
"""
p0='London' (Size = 4000)
p1='Sales Representative' (Size = 4000)

SELECT * FROM `Customers` WHERE `City` = @p0 AND `ContactTitle` = @p1
""");
    }

    public override async Task SqlQuery_queryable_with_parameters_interpolated(bool async)
    {
        await base.SqlQuery_queryable_with_parameters_interpolated(async);

        AssertSql(
"""
p0='London' (Size = 4000)
p1='Sales Representative' (Size = 4000)

SELECT * FROM `Customers` WHERE `City` = @p0 AND `ContactTitle` = @p1
""");
    }

    public override async Task SqlQuery_queryable_with_parameters_inline_interpolated(bool async)
    {
        await base.SqlQuery_queryable_with_parameters_inline_interpolated(async);

        AssertSql(
"""
p0='London' (Size = 4000)
p1='Sales Representative' (Size = 4000)

SELECT * FROM `Customers` WHERE `City` = @p0 AND `ContactTitle` = @p1
""");
    }

    public override Task SqlQuery_queryable_multiple_composed_with_parameters_and_closure_parameters_interpolated(bool async)
    {
        // Base implementation sends DateTime with Kind=Unspecified in a SQL query, but MySql rejects it because timestamptz
        return Task.CompletedTask;
    }

    public override async Task SqlQueryRaw_queryable_with_null_parameter(bool async)
    {
        await base.SqlQueryRaw_queryable_with_null_parameter(async);

        AssertSql(
"""
p0=NULL (Nullable = false)

SELECT * FROM `Employees` WHERE `ReportsTo` = @p0 OR (`ReportsTo` IS NULL AND @p0 IS NULL)
""");
    }

    public override async Task<string> SqlQueryRaw_queryable_with_parameters_and_closure(bool async)
    {
        var queryString = await base.SqlQueryRaw_queryable_with_parameters_and_closure(async);

        AssertSql(
"""
p0='London' (Size = 4000)
@__contactTitle_1='Sales Representative' (Size = 30)

SELECT `m`.`Address`, `m`.`City`, `m`.`CompanyName`, `m`.`ContactName`, `m`.`ContactTitle`, `m`.`Country`, `m`.`CustomerID`, `m`.`Fax`, `m`.`Phone`, `m`.`Region`, `m`.`PostalCode`
FROM (
    SELECT * FROM `Customers` WHERE `City` = @p0
) AS `m`
WHERE `m`.`ContactTitle` = @__contactTitle_1
""");

        return null;
    }

    public override async Task SqlQueryRaw_queryable_simple_cache_key_includes_query_string(bool async)
    {
        await base.SqlQueryRaw_queryable_simple_cache_key_includes_query_string(async);

        AssertSql(
"""
SELECT * FROM `Customers` WHERE `City` = 'London'
""",
                //
                """
SELECT * FROM `Customers` WHERE `City` = 'Seattle'
""");
    }

    public override async Task SqlQueryRaw_queryable_with_parameters_cache_key_includes_parameters(bool async)
    {
        await base.SqlQueryRaw_queryable_with_parameters_cache_key_includes_parameters(async);

        AssertSql(
"""
p0='London' (Size = 4000)
p1='Sales Representative' (Size = 4000)

SELECT * FROM `Customers` WHERE `City` = @p0 AND `ContactTitle` = @p1
""",
                //
                """
p0='Madrid' (Size = 4000)
p1='Accounting Manager' (Size = 4000)

SELECT * FROM `Customers` WHERE `City` = @p0 AND `ContactTitle` = @p1
""");
    }

    public override async Task SqlQueryRaw_queryable_simple_as_no_tracking_not_composed(bool async)
    {
        await base.SqlQueryRaw_queryable_simple_as_no_tracking_not_composed(async);

        AssertSql(
"""
SELECT * FROM `Customers`
""");
    }

    public override async Task SqlQueryRaw_queryable_simple_projection_composed(bool async)
    {
        await base.SqlQueryRaw_queryable_simple_projection_composed(async);

        AssertSql(
"""
SELECT `m`.`ProductName`
FROM (
    SELECT *
    FROM `Products`
    WHERE `Discontinued` <> TRUE
    AND ((`UnitsInStock` + `UnitsOnOrder`) < `ReorderLevel`)
) AS `m`
""");
    }

    public override async Task SqlQueryRaw_annotations_do_not_affect_successive_calls(bool async)
    {
        await base.SqlQueryRaw_annotations_do_not_affect_successive_calls(async);

        AssertSql(
"""
SELECT * FROM `Customers` WHERE `ContactName` LIKE '%z%'
""",
                //
                """
SELECT * FROM `Customers`
""");
    }

    public override async Task SqlQueryRaw_composed_with_predicate(bool async)
    {
        await base.SqlQueryRaw_composed_with_predicate(async);

        AssertSql(
"""
SELECT `m`.`Address`, `m`.`City`, `m`.`CompanyName`, `m`.`ContactName`, `m`.`ContactTitle`, `m`.`Country`, `m`.`CustomerID`, `m`.`Fax`, `m`.`Phone`, `m`.`Region`, `m`.`PostalCode`
FROM (
    SELECT * FROM `Customers`
) AS `m`
WHERE SUBSTRING(`m`.`ContactName`, 0 + 1, 1) = SUBSTRING(`m`.`CompanyName`, 0 + 1, 1)
""");
    }

    public override async Task SqlQueryRaw_composed_with_empty_predicate(bool async)
    {
        await base.SqlQueryRaw_composed_with_empty_predicate(async);

        AssertSql(
"""
SELECT `m`.`Address`, `m`.`City`, `m`.`CompanyName`, `m`.`ContactName`, `m`.`ContactTitle`, `m`.`Country`, `m`.`CustomerID`, `m`.`Fax`, `m`.`Phone`, `m`.`Region`, `m`.`PostalCode`
FROM (
    SELECT * FROM `Customers`
) AS `m`
WHERE `m`.`ContactName` = `m`.`CompanyName`
""");
    }

    public override async Task SqlQueryRaw_with_dbParameter(bool async)
    {
        await base.SqlQueryRaw_with_dbParameter(async);

        AssertSql(
"""
@city='London' (Nullable = false)

SELECT * FROM `Customers` WHERE `City` = @city
""");
    }

    public override async Task SqlQueryRaw_with_dbParameter_without_name_prefix(bool async)
    {
        await base.SqlQueryRaw_with_dbParameter_without_name_prefix(async);
        AssertSql(
"""
city='London' (Nullable = false)

SELECT * FROM `Customers` WHERE `City` = @city
""");
    }

    public override async Task SqlQueryRaw_with_dbParameter_mixed(bool async)
    {
        await base.SqlQueryRaw_with_dbParameter_mixed(async);

        AssertSql(
"""
p0='London' (Size = 4000)
@title='Sales Representative' (Nullable = false)

SELECT * FROM `Customers` WHERE `City` = @p0 AND `ContactTitle` = @title
""",
                //
                """
@city='London' (Nullable = false)
p1='Sales Representative' (Size = 4000)

SELECT * FROM `Customers` WHERE `City` = @city AND `ContactTitle` = @p1
""");
    }

    public override async Task SqlQueryRaw_with_db_parameters_called_multiple_times(bool async)
    {
        await base.SqlQueryRaw_with_db_parameters_called_multiple_times(async);

        AssertSql(
"""
@id='ALFKI' (Nullable = false)

SELECT * FROM `Customers` WHERE `CustomerID` = @id
""",
                //
                """
@id='ALFKI' (Nullable = false)

SELECT * FROM `Customers` WHERE `CustomerID` = @id
""");
    }

    public override async Task SqlQuery_with_inlined_db_parameter(bool async)
    {
        await base.SqlQuery_with_inlined_db_parameter(async);

        AssertSql(
"""
@somename='ALFKI' (Nullable = false)

SELECT * FROM `Customers` WHERE `CustomerID` = @somename
""");
    }

    public override async Task SqlQuery_with_inlined_db_parameter_without_name_prefix(bool async)
    {
        await base.SqlQuery_with_inlined_db_parameter_without_name_prefix(async);

        AssertSql(
"""
somename='ALFKI' (Nullable = false)

SELECT * FROM `Customers` WHERE `CustomerID` = @somename
""");
    }

    public override async Task SqlQuery_parameterization_issue_12213(bool async)
    {
        await base.SqlQuery_parameterization_issue_12213(async);

        AssertSql(
"""
p0='10300'

SELECT `m`.`OrderID`
FROM (
    SELECT * FROM `Orders` WHERE `OrderID` >= @p0
) AS `m`
""",
                //
                """
@__max_1='10400'
p0='10300'

SELECT `m`.`OrderID`
FROM (
    SELECT * FROM `Orders`
) AS `m`
WHERE (`m`.`OrderID` <= @__max_1) AND `m`.`OrderID` IN (
    SELECT `m0`.`OrderID`
    FROM (
        SELECT * FROM `Orders` WHERE `OrderID` >= @p0
    ) AS `m0`
)
""",
                //
                """
@__max_1='10400'
p0='10300'

SELECT `m`.`OrderID`
FROM (
    SELECT * FROM `Orders`
) AS `m`
WHERE (`m`.`OrderID` <= @__max_1) AND `m`.`OrderID` IN (
    SELECT `m0`.`OrderID`
    FROM (
        SELECT * FROM `Orders` WHERE `OrderID` >= @p0
    ) AS `m0`
)
""");
    }

    public override async Task SqlQueryRaw_does_not_parameterize_interpolated_string(bool async)
    {
        await base.SqlQueryRaw_does_not_parameterize_interpolated_string(async);

        AssertSql(
"""
p0='10250'

SELECT * FROM `Orders` WHERE `OrderID` < @p0
""");
    }

    public override async Task SqlQueryRaw_with_set_operation(bool async)
    {
        await base.SqlQueryRaw_with_set_operation(async);

        AssertSql(
"""
SELECT `m`.`Address`, `m`.`City`, `m`.`CompanyName`, `m`.`ContactName`, `m`.`ContactTitle`, `m`.`Country`, `m`.`CustomerID`, `m`.`Fax`, `m`.`Phone`, `m`.`Region`, `m`.`PostalCode`
FROM (
    SELECT * FROM `Customers` WHERE `City` = 'London'
) AS `m`
UNION ALL
SELECT `m0`.`Address`, `m0`.`City`, `m0`.`CompanyName`, `m0`.`ContactName`, `m0`.`ContactTitle`, `m0`.`Country`, `m0`.`CustomerID`, `m0`.`Fax`, `m0`.`Phone`, `m0`.`Region`, `m0`.`PostalCode`
FROM (
    SELECT * FROM `Customers` WHERE `City` = 'Berlin'
) AS `m0`
""");
    }

    public override async Task Line_endings_after_Select(bool async)
    {
        await base.Line_endings_after_Select(async);

        AssertSql(
"""
SELECT `m`.`Address`, `m`.`City`, `m`.`CompanyName`, `m`.`ContactName`, `m`.`ContactTitle`, `m`.`Country`, `m`.`CustomerID`, `m`.`Fax`, `m`.`Phone`, `m`.`Region`, `m`.`PostalCode`
FROM (
    SELECT
    * FROM `Customers`
) AS `m`
WHERE `m`.`City` = 'Seattle'
""");
    }

    public override async Task SqlQueryRaw_in_subquery_with_dbParameter(bool async)
    {
        await base.SqlQueryRaw_in_subquery_with_dbParameter(async);

        AssertSql(
"""
@city='London' (Nullable = false)

SELECT `m`.`CustomerID`, `m`.`EmployeeID`, `m`.`Freight`, `m`.`OrderDate`, `m`.`OrderID`, `m`.`RequiredDate`, `m`.`ShipAddress`, `m`.`ShipCity`, `m`.`ShipCountry`, `m`.`ShipName`, `m`.`ShipPostalCode`, `m`.`ShipRegion`, `m`.`ShipVia`, `m`.`ShippedDate`
FROM (
    SELECT * FROM `Orders`
) AS `m`
WHERE `m`.`CustomerID` IN (
    SELECT `m0`.`CustomerID`
    FROM (
        SELECT * FROM `Customers` WHERE `City` = @city
    ) AS `m0`
)
""");
    }

    public override async Task SqlQueryRaw_in_subquery_with_positional_dbParameter_without_name(bool async)
    {
        await base.SqlQueryRaw_in_subquery_with_positional_dbParameter_without_name(async);

        AssertSql(
"""
p0='London' (Nullable = false)

SELECT `m`.`CustomerID`, `m`.`EmployeeID`, `m`.`Freight`, `m`.`OrderDate`, `m`.`OrderID`, `m`.`RequiredDate`, `m`.`ShipAddress`, `m`.`ShipCity`, `m`.`ShipCountry`, `m`.`ShipName`, `m`.`ShipPostalCode`, `m`.`ShipRegion`, `m`.`ShipVia`, `m`.`ShippedDate`
FROM (
    SELECT * FROM `Orders`
) AS `m`
WHERE `m`.`CustomerID` IN (
    SELECT `m0`.`CustomerID`
    FROM (
        SELECT * FROM `Customers` WHERE `City` = @p0
    ) AS `m0`
)
""");
    }

    public override async Task SqlQueryRaw_in_subquery_with_positional_dbParameter_with_name(bool async)
    {
        await base.SqlQueryRaw_in_subquery_with_positional_dbParameter_with_name(async);

        AssertSql(
"""
@city='London' (Nullable = false)

SELECT `m`.`CustomerID`, `m`.`EmployeeID`, `m`.`Freight`, `m`.`OrderDate`, `m`.`OrderID`, `m`.`RequiredDate`, `m`.`ShipAddress`, `m`.`ShipCity`, `m`.`ShipCountry`, `m`.`ShipName`, `m`.`ShipPostalCode`, `m`.`ShipRegion`, `m`.`ShipVia`, `m`.`ShippedDate`
FROM (
    SELECT * FROM `Orders`
) AS `m`
WHERE `m`.`CustomerID` IN (
    SELECT `m0`.`CustomerID`
    FROM (
        SELECT * FROM `Customers` WHERE `City` = @city
    ) AS `m0`
)
""");
    }

    public override async Task SqlQueryRaw_with_dbParameter_mixed_in_subquery(bool async)
    {
        await base.SqlQueryRaw_with_dbParameter_mixed_in_subquery(async);

        AssertSql(
"""
p0='London' (Size = 4000)
@title='Sales Representative' (Nullable = false)

SELECT `m`.`CustomerID`, `m`.`EmployeeID`, `m`.`Freight`, `m`.`OrderDate`, `m`.`OrderID`, `m`.`RequiredDate`, `m`.`ShipAddress`, `m`.`ShipCity`, `m`.`ShipCountry`, `m`.`ShipName`, `m`.`ShipPostalCode`, `m`.`ShipRegion`, `m`.`ShipVia`, `m`.`ShippedDate`
FROM (
    SELECT * FROM `Orders`
) AS `m`
WHERE `m`.`CustomerID` IN (
    SELECT `m0`.`CustomerID`
    FROM (
        SELECT * FROM `Customers` WHERE `City` = @p0 AND `ContactTitle` = @title
    ) AS `m0`
)
""",
                //
                """
@city='London' (Nullable = false)
p1='Sales Representative' (Size = 4000)

SELECT `m`.`CustomerID`, `m`.`EmployeeID`, `m`.`Freight`, `m`.`OrderDate`, `m`.`OrderID`, `m`.`RequiredDate`, `m`.`ShipAddress`, `m`.`ShipCity`, `m`.`ShipCountry`, `m`.`ShipName`, `m`.`ShipPostalCode`, `m`.`ShipRegion`, `m`.`ShipVia`, `m`.`ShippedDate`
FROM (
    SELECT * FROM `Orders`
) AS `m`
WHERE `m`.`CustomerID` IN (
    SELECT `m0`.`CustomerID`
    FROM (
        SELECT * FROM `Customers` WHERE `City` = @city AND `ContactTitle` = @p1
    ) AS `m0`
)
""");
    }

    public override async Task Multiple_occurrences_of_SqlQuery_with_db_parameter_adds_parameter_only_once(bool async)
    {
        await base.Multiple_occurrences_of_SqlQuery_with_db_parameter_adds_parameter_only_once(async);

        AssertSql(
"""
city='Seattle' (Nullable = false)

SELECT `m`.`Address`, `m`.`City`, `m`.`CompanyName`, `m`.`ContactName`, `m`.`ContactTitle`, `m`.`Country`, `m`.`CustomerID`, `m`.`Fax`, `m`.`Phone`, `m`.`Region`, `m`.`PostalCode`
FROM (
    SELECT * FROM `Customers` WHERE `City` = @city
) AS `m`
INTERSECT
SELECT `m0`.`Address`, `m0`.`City`, `m0`.`CompanyName`, `m0`.`ContactName`, `m0`.`ContactTitle`, `m0`.`Country`, `m0`.`CustomerID`, `m0`.`Fax`, `m0`.`Phone`, `m0`.`Region`, `m0`.`PostalCode`
FROM (
    SELECT * FROM `Customers` WHERE `City` = @city
) AS `m0`
""");
    }

    public override async Task Bad_data_error_handling_invalid_cast_key(bool async)
    {
        await base.Bad_data_error_handling_invalid_cast_key(async);

        AssertSql(
"""
SELECT `ProductName` AS `ProductID`, `ProductID` AS `ProductName`, `SupplierID`, `UnitPrice`, `UnitsInStock`, `Discontinued`
                      FROM `Products`
""");
    }

    public override async Task Bad_data_error_handling_invalid_cast(bool async)
    {
        await base.Bad_data_error_handling_invalid_cast(async);

        AssertSql(
"""
SELECT `ProductID`, `ProductName` AS `UnitPrice`, `ProductName`, `SupplierID`, `UnitsInStock`, `Discontinued`
                      FROM `Products`
""");
    }

    public override async Task Bad_data_error_handling_invalid_cast_projection(bool async)
    {
        await base.Bad_data_error_handling_invalid_cast_projection(async);

        AssertSql(
"""
SELECT `m`.`UnitPrice`
FROM (
    SELECT `ProductID`, `ProductName` AS `UnitPrice`, `ProductName`, `UnitsInStock`, `Discontinued`
                          FROM `Products`
) AS `m`
""");
    }

    public override async Task Bad_data_error_handling_invalid_cast_no_tracking(bool async)
    {
        await base.Bad_data_error_handling_invalid_cast_no_tracking(async);

        AssertSql(
"""
SELECT `ProductName` AS `ProductID`, `ProductID` AS `ProductName`, `SupplierID`, `UnitPrice`, `UnitsInStock`, `Discontinued`
                    FROM `Products`
""");
    }

    public override async Task Bad_data_error_handling_null(bool async)
    {
        await base.Bad_data_error_handling_null(async);

        AssertSql(
"""
SELECT `ProductID`, `ProductName`, `SupplierID`, `UnitPrice`, `UnitsInStock`, NULL AS `Discontinued`
                FROM `Products`
""");
    }

    public override async Task Bad_data_error_handling_null_projection(bool async)
    {
        await base.Bad_data_error_handling_null_projection(async);

        AssertSql(
"""
SELECT `m`.`Discontinued`
FROM (
    SELECT `ProductID`, `ProductName`, `SupplierID`, `UnitPrice`, `UnitsInStock`, NULL AS `Discontinued`
                              FROM `Products`
) AS `m`
""");
    }

    public override async Task Bad_data_error_handling_null_no_tracking(bool async)
    {
        await base.Bad_data_error_handling_null_no_tracking(async);

        AssertSql(
"""
SELECT `ProductID`, `ProductName`, `SupplierID`, `UnitPrice`, `UnitsInStock`, NULL AS `Discontinued`
                          FROM `Products`
""");
    }

    public override async Task SqlQueryRaw_queryable_simple_mapped_type(bool async)
    {
        await base.SqlQueryRaw_queryable_simple_mapped_type(async);

        AssertSql(
"""
SELECT * FROM `Customers` WHERE `ContactName` LIKE '%z%'
""");
    }

    public override async Task SqlQueryRaw_queryable_simple_columns_out_of_order_and_not_enough_columns_throws(bool async)
    {
        await base.SqlQueryRaw_queryable_simple_columns_out_of_order_and_not_enough_columns_throws(async);

        AssertSql(
"""
SELECT `PostalCode`, `Phone`, `Fax`, `CustomerID`, `Country`, `ContactTitle`, `ContactName`, `CompanyName`, `City`, `Address` FROM `Customers`
""");
    }

    public override async Task SqlQueryRaw_queryable_simple_different_cased_columns_and_not_enough_columns_throws(bool async)
    {
        await base.SqlQueryRaw_queryable_simple_different_cased_columns_and_not_enough_columns_throws(async);

        AssertSql(
"""
SELECT `PostalCODE`, `Phone`, `Fax`, `CustomerID`, `Country`, `ContactTitle`, `ContactName`, `CompanyName`, `City`, `Address` FROM `Customers`
""");
    }

    public override async Task SqlQueryRaw_queryable_simple_projection_not_composed(bool async)
    {
        await base.SqlQueryRaw_queryable_simple_projection_not_composed(async);

        AssertSql(
"""
SELECT `m`.`CustomerID`, `m`.`City`
FROM (
    SELECT * FROM `Customers`
) AS `m`
""");
    }

    public override void Ad_hoc_type_with_reference_navigation_throws()
    {
        base.Ad_hoc_type_with_reference_navigation_throws();

        AssertSql();
    }

    public override void Ad_hoc_type_with_collection_navigation_throws()
    {
        base.Ad_hoc_type_with_collection_navigation_throws();

        AssertSql();
    }

    public override void Ad_hoc_type_with_unmapped_property_throws()
    {
        base.Ad_hoc_type_with_unmapped_property_throws();

        AssertSql();
    }

    public override async Task SqlQueryRaw_then_String_Length(bool async)
    {
        await base.SqlQueryRaw_then_String_Length(async);

        AssertSql(
"""
SELECT `s`.`Value`
FROM (
    SELECT 'x' AS `Value` FROM `Customers`
) AS `s`
WHERE CHAR_LENGTH(`s`.`Value`) = 0
""");
    }

    public override async Task SqlQueryRaw_then_String_ToUpper_String_Length(bool async)
    {
        await base.SqlQueryRaw_then_String_ToUpper_String_Length(async);

        AssertSql(
"""
SELECT `s`.`Value`
FROM (
    SELECT 'x' AS `Value` FROM `Customers`
) AS `s`
WHERE CHAR_LENGTH(UPPER(`s`.`Value`)) = 0
""");
    }

    [SupportedServerVersionCondition(nameof(ServerVersionSupport.CommonTableExpressions))]
    public override async Task SqlQueryRaw_composed_with_common_table_expression(bool async)
    {
        await base.SqlQueryRaw_composed_with_common_table_expression(async);

        AssertSql(
"""
SELECT `m`.`Address`, `m`.`City`, `m`.`CompanyName`, `m`.`ContactName`, `m`.`ContactTitle`, `m`.`Country`, `m`.`CustomerID`, `m`.`Fax`, `m`.`Phone`, `m`.`Region`, `m`.`PostalCode`
FROM (
    WITH `Customers2` AS (
        SELECT * FROM `Customers`
    )
    SELECT * FROM `Customers2`
) AS `m`
WHERE `m`.`ContactName` LIKE '%z%'
""");
    }

    [ConditionalFact]
    public virtual void Check_all_tests_overridden()
        => MySqlTestHelpers.AssertAllMethodsOverridden(GetType());

    protected override DbParameter CreateDbParameter(string name, object value)
        => new MySqlParameter { ParameterName = name, Value = value };

    private void AssertSql(params string[] expected)
        => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);
}
