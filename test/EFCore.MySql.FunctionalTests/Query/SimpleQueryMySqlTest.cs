using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.TestModels.Northwind;
using Microsoft.EntityFrameworkCore.TestUtilities;
using MySql.Data.MySqlClient;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities.Attributes;
using Pomelo.EntityFrameworkCore.MySql.Storage;
using Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public partial class SimpleQueryMySqlTest : SimpleQueryTestBase<NorthwindQueryMySqlFixture<NoopModelCustomizer>>
    {
        // ReSharper disable once UnusedParameter.Local
        public SimpleQueryMySqlTest(NorthwindQueryMySqlFixture<NoopModelCustomizer> fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override async Task Take_Skip(bool isAsync)
        {
            await base.Take_Skip(isAsync);

            AssertSql(
                @"@__p_0='10'
@__p_1='5'

SELECT `t`.`CustomerID`, `t`.`Address`, `t`.`City`, `t`.`CompanyName`, `t`.`ContactName`, `t`.`ContactTitle`, `t`.`Country`, `t`.`Fax`, `t`.`Phone`, `t`.`PostalCode`, `t`.`Region`
FROM (
    SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
    FROM `Customers` AS `c`
    ORDER BY `c`.`ContactName`
    LIMIT @__p_0
) AS `t`
ORDER BY `t`.`ContactName`
LIMIT 18446744073709551610 OFFSET @__p_1");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override async Task Where_datetime_now(bool isAsync)
        {
            await base.Where_datetime_now(isAsync);

            AssertSql(
                @"@__myDatetime_0='2015-04-10T00:00:00' (DbType = DateTime)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE (CURRENT_TIMESTAMP() <> @__myDatetime_0) OR CURRENT_TIMESTAMP() IS NULL");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override async Task Where_datetime_utcnow(bool isAsync)
        {
            await base.Where_datetime_utcnow(isAsync);

            AssertSql(
                @"@__myDatetime_0='2015-04-10T00:00:00' (DbType = DateTime)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE (UTC_TIMESTAMP() <> @__myDatetime_0) OR UTC_TIMESTAMP() IS NULL");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override async Task Where_datetime_today(bool isAsync)
        {
            await base.Where_datetime_today(isAsync);

            AssertSql(
                @"SELECT `e`.`EmployeeID`, `e`.`City`, `e`.`Country`, `e`.`FirstName`, `e`.`ReportsTo`, `e`.`Title`
FROM `Employees` AS `e`
WHERE (CONVERT(CURRENT_TIMESTAMP(), date) = CURDATE()) OR (CONVERT(CURRENT_TIMESTAMP(), date) IS NULL AND CURDATE() IS NULL)");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override async Task Where_datetime_date_component(bool isAsync)
        {
            await base.Where_datetime_date_component(isAsync);

            AssertSql(
                @"@__myDatetime_0='1998-05-04T00:00:00' (DbType = DateTime)

SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM `Orders` AS `o`
WHERE CONVERT(`o`.`OrderDate`, date) = @__myDatetime_0");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override async Task Where_datetime_year_component(bool isAsync)
        {
            await base.Where_datetime_year_component(isAsync);

            AssertSql(
                @"SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM `Orders` AS `o`
WHERE EXTRACT(year FROM `o`.`OrderDate`) = 1998");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override async Task Where_datetime_month_component(bool isAsync)
        {
            await base.Where_datetime_month_component(isAsync);

            AssertSql(
                @"SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM `Orders` AS `o`
WHERE EXTRACT(month FROM `o`.`OrderDate`) = 4");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override async Task Where_datetime_dayOfYear_component(bool isAsync)
        {
            await base.Where_datetime_dayOfYear_component(isAsync);

            AssertSql(
                @"SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM `Orders` AS `o`
WHERE DAYOFYEAR(`o`.`OrderDate`) = 68");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override async Task Where_datetime_day_component(bool isAsync)
        {
            await base.Where_datetime_day_component(isAsync);

            AssertSql(
                @"SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM `Orders` AS `o`
WHERE EXTRACT(day FROM `o`.`OrderDate`) = 4");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override async Task Where_datetime_hour_component(bool isAsync)
        {
            await base.Where_datetime_hour_component(isAsync);

            AssertSql(
                @"SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM `Orders` AS `o`
WHERE EXTRACT(hour FROM `o`.`OrderDate`) = 14");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override async Task Where_datetime_minute_component(bool isAsync)
        {
            await base.Where_datetime_minute_component(isAsync);

            AssertSql(
                @"SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM `Orders` AS `o`
WHERE EXTRACT(minute FROM `o`.`OrderDate`) = 23");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override async Task Where_datetime_second_component(bool isAsync)
        {
            await base.Where_datetime_second_component(isAsync);

            AssertSql(
                @"SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM `Orders` AS `o`
WHERE EXTRACT(second FROM `o`.`OrderDate`) = 44");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override async Task Where_datetime_millisecond_component(bool isAsync)
        {
            await base.Where_datetime_millisecond_component(isAsync);

            AssertSql(
                @"SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM `Orders` AS `o`
WHERE (EXTRACT(microsecond FROM `o`.`OrderDate`)) DIV (1000) = 88");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override async Task String_StartsWith_Literal(bool isAsync)
        {
            await base.String_StartsWith_Literal(isAsync);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`ContactName` IS NOT NULL AND ((`c`.`ContactName` LIKE CONCAT('M', '%')) AND (LEFT(`c`.`ContactName`, CHAR_LENGTH(CONVERT('M' USING utf8mb4) COLLATE utf8mb4_bin)) = CONVERT('M' USING utf8mb4) COLLATE utf8mb4_bin))");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override async Task String_StartsWith_Identity(bool isAsync)
        {
            await base.String_StartsWith_Identity(isAsync);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE (`c`.`ContactName` = '') OR (`c`.`ContactName` IS NOT NULL AND (`c`.`ContactName` IS NOT NULL AND ((`c`.`ContactName` LIKE CONCAT(`c`.`ContactName`, '%')) AND (LEFT(`c`.`ContactName`, CHAR_LENGTH(CONVERT(`c`.`ContactName` USING utf8mb4) COLLATE utf8mb4_bin)) = CONVERT(`c`.`ContactName` USING utf8mb4) COLLATE utf8mb4_bin))))");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override async Task String_StartsWith_Column(bool isAsync)
        {
            await base.String_StartsWith_Column(isAsync);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE (`c`.`ContactName` = '') OR (`c`.`ContactName` IS NOT NULL AND (`c`.`ContactName` IS NOT NULL AND ((`c`.`ContactName` LIKE CONCAT(`c`.`ContactName`, '%')) AND (LEFT(`c`.`ContactName`, CHAR_LENGTH(CONVERT(`c`.`ContactName` USING utf8mb4) COLLATE utf8mb4_bin)) = CONVERT(`c`.`ContactName` USING utf8mb4) COLLATE utf8mb4_bin))))");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override async Task String_StartsWith_MethodCall(bool isAsync)
        {
            await base.String_StartsWith_MethodCall(isAsync);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`ContactName` IS NOT NULL AND ((`c`.`ContactName` LIKE CONCAT('M', '%')) AND (LEFT(`c`.`ContactName`, CHAR_LENGTH(CONVERT('M' USING utf8mb4) COLLATE utf8mb4_bin)) = CONVERT('M' USING utf8mb4) COLLATE utf8mb4_bin))");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override async Task String_EndsWith_Literal(bool isAsync)
        {
            await base.String_EndsWith_Literal(isAsync);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`ContactName` IS NOT NULL AND (RIGHT(`c`.`ContactName`, CHAR_LENGTH(CONVERT('b' USING utf8mb4) COLLATE utf8mb4_bin)) = CONVERT('b' USING utf8mb4) COLLATE utf8mb4_bin)");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override async Task String_EndsWith_Identity(bool isAsync)
        {
            await base.String_EndsWith_Identity(isAsync);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE (`c`.`ContactName` = '') OR (`c`.`ContactName` IS NOT NULL AND (`c`.`ContactName` IS NOT NULL AND ((RIGHT(`c`.`ContactName`, CHAR_LENGTH(CONVERT(`c`.`ContactName` USING utf8mb4) COLLATE utf8mb4_bin)) = CONVERT(`c`.`ContactName` USING utf8mb4) COLLATE utf8mb4_bin) OR (`c`.`ContactName` = ''))))");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override async Task String_EndsWith_Column(bool isAsync)
        {
            await base.String_EndsWith_Column(isAsync);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE (`c`.`ContactName` = '') OR (`c`.`ContactName` IS NOT NULL AND (`c`.`ContactName` IS NOT NULL AND ((RIGHT(`c`.`ContactName`, CHAR_LENGTH(CONVERT(`c`.`ContactName` USING utf8mb4) COLLATE utf8mb4_bin)) = CONVERT(`c`.`ContactName` USING utf8mb4) COLLATE utf8mb4_bin) OR (`c`.`ContactName` = ''))))");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override async Task String_EndsWith_MethodCall(bool isAsync)
        {
            await base.String_EndsWith_MethodCall(isAsync);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`ContactName` IS NOT NULL AND (RIGHT(`c`.`ContactName`, CHAR_LENGTH(CONVERT('m' USING utf8mb4) COLLATE utf8mb4_bin)) = CONVERT('m' USING utf8mb4) COLLATE utf8mb4_bin)");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override async Task String_Contains_Literal(bool isAsync)
        {
            await base.String_Contains_Literal(isAsync);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE LOCATE(CONVERT('M' USING utf8mb4) COLLATE utf8mb4_bin, `c`.`ContactName`) > 0");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override async Task String_Contains_Identity(bool isAsync)
        {
            await base.String_Contains_Identity(isAsync);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE (LOCATE(CONVERT(`c`.`ContactName` USING utf8mb4) COLLATE utf8mb4_bin, `c`.`ContactName`) > 0) OR (`c`.`ContactName` = '')");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override async Task String_Contains_Column(bool isAsync)
        {
            await base.String_Contains_Column(isAsync);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE (LOCATE(CONVERT(`c`.`ContactName` USING utf8mb4) COLLATE utf8mb4_bin, `c`.`ContactName`) > 0) OR (`c`.`ContactName` = '')");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override async Task String_Contains_MethodCall(bool isAsync)
        {
            await base.String_Contains_MethodCall(isAsync);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE LOCATE(CONVERT('M' USING utf8mb4) COLLATE utf8mb4_bin, `c`.`ContactName`) > 0");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override async Task IsNullOrWhiteSpace_in_predicate(bool isAsync)
        {
            await base.IsNullOrWhiteSpace_in_predicate(isAsync);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`Region` IS NULL OR (TRIM(`c`.`Region`) = '')");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override async Task Where_string_length(bool isAsync)
        {
            await base.Where_string_length(isAsync);
            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE CHAR_LENGTH(`c`.`City`) = 6");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override async Task Where_string_indexof(bool isAsync)
        {
            await base.Where_string_indexof(isAsync);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE ((LOCATE(CONVERT('Sea' USING utf8mb4) COLLATE utf8mb4_bin, `c`.`City`) - 1) <> -1) OR LOCATE(CONVERT('Sea' USING utf8mb4) COLLATE utf8mb4_bin, `c`.`City`) IS NULL");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override async Task Indexof_with_emptystring(bool isAsync)
        {
            await base.Indexof_with_emptystring(isAsync);

            AssertSql(
                @"SELECT LOCATE(CONVERT('' USING utf8mb4) COLLATE utf8mb4_bin, `c`.`ContactName`) - 1
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` = 'ALFKI'");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override async Task Where_string_replace(bool isAsync)
        {
            await base.Where_string_replace(isAsync);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE REPLACE(`c`.`City`, 'Sea', 'Rea') = 'Reattle'");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override async Task Replace_with_emptystring(bool isAsync)
        {
            await base.Replace_with_emptystring(isAsync);

            AssertSql(
                @"SELECT REPLACE(`c`.`ContactName`, 'ari', '')
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` = 'ALFKI'");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override async Task Where_string_substring(bool isAsync)
        {
            await base.Where_string_substring(isAsync);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE SUBSTRING(`c`.`City`, 1 + 1, 2) = 'ea'");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override async Task Substring_with_zero_startindex(bool isAsync)
        {
            await base.Substring_with_zero_startindex(isAsync);

            AssertSql(
                @"SELECT SUBSTRING(`c`.`ContactName`, 0 + 1, 3)
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` = 'ALFKI'");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override async Task Substring_with_constant(bool isAsync)
        {
            await base.Substring_with_constant(isAsync);

            AssertSql(
                @"SELECT SUBSTRING(`c`.`ContactName`, 1 + 1, 3)
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` = 'ALFKI'");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override async Task Substring_with_closure(bool isAsync)
        {
            await base.Substring_with_closure(isAsync);

            AssertSql(
                @"@__start_0='2'

SELECT SUBSTRING(`c`.`ContactName`, @__start_0 + 1, 3)
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` = 'ALFKI'");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override async Task Substring_with_zero_length(bool isAsync)
        {
            await base.Substring_with_zero_length(isAsync);

            AssertSql(
                @"SELECT SUBSTRING(`c`.`ContactName`, 2 + 1, 0)
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` = 'ALFKI'");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override async Task Where_math_abs1(bool isAsync)
        {
            await base.Where_math_abs1(isAsync);

            AssertSql(
                @"SELECT `o`.`OrderID`, `o`.`ProductID`, `o`.`Discount`, `o`.`Quantity`, `o`.`UnitPrice`
FROM `Order Details` AS `o`
WHERE ABS(`o`.`ProductID`) > 10");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override async Task Where_math_abs2(bool isAsync)
        {
            await base.Where_math_abs2(isAsync);

            AssertSql(
                @"SELECT `o`.`OrderID`, `o`.`ProductID`, `o`.`Discount`, `o`.`Quantity`, `o`.`UnitPrice`
FROM `Order Details` AS `o`
WHERE ABS(`o`.`Quantity`) > 10");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override async Task Where_math_abs_uncorrelated(bool isAsync)
        {
            await base.Where_math_abs_uncorrelated(isAsync);

            AssertSql(
                @"SELECT `o`.`OrderID`, `o`.`ProductID`, `o`.`Discount`, `o`.`Quantity`, `o`.`UnitPrice`
FROM `Order Details` AS `o`
WHERE 10 < `o`.`ProductID`");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override async Task Select_math_round_int(bool isAsync)
        {
            await base.Select_math_round_int(isAsync);

            if (AppConfig.ServerVersion.SupportsDoubleCast)
            {
                AssertSql(
                    @"SELECT ROUND(CAST(`o`.`OrderID` AS double)) AS `A`
FROM `Orders` AS `o`
WHERE `o`.`OrderID` < 10250");
            }
            else
            {
                AssertSql(
                    @"SELECT ROUND((CAST(`o`.`OrderID` AS decimal(65,30)) + 0e0)) AS `A`
FROM `Orders` AS `o`
WHERE `o`.`OrderID` < 10250");
            }
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override async Task Where_math_min(bool isAsync)
        {
            await base.Where_math_min(isAsync);

            AssertSql(
                @"SELECT `o`.`OrderID`, `o`.`ProductID`, `o`.`Discount`, `o`.`Quantity`, `o`.`UnitPrice`
FROM `Order Details` AS `o`
WHERE (`o`.`OrderID` = 11077) AND (LEAST(`o`.`OrderID`, `o`.`ProductID`) = `o`.`ProductID`)");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override async Task Where_math_max(bool isAsync)
        {
            await base.Where_math_max(isAsync);

            AssertSql(
                @"SELECT `o`.`OrderID`, `o`.`ProductID`, `o`.`Discount`, `o`.`Quantity`, `o`.`UnitPrice`
FROM `Order Details` AS `o`
WHERE (`o`.`OrderID` = 11077) AND (GREATEST(`o`.`OrderID`, `o`.`ProductID`) = `o`.`OrderID`)");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override async Task Where_string_to_lower(bool isAsync)
        {
            await base.Where_string_to_lower(isAsync);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE LOWER(`c`.`CustomerID`) = 'alfki'");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override async Task Where_string_to_upper(bool isAsync)
        {
            await base.Where_string_to_upper(isAsync);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE UPPER(`c`.`CustomerID`) = 'ALFKI'");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override async Task TrimStart_without_arguments_in_predicate(bool isAsync)
        {
            await base.TrimStart_without_arguments_in_predicate(isAsync);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE TRIM(LEADING FROM `c`.`ContactTitle`) = 'Owner'");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override async Task TrimStart_with_char_argument_in_predicate(bool isAsync)
        {
            await base.TrimStart_with_char_argument_in_predicate(isAsync);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE TRIM(LEADING 'O' FROM `c`.`ContactTitle`) = 'wner'");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override Task TrimStart_with_char_array_argument_in_predicate(bool isAsync)
        {
            // MySQL only supports a string (characters in fixed order) as the parameter specifying what should be trimmed.
            // String.TrimStart has a different behavior, where any single character in any order will be trimmed.
            // Therefore, calling String.TrimStart with more than one char to trim, triggers client eval.
            return Assert.ThrowsAsync<InvalidOperationException>(() => base.TrimStart_with_char_array_argument_in_predicate(isAsync));
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override async Task TrimEnd_without_arguments_in_predicate(bool isAsync)
        {
            await base.TrimEnd_without_arguments_in_predicate(isAsync);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE TRIM(TRAILING FROM `c`.`ContactTitle`) = 'Owner'");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override async Task TrimEnd_with_char_argument_in_predicate(bool isAsync)
        {
            await base.TrimEnd_with_char_argument_in_predicate(isAsync);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE TRIM(TRAILING 'r' FROM `c`.`ContactTitle`) = 'Owne'");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override Task TrimEnd_with_char_array_argument_in_predicate(bool isAsync)
        {
            // MySQL only supports a string (characters in fixed order) as the parameter specifying what should be trimmed.
            // String.TrimEnd has a different behavior, where any single character in any order will be trimmed.
            // Therefore, calling String.TrimEnd with more than one char to trim, triggers client eval.
            return Assert.ThrowsAsync<InvalidOperationException>(() => base.TrimEnd_with_char_array_argument_in_predicate(isAsync));
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override async Task Trim_without_argument_in_predicate(bool isAsync)
        {
            await base.Trim_without_argument_in_predicate(isAsync);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE TRIM(`c`.`ContactTitle`) = 'Owner'");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override async Task Trim_with_char_argument_in_predicate(bool isAsync)
        {
            await base.Trim_with_char_argument_in_predicate(isAsync);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE TRIM('O' FROM `c`.`ContactTitle`) = 'wner'");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override Task Trim_with_char_array_argument_in_predicate(bool isAsync)
        {
            // MySQL only supports a string (characters in fixed order) as the parameter specifying what should be trimmed.
            // String.Trim has a different behavior, where any single character in any order will be trimmed.
            // Therefore, calling String.Trim with more than one char to trim, triggers client eval.
            return Assert.ThrowsAsync<InvalidOperationException>(() => base.Trim_with_char_array_argument_in_predicate(isAsync));
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override async Task Sum_with_coalesce(bool isAsync)
        {
            await base.Sum_with_coalesce(isAsync);

            AssertSql(
                @"SELECT SUM(COALESCE(`p`.`UnitPrice`, 0.0))
FROM `Products` AS `p`
WHERE `p`.`ProductID` < 40");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override async Task Select_datetime_year_component(bool isAsync)
        {
            await base.Select_datetime_year_component(isAsync);

            AssertSql(
                @"SELECT EXTRACT(year FROM `o`.`OrderDate`)
FROM `Orders` AS `o`");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override async Task Select_datetime_month_component(bool isAsync)
        {
            await base.Select_datetime_month_component(isAsync);

            AssertSql(
                @"SELECT EXTRACT(month FROM `o`.`OrderDate`)
FROM `Orders` AS `o`");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override async Task Select_datetime_day_of_year_component(bool isAsync)
        {
            await base.Select_datetime_day_of_year_component(isAsync);

            AssertSql(
                @"SELECT DAYOFYEAR(`o`.`OrderDate`)
FROM `Orders` AS `o`");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override async Task Select_datetime_day_component(bool isAsync)
        {
            await base.Select_datetime_day_component(isAsync);

            AssertSql(
                @"SELECT EXTRACT(day FROM `o`.`OrderDate`)
FROM `Orders` AS `o`");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override async Task Select_datetime_hour_component(bool isAsync)
        {
            await base.Select_datetime_hour_component(isAsync);

            AssertSql(
                @"SELECT EXTRACT(hour FROM `o`.`OrderDate`)
FROM `Orders` AS `o`");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override async Task Select_datetime_minute_component(bool isAsync)
        {
            await base.Select_datetime_minute_component(isAsync);

            AssertSql(
                @"SELECT EXTRACT(minute FROM `o`.`OrderDate`)
FROM `Orders` AS `o`");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override async Task Select_datetime_second_component(bool isAsync)
        {
            await base.Select_datetime_second_component(isAsync);

            AssertSql(
                @"SELECT EXTRACT(second FROM `o`.`OrderDate`)
FROM `Orders` AS `o`");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override async Task Select_datetime_millisecond_component(bool isAsync)
        {
            await base.Select_datetime_millisecond_component(isAsync);

            AssertSql(
                @"SELECT (EXTRACT(microsecond FROM `o`.`OrderDate`)) DIV (1000)
FROM `Orders` AS `o`");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override async Task Select_expression_references_are_updated_correctly_with_subquery(bool isAsync)
        {
            await base.Select_expression_references_are_updated_correctly_with_subquery(isAsync);

            AssertSql(
                @"@__nextYear_0='2017'

SELECT DISTINCT EXTRACT(year FROM `o`.`OrderDate`)
FROM `Orders` AS `o`
WHERE `o`.`OrderDate` IS NOT NULL AND (EXTRACT(year FROM `o`.`OrderDate`) < @__nextYear_0)");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override Task Where_equals_method_string_with_ignore_case(bool isAsync)
        {
            // We have an implementation for this and therefore don't throw.
            return AssertQuery(
                isAsync,
                ss => ss.Set<Customer>().Where(c => c.City.Equals("London", StringComparison.OrdinalIgnoreCase)),
                entryCount: 6);
        }

        [ConditionalTheory(Skip = "issue #573")]
        [MemberData(nameof(IsAsyncData))]
        public override Task Project_single_element_from_collection_with_OrderBy_Take_and_FirstOrDefault(bool isAsync)
        {
            return base.Project_single_element_from_collection_with_OrderBy_Take_and_FirstOrDefault(isAsync);
        }

        [ConditionalTheory(Skip = "issue #573")]
        [MemberData(nameof(IsAsyncData))]
        public override Task Project_single_element_from_collection_with_OrderBy_Take_and_FirstOrDefault_with_parameter(bool isAsync)
        {
            return base.Project_single_element_from_collection_with_OrderBy_Take_and_FirstOrDefault_with_parameter(isAsync);
        }

        [ConditionalTheory(Skip = "issue #573")]
        [MemberData(nameof(IsAsyncData))]
        public override Task Project_single_element_from_collection_with_multiple_OrderBys_Take_and_FirstOrDefault(bool isAsync)
        {
            return base.Project_single_element_from_collection_with_multiple_OrderBys_Take_and_FirstOrDefault(isAsync);
        }

        [ConditionalTheory(Skip = "issue #573")]
        [MemberData(nameof(IsAsyncData))]
        public override Task Project_single_element_from_collection_with_multiple_OrderBys_Take_and_FirstOrDefault_followed_by_projection_of_length_property(bool isAsync)
        {
            return base.Project_single_element_from_collection_with_multiple_OrderBys_Take_and_FirstOrDefault_followed_by_projection_of_length_property(isAsync);
        }

        [ConditionalTheory(Skip = "issue #573")]
        [MemberData(nameof(IsAsyncData))]
        public override Task Project_single_element_from_collection_with_multiple_OrderBys_Take_and_FirstOrDefault_2(bool isAsync)
        {
            return base.Project_single_element_from_collection_with_multiple_OrderBys_Take_and_FirstOrDefault_2(isAsync);
        }

        [ConditionalTheory(Skip = "issue #573")]
        [MemberData(nameof(IsAsyncData))]
        public override Task Project_single_element_from_collection_with_OrderBy_over_navigation_Take_and_FirstOrDefault(bool isAsync)
        {
            return base.Project_single_element_from_collection_with_OrderBy_over_navigation_Take_and_FirstOrDefault(isAsync);
        }

        [ConditionalTheory(Skip = "issue #573")]
        [MemberData(nameof(IsAsyncData))]
        public override Task Where_as_queryable_expression(bool isAsync)
        {
            return base.Where_as_queryable_expression(isAsync);
        }

        [ConditionalTheory(Skip = "issue #552")]
        [MemberData(nameof(IsAsyncData))]
        public override Task Where_multiple_contains_in_subquery_with_and(bool isAsync)
        {
            return base.Where_multiple_contains_in_subquery_with_and(isAsync);
        }

        [ConditionalTheory(Skip = "issue #552")]
        [MemberData(nameof(IsAsyncData))]
        public override Task Where_multiple_contains_in_subquery_with_or(bool isAsync)
        {
            return base.Where_multiple_contains_in_subquery_with_or(isAsync);
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override Task Intersect(bool isAsync)
        {
            // INTERSECT is not natively supported by MySQL.
            return Assert.ThrowsAsync<InvalidOperationException>(() => base.Intersect(isAsync));
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override Task Intersect_nested(bool isAsync)
        {
            // INTERSECT is not natively supported by MySQL.
            return Assert.ThrowsAsync<InvalidOperationException>(() => base.Intersect_nested(isAsync));
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override Task Intersect_non_entity(bool isAsync)
        {
            // INTERSECT is not natively supported by MySQL.
            return Assert.ThrowsAsync<InvalidOperationException>(() => base.Intersect_non_entity(isAsync));
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override Task Union_Intersect(bool isAsync)
        {
            // INTERSECT is not natively supported by MySQL.
            return Assert.ThrowsAsync<InvalidOperationException>(() => base.Union_Intersect(isAsync));
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override Task Except(bool isAsync)
        {
            // EXCEPT is not natively supported by MySQL.
            return Assert.ThrowsAsync<InvalidOperationException>(() => base.Except(isAsync));
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override Task Except_simple_followed_by_projecting_constant(bool isAsync)
        {
            // EXCEPT is not natively supported by MySQL.
            return Assert.ThrowsAsync<InvalidOperationException>(() => base.Except_simple_followed_by_projecting_constant(isAsync));
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override Task Except_nested(bool isAsync)
        {
            // EXCEPT is not natively supported by MySQL.
            return Assert.ThrowsAsync<InvalidOperationException>(() => base.Except_nested(isAsync));
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override Task Except_non_entity(bool isAsync)
        {
            // EXCEPT is not natively supported by MySQL.
            return Assert.ThrowsAsync<InvalidOperationException>(() => base.Except_non_entity(isAsync));
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override Task Select_Except_reference_projection(bool isAsync)
        {
            // EXCEPT is not natively supported by MySQL.
            return Assert.ThrowsAsync<InvalidOperationException>(() => base.Select_Except_reference_projection(isAsync));
        }

        [SupportedServerVersionTheory(ServerVersion.CrossApplySupportKey)]
        [MemberData(nameof(IsAsyncData))]
        public override Task SelectMany_correlated_with_outer_1(bool isAsync)
        {
            return base.SelectMany_correlated_with_outer_1(isAsync);
        }

        // [SupportedServerVersionTheory(ServerVersion.CrossApplySupportKey)]
        [ConditionalTheory(Skip = "Leads to a different result set in CI on Linux with MySQL 8.0.17. TODO: Needs investigation!")]
        [MemberData(nameof(IsAsyncData))]
        public override Task SelectMany_correlated_with_outer_2(bool isAsync)
        {
            return base.SelectMany_correlated_with_outer_2(isAsync);
        }

        [SupportedServerVersionTheory(ServerVersion.OuterApplySupportKey)]
        [MemberData(nameof(IsAsyncData))]
        public override Task SelectMany_correlated_with_outer_3(bool isAsync)
        {
            return base.SelectMany_correlated_with_outer_3(isAsync);
        }

        // [SupportedServerVersionTheory(ServerVersion.CrossApplySupportKey)]
        [ConditionalTheory(Skip = "Leads to a different result set in CI on Linux with MySQL 8.0.17. TODO: Needs investigation!")]
        [MemberData(nameof(IsAsyncData))]
        public override Task SelectMany_correlated_with_outer_4(bool isAsync)
        {
            return base.SelectMany_correlated_with_outer_4(isAsync);
        }

        [SupportedServerVersionTheory(ServerVersion.OuterApplySupportKey)]
        [MemberData(nameof(IsAsyncData))]
        public override Task Project_single_element_from_collection_with_OrderBy_over_navigation_Take_and_FirstOrDefault_2(bool isAsync)
        {
            return base.Project_single_element_from_collection_with_OrderBy_over_navigation_Take_and_FirstOrDefault_2(isAsync);
        }

        [SupportedServerVersionFact(ServerVersion.OuterApplySupportKey)]
        public override void Select_nested_collection_multi_level()
        {
            base.Select_nested_collection_multi_level();
        }

        [SupportedServerVersionTheory(ServerVersion.OuterApplySupportKey)]
        [MemberData(nameof(IsAsyncData))]
        public override Task Project_single_element_from_collection_with_OrderBy_Distinct_and_FirstOrDefault_followed_by_projecting_length(bool isAsync)
        {
            return base.Project_single_element_from_collection_with_OrderBy_Distinct_and_FirstOrDefault_followed_by_projecting_length(isAsync);
        }

        [SupportedServerVersionTheory(ServerVersion.OuterApplySupportKey)]
        [MemberData(nameof(IsAsyncData))]
        public override Task Project_single_element_from_collection_with_OrderBy_Take_and_SingleOrDefault(bool isAsync)
        {
            return base.Project_single_element_from_collection_with_OrderBy_Take_and_SingleOrDefault(isAsync);
        }

        [ConditionalTheory(Skip = "TODO: MySQL does not seem to allow an ORDER BY or LIMIT clause directly in a SELECT statement that is part of a UNION.")]
        [MemberData(nameof(IsAsyncData))]
        public override Task Union_Take_Union_Take(bool isAsync)
        {
            // TODO: MySQL does not seem to allow an ORDER BY or LIMIT clause directly in a SELECT statement that is part of a UNION.
            //       To make this work, the SELECT statement containing the ORDER BY and/or LIMIT clause needs to be wrapped by another
            //       SELECT statement.
            return base.Union_Take_Union_Take(isAsync);
        }

        [SupportedServerVersionTheory(ServerVersion.WindowFunctionsSupportKey)]
        [MemberData(nameof(IsAsyncData))]
        public override Task SelectMany_Joined_Take(bool isAsync)
        {
            return base.SelectMany_Joined_Take(isAsync);
        }

        [SupportedServerVersionTheory(ServerVersion.OuterApplySupportKey)]
        [MemberData(nameof(IsAsyncData))]
        public override Task Complex_nested_query_doesnt_try_binding_to_grandparent_when_parent_returns_complex_result(bool isAsync)
        {
            // MySql.Data.MySqlClient.MySqlException: Reference 'CustomerID' not supported (forward reference in item list)
            return Assert.ThrowsAsync<MySqlException>(() => base.Complex_nested_query_doesnt_try_binding_to_grandparent_when_parent_returns_complex_result(isAsync));
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override Task Member_binding_after_ctor_arguments_fails_with_client_eval(bool isAsync)
        {
            return AssertTranslationFailed(() => base.Member_binding_after_ctor_arguments_fails_with_client_eval(isAsync));
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override Task Union_Select_scalar(bool isAsync)
        {
            return AssertTranslationFailed(() => base.Union_Select_scalar(isAsync));
        }

        [SupportedServerVersionTheory(ServerVersion.OuterApplySupportKey)]
        [MemberData(nameof(IsAsyncData))]
        public override Task AsQueryable_in_query_server_evals(bool isAsync)
        {
            return base.AsQueryable_in_query_server_evals(isAsync);
        }

        [SupportedServerVersionTheory(ServerVersion.CrossApplySupportKey)]
        [MemberData(nameof(IsAsyncData))]
        public override Task SelectMany_correlated_subquery_hard(bool isAsync)
        {
            return base.SelectMany_correlated_subquery_hard(isAsync);
        }

        [SupportedServerVersionTheory(ServerVersion.CrossApplySupportKey)]
        [MemberData(nameof(IsAsyncData))]
        public override Task SelectMany_whose_selector_references_outer_source(bool isAsync)
        {
            return base.SelectMany_whose_selector_references_outer_source(isAsync);
        }

        [SupportedServerVersionTheory(ServerVersion.CrossApplySupportKey)]
        [MemberData(nameof(IsAsyncData))]
        public override Task SelectMany_with_collection_being_correlated_subquery_which_references_inner_and_outer_entity(bool isAsync)
        {
            return base.SelectMany_with_collection_being_correlated_subquery_which_references_inner_and_outer_entity(isAsync);
        }

        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);
    }
}
