using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestModels.Northwind;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.EntityFrameworkCore.TestUtilities.Xunit;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class SimpleQueryMySqlTest : SimpleQueryTestBase<NorthwindQueryMySqlFixture<NoopModelCustomizer>>
    {
        // ReSharper disable once UnusedParameter.Local
        public SimpleQueryMySqlTest(NorthwindQueryMySqlFixture<NoopModelCustomizer> fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        public override async Task Take_Skip(bool isAsync)
        {
            await base.Take_Skip(isAsync);

            AssertSql(
                @"@__p_0='10'
@__p_1='5'

SELECT `t`.*
FROM (
    SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
    FROM `Customers` AS `c`
    ORDER BY `c`.`ContactName`
    LIMIT @__p_0
) AS `t`
ORDER BY `t`.`ContactName`
LIMIT 18446744073709551610 OFFSET @__p_1");
        }

        public override async Task Where_datetime_now(bool isAsync)
        {
            await base.Where_datetime_now(isAsync);

            AssertSql(
                @"@__myDatetime_0='2015-04-10T00:00:00' (DbType = DateTime)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE CURRENT_TIMESTAMP() <> @__myDatetime_0");
        }

        public override async Task Where_datetime_utcnow(bool isAsync)
        {
            await base.Where_datetime_utcnow(isAsync);

            AssertSql(
                @"@__myDatetime_0='2015-04-10T00:00:00' (DbType = DateTime)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE UTC_TIMESTAMP() <> @__myDatetime_0");
        }

        public override async Task Where_datetime_today(bool isAsync)
        {
            await base.Where_datetime_today(isAsync);

            AssertSql(
                @"SELECT `e`.`EmployeeID`, `e`.`City`, `e`.`Country`, `e`.`FirstName`, `e`.`ReportsTo`, `e`.`Title`
FROM `Employees` AS `e`
WHERE CONVERT(CURRENT_TIMESTAMP(), date) = CURDATE()");
        }

        public override async Task Where_datetime_date_component(bool isAsync)
        {
            await base.Where_datetime_date_component(isAsync);

            AssertSql(
                @"@__myDatetime_0='1998-05-04T00:00:00' (DbType = DateTime)

SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM `Orders` AS `o`
WHERE CONVERT(`o`.`OrderDate`, date) = @__myDatetime_0");
        }

        public override async Task Where_datetime_year_component(bool isAsync)
        {
            await base.Where_datetime_year_component(isAsync);

            AssertSql(
                @"SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM `Orders` AS `o`
WHERE EXTRACT(year FROM `o`.`OrderDate`) = 1998");
        }

        public override async Task Where_datetime_month_component(bool isAsync)
        {
            await base.Where_datetime_month_component(isAsync);

            AssertSql(
                @"SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM `Orders` AS `o`
WHERE EXTRACT(month FROM `o`.`OrderDate`) = 4");
        }

        public override async Task Where_datetime_dayOfYear_component(bool isAsync)
        {
            await base.Where_datetime_dayOfYear_component(isAsync);

            AssertSql(
                @"SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM `Orders` AS `o`
WHERE DAYOFYEAR(`o`.`OrderDate`) = 68");
        }

        public override async Task Where_datetime_day_component(bool isAsync)
        {
            await base.Where_datetime_day_component(isAsync);

            AssertSql(
                @"SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM `Orders` AS `o`
WHERE EXTRACT(day FROM `o`.`OrderDate`) = 4");
        }

        public override async Task Where_datetime_hour_component(bool isAsync)
        {
            await base.Where_datetime_hour_component(isAsync);

            AssertSql(
                @"SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM `Orders` AS `o`
WHERE EXTRACT(hour FROM `o`.`OrderDate`) = 14");
        }

        public override async Task Where_datetime_minute_component(bool isAsync)
        {
            await base.Where_datetime_minute_component(isAsync);

            AssertSql(
                @"SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM `Orders` AS `o`
WHERE EXTRACT(minute FROM `o`.`OrderDate`) = 23");
        }

        public override async Task Where_datetime_second_component(bool isAsync)
        {
            await base.Where_datetime_second_component(isAsync);

            AssertSql(
                @"SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM `Orders` AS `o`
WHERE EXTRACT(second FROM `o`.`OrderDate`) = 44");
        }

        public override async Task Where_datetime_millisecond_component(bool isAsync)
        {
            await base.Where_datetime_millisecond_component(isAsync);

            AssertSql(
                @"SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM `Orders` AS `o`");
        }

        public override async Task String_StartsWith_Literal(bool isAsync)
        {
            await base.String_StartsWith_Literal(isAsync);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`ContactName` LIKE CONCAT('M', '%') AND (LEFT(`c`.`ContactName`, CHAR_LENGTH('M')) = 'M')");
        }

        public override async Task String_StartsWith_Identity(bool isAsync)
        {
            await base.String_StartsWith_Identity(isAsync);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE (`c`.`ContactName` LIKE CONCAT(`c`.`ContactName`, '%') AND (LEFT(`c`.`ContactName`, CHAR_LENGTH(`c`.`ContactName`)) = `c`.`ContactName`)) OR (`c`.`ContactName` = '')");
        }

        public override async Task String_StartsWith_Column(bool isAsync)
        {
            await base.String_StartsWith_Column(isAsync);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE (`c`.`ContactName` LIKE CONCAT(`c`.`ContactName`, '%') AND (LEFT(`c`.`ContactName`, CHAR_LENGTH(`c`.`ContactName`)) = `c`.`ContactName`)) OR (`c`.`ContactName` = '')");
        }

        public override async Task String_StartsWith_MethodCall(bool isAsync)
        {
            await base.String_StartsWith_MethodCall(isAsync);

            AssertSql(
                @"@__LocalMethod1_0='M' (Size = 4000)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE (`c`.`ContactName` LIKE CONCAT(@__LocalMethod1_0, '%') AND (LEFT(`c`.`ContactName`, CHAR_LENGTH(@__LocalMethod1_0)) = @__LocalMethod1_0)) OR (@__LocalMethod1_0 = '')");
        }

        public override async Task String_EndsWith_Literal(bool isAsync)
        {
            await base.String_EndsWith_Literal(isAsync);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE RIGHT(`c`.`ContactName`, CHAR_LENGTH('b')) = 'b'");
        }

        public override async Task String_EndsWith_Identity(bool isAsync)
        {
            await base.String_EndsWith_Identity(isAsync);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE (RIGHT(`c`.`ContactName`, CHAR_LENGTH(`c`.`ContactName`)) = `c`.`ContactName`) OR (`c`.`ContactName` = '')");
        }

        public override async Task String_EndsWith_Column(bool isAsync)
        {
            await base.String_EndsWith_Column(isAsync);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE (RIGHT(`c`.`ContactName`, CHAR_LENGTH(`c`.`ContactName`)) = `c`.`ContactName`) OR (`c`.`ContactName` = '')");
        }

        public override async Task String_EndsWith_MethodCall(bool isAsync)
        {
            await base.String_EndsWith_MethodCall(isAsync);

            AssertSql(
                @"@__LocalMethod2_0='m' (Size = 4000)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE (RIGHT(`c`.`ContactName`, CHAR_LENGTH(@__LocalMethod2_0)) = @__LocalMethod2_0) OR (@__LocalMethod2_0 = '')");
        }

        public override async Task String_Contains_Literal(bool isAsync)
        {
            await base.String_Contains_Literal(isAsync);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE LOCATE(BINARY 'M', `c`.`ContactName`) > 0");
        }

        public override async Task String_Contains_Identity(bool isAsync)
        {
            await base.String_Contains_Identity(isAsync);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE (LOCATE(BINARY `c`.`ContactName`, `c`.`ContactName`) > 0) OR (`c`.`ContactName` = '')");
        }

        public override async Task String_Contains_Column(bool isAsync)
        {
            await base.String_Contains_Column(isAsync);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE (LOCATE(BINARY `c`.`ContactName`, `c`.`ContactName`) > 0) OR (`c`.`ContactName` = '')");
        }

        public override async Task String_Contains_MethodCall(bool isAsync)
        {
            await base.String_Contains_MethodCall(isAsync);

            AssertSql(
                @"@__LocalMethod1_0='M' (Size = 4000)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE (LOCATE(BINARY @__LocalMethod1_0, `c`.`ContactName`) > 0) OR (@__LocalMethod1_0 = '')");
        }

        public override async Task IsNullOrWhiteSpace_in_predicate(bool isAsync)
        {
            await base.IsNullOrWhiteSpace_in_predicate(isAsync);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`Region` IS NULL OR (LTRIM(RTRIM(`c`.`Region`)) = '')");
        }

        public override async Task Where_string_length(bool isAsync)
        {
            await base.Where_string_length(isAsync);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE CHAR_LENGTH(`c`.`City`) = 6");
        }

        public override async Task Where_string_indexof(bool isAsync)
        {
            await base.Where_string_indexof(isAsync);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE (LOCATE('Sea', `c`.`City`) - 1) <> -1");
        }

        public override async Task Indexof_with_emptystring(bool isAsync)
        {
            await base.Indexof_with_emptystring(isAsync);

            AssertSql(
                @"SELECT LOCATE('', `c`.`ContactName`) - 1
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` = 'ALFKI'");
        }

        public override async Task Where_string_replace(bool isAsync)
        {
            await base.Where_string_replace(isAsync);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE REPLACE(`c`.`City`, 'Sea', 'Rea') = 'Reattle'");
        }

        public override async Task Replace_with_emptystring(bool isAsync)
        {
            await base.Replace_with_emptystring(isAsync);

            AssertSql(
                @"SELECT REPLACE(`c`.`ContactName`, 'ari', '')
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` = 'ALFKI'");
        }

        public override async Task Where_string_substring(bool isAsync)
        {
            await base.Where_string_substring(isAsync);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE SUBSTRING(`c`.`City`, 2, 2) = 'ea'");
        }

        public override async Task Substring_with_zero_startindex(bool isAsync)
        {
            await base.Substring_with_zero_startindex(isAsync);

            AssertSql(
                @"SELECT SUBSTRING(`c`.`ContactName`, 1, 3)
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` = 'ALFKI'");
        }

        public override async Task Substring_with_constant(bool isAsync)
        {
            await base.Substring_with_constant(isAsync);

            AssertSql(
                @"SELECT SUBSTRING(`c`.`ContactName`, 2, 3)
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` = 'ALFKI'");
        }

        public override async Task Substring_with_closure(bool isAsync)
        {
            await base.Substring_with_closure(isAsync);

            AssertSql(
                @"@__start_0='2'

SELECT SUBSTRING(`c`.`ContactName`, @__start_0 + 1, 3)
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` = 'ALFKI'");
        }

        public override async Task Substring_with_client_eval(bool isAsync)
        {
            await base.Substring_with_client_eval(isAsync);

            AssertSql(
                @"SELECT `c`.`ContactName`
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` = 'ALFKI'");
        }

        public override async Task Substring_with_zero_length(bool isAsync)
        {
            await base.Substring_with_zero_length(isAsync);

            AssertSql(
                @"SELECT SUBSTRING(`c`.`ContactName`, 3, 0)
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` = 'ALFKI'");
        }

        public override async Task Where_math_abs1(bool isAsync)
        {
            await base.Where_math_abs1(isAsync);

            AssertSql(
                @"SELECT `od`.`OrderID`, `od`.`ProductID`, `od`.`Discount`, `od`.`Quantity`, `od`.`UnitPrice`
FROM `Order Details` AS `od`
WHERE ABS(`od`.`ProductID`) > 10");
        }

        public override async Task Where_math_abs2(bool isAsync)
        {
            await base.Where_math_abs2(isAsync);

            AssertSql(
                @"SELECT `od`.`OrderID`, `od`.`ProductID`, `od`.`Discount`, `od`.`Quantity`, `od`.`UnitPrice`
FROM `Order Details` AS `od`
WHERE ABS(`od`.`Quantity`) > 10");
        }

        public override async Task Where_math_abs_uncorrelated(bool isAsync)
        {
            await base.Where_math_abs_uncorrelated(isAsync);

            AssertSql(
                @"@__Abs_0='10'

SELECT `od`.`OrderID`, `od`.`ProductID`, `od`.`Discount`, `od`.`Quantity`, `od`.`UnitPrice`
FROM `Order Details` AS `od`
WHERE @__Abs_0 < `od`.`ProductID`");
        }

        public override async Task Select_math_round_int(bool isAsync)
        {
            await base.Select_math_round_int(isAsync);

            if (Fixture.TestStore.ServiceProvider.GetService<IMySqlOptions>()?.ServerVersion.SupportsDoubleCast ?? false)
            {
                AssertSql(
                    @"SELECT ROUND(CAST(`o`.`OrderID` AS double), 0) AS `A`
FROM `Orders` AS `o`
WHERE `o`.`OrderID` < 10250");
            }
            else
            {
                AssertSql(
                    @"SELECT ROUND((CAST(`o`.`OrderID` AS decimal(65,30)) + 0e0), 0) AS `A`
FROM `Orders` AS `o`
WHERE `o`.`OrderID` < 10250");
            }
        }

        public override async Task Where_math_min(bool isAsync)
        {
            await base.Where_math_min(isAsync);

            AssertSql(
                @"SELECT `od`.`OrderID`, `od`.`ProductID`, `od`.`Discount`, `od`.`Quantity`, `od`.`UnitPrice`
FROM `Order Details` AS `od`
WHERE `od`.`OrderID` = 11077");
        }

        public override async Task Where_math_max(bool isAsync)
        {
            await base.Where_math_max(isAsync);

            AssertSql(
                @"SELECT `od`.`OrderID`, `od`.`ProductID`, `od`.`Discount`, `od`.`Quantity`, `od`.`UnitPrice`
FROM `Order Details` AS `od`
WHERE `od`.`OrderID` = 11077");
        }

        public override async Task Where_string_to_lower(bool isAsync)
        {
            await base.Where_string_to_lower(isAsync);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE LOWER(`c`.`CustomerID`) = 'alfki'");
        }

        public override async Task Where_string_to_upper(bool isAsync)
        {
            await base.Where_string_to_upper(isAsync);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE UPPER(`c`.`CustomerID`) = 'ALFKI'");
        }

        public override async Task TrimStart_without_arguments_in_predicate(bool isAsync)
        {
            await base.TrimStart_without_arguments_in_predicate(isAsync);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE LTRIM(`c`.`ContactTitle`) = 'Owner'");
        }

        public override async Task TrimStart_with_char_argument_in_predicate(bool isAsync)
        {
            await base.TrimStart_with_char_argument_in_predicate(isAsync);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`");
        }

        public override async Task TrimStart_with_char_array_argument_in_predicate(bool isAsync)
        {
            await base.TrimStart_with_char_array_argument_in_predicate(isAsync);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`");
        }

        public override async Task TrimEnd_without_arguments_in_predicate(bool isAsync)
        {
            await base.TrimEnd_without_arguments_in_predicate(isAsync);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE RTRIM(`c`.`ContactTitle`) = 'Owner'");
        }

        public override async Task TrimEnd_with_char_argument_in_predicate(bool isAsync)
        {
            await base.TrimEnd_with_char_argument_in_predicate(isAsync);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`");
        }

        public override async Task TrimEnd_with_char_array_argument_in_predicate(bool isAsync)
        {
            await base.TrimEnd_with_char_array_argument_in_predicate(isAsync);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`");
        }

        public override async Task Trim_without_argument_in_predicate(bool isAsync)
        {
            await base.Trim_without_argument_in_predicate(isAsync);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE LTRIM(RTRIM(`c`.`ContactTitle`)) = 'Owner'");
        }

        public override async Task Trim_with_char_argument_in_predicate(bool isAsync)
        {
            await base.Trim_with_char_argument_in_predicate(isAsync);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`");
        }

        public override async Task Trim_with_char_array_argument_in_predicate(bool isAsync)
        {
            await base.Trim_with_char_array_argument_in_predicate(isAsync);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`");
        }

        public override async Task Sum_with_coalesce(bool isAsync)
        {
            await base.Sum_with_coalesce(isAsync);

            AssertSql(
                @"SELECT SUM(COALESCE(`p`.`UnitPrice`, 0.0))
FROM `Products` AS `p`
WHERE `p`.`ProductID` < 40");
        }

        public override async Task Select_datetime_year_component(bool isAsync)
        {
            await base.Select_datetime_year_component(isAsync);

            AssertSql(
                @"SELECT EXTRACT(year FROM `o`.`OrderDate`)
FROM `Orders` AS `o`");
        }

        public override async Task Select_datetime_month_component(bool isAsync)
        {
            await base.Select_datetime_month_component(isAsync);

            AssertSql(
                @"SELECT EXTRACT(month FROM `o`.`OrderDate`)
FROM `Orders` AS `o`");
        }

        public override async Task Select_datetime_day_of_year_component(bool isAsync)
        {
            await base.Select_datetime_day_of_year_component(isAsync);

            AssertSql(
                @"SELECT DAYOFYEAR(`o`.`OrderDate`)
FROM `Orders` AS `o`");
        }

        public override async Task Select_datetime_day_component(bool isAsync)
        {
            await base.Select_datetime_day_component(isAsync);

            AssertSql(
                @"SELECT EXTRACT(day FROM `o`.`OrderDate`)
FROM `Orders` AS `o`");
        }

        public override async Task Select_datetime_hour_component(bool isAsync)
        {
            await base.Select_datetime_hour_component(isAsync);

            AssertSql(
                @"SELECT EXTRACT(hour FROM `o`.`OrderDate`)
FROM `Orders` AS `o`");
        }

        public override async Task Select_datetime_minute_component(bool isAsync)
        {
            await base.Select_datetime_minute_component(isAsync);

            AssertSql(
                @"SELECT EXTRACT(minute FROM `o`.`OrderDate`)
FROM `Orders` AS `o`");
        }

        public override async Task Select_datetime_second_component(bool isAsync)
        {
            await base.Select_datetime_second_component(isAsync);

            AssertSql(
                @"SELECT EXTRACT(second FROM `o`.`OrderDate`)
FROM `Orders` AS `o`");
        }

        public override async Task Select_datetime_millisecond_component(bool isAsync)
        {
            await base.Select_datetime_millisecond_component(isAsync);

            AssertSql(
                @"SELECT `o`.`OrderDate`
FROM `Orders` AS `o`");
        }

        public override async Task Select_expression_references_are_updated_correctly_with_subquery(bool isAsync)
        {
            await base.Select_expression_references_are_updated_correctly_with_subquery(isAsync);

            AssertSql(
                @"@__nextYear_0='2017'

SELECT `t`.`c`
FROM (
    SELECT DISTINCT EXTRACT(year FROM `o`.`OrderDate`) AS `c`
    FROM `Orders` AS `o`
    WHERE `o`.`OrderDate` IS NOT NULL
) AS `t`
WHERE `t`.`c` < @__nextYear_0");
        }

        [ConditionalTheory(Skip = "issue #573")]
        [MemberData("IsAsyncData")]
        public override Task Project_single_element_from_collection_with_OrderBy_Take_and_FirstOrDefault(bool isAsync)
        {
            return base.Project_single_element_from_collection_with_OrderBy_Take_and_FirstOrDefault(isAsync);
        }

        [ConditionalTheory(Skip = "issue #573")]
        [MemberData("IsAsyncData")]
        public override Task Project_single_element_from_collection_with_OrderBy_Take_and_FirstOrDefault_with_parameter(bool isAsync)
        {
            return base.Project_single_element_from_collection_with_OrderBy_Take_and_FirstOrDefault_with_parameter(isAsync);
        }

        [ConditionalTheory(Skip = "issue #573")]
        [MemberData("IsAsyncData")]
        public override Task Project_single_element_from_collection_with_multiple_OrderBys_Take_and_FirstOrDefault(bool isAsync)
        {
            return base.Project_single_element_from_collection_with_multiple_OrderBys_Take_and_FirstOrDefault(isAsync);
        }

        [ConditionalTheory(Skip = "issue #573")]
        [MemberData("IsAsyncData")]
        public override Task Project_single_element_from_collection_with_multiple_OrderBys_Take_and_FirstOrDefault_followed_by_projection_of_length_property(bool isAsync)
        {
            return base.Project_single_element_from_collection_with_multiple_OrderBys_Take_and_FirstOrDefault_followed_by_projection_of_length_property(isAsync);
        }

        [ConditionalTheory(Skip = "issue #573")]
        [MemberData("IsAsyncData")]
        public override Task Project_single_element_from_collection_with_multiple_OrderBys_Take_and_FirstOrDefault_2(bool isAsync)
        {
            return base.Project_single_element_from_collection_with_multiple_OrderBys_Take_and_FirstOrDefault_2(isAsync);
        }

        [ConditionalTheory(Skip = "issue #573")]
        [MemberData("IsAsyncData")]
        public override Task Project_single_element_from_collection_with_OrderBy_over_navigation_Take_and_FirstOrDefault(bool isAsync)
        {
            return base.Project_single_element_from_collection_with_OrderBy_over_navigation_Take_and_FirstOrDefault(isAsync);
        }

        [ConditionalTheory(Skip = "issue #573")]
        [MemberData("IsAsyncData")]
        public override Task Where_as_queryable_expression(bool isAsync)
        {
            return base.Where_as_queryable_expression(isAsync);
        }

        [ConditionalTheory(Skip = "issue #552")]
        [MemberData("IsAsyncData")]
        public override Task Where_multiple_contains_in_subquery_with_and(bool isAsync)
        {
            return base.Where_multiple_contains_in_subquery_with_and(isAsync);
        }

        [ConditionalTheory(Skip = "issue #552")]
        [MemberData("IsAsyncData")]
        public override Task Where_multiple_contains_in_subquery_with_or(bool isAsync)
        {
            return base.Where_multiple_contains_in_subquery_with_or(isAsync);
        }

        [ConditionalTheory]
        [MemberData("IsAsyncData")]
        public async Task PadLeft_without_second_arg(bool isAsync)
        {
            await AssertSingleResult<Customer>(isAsync,
                    customer => customer.Where(r => r.CustomerID.PadLeft(2) == "AL").Count(),
                    customer => customer.Where(r => r.CustomerID.PadLeft(2) == "AL").CountAsync(),
                    asserter: (_, a) =>
                    {
                        var len = (int)a;
                        Assert.Equal(len, 1);
                        AssertSql(@"SELECT COUNT(*)
FROM `Customers` AS `r`
WHERE LPAD(`r`.`CustomerID`, 2, ' ') = 'AL'");
                    }
                );
        }

        [ConditionalTheory]
        [MemberData("IsAsyncData")]
        public async Task PadLeft_with_second_arg(bool isAsync)
        {
            await AssertSingleResult<Customer>(isAsync,
                    customer => customer.Where(r => r.CustomerID.PadLeft(3, 'x') == "AL").Count(),
                    customer => customer.Where(r => r.CustomerID.PadLeft(3, 'x') == "AL").CountAsync(),
                    asserter: (_, a) =>
                    {
                        var len = (int)a;
                        Assert.Equal(len, 0);
                        AssertSql(@"SELECT COUNT(*)
FROM `Customers` AS `r`
WHERE LPAD(`r`.`CustomerID`, 3, 'x') = 'AL'");
                    }
                );
        }

        [ConditionalTheory]
        [MemberData("IsAsyncData")]
        public async Task PadRight_without_second_arg(bool isAsync)
        {
            await AssertSingleResult<Customer>(isAsync,
                    customer => customer.Where(r => r.CustomerID.PadRight(3) == "AL").Count(),
                    customer => customer.Where(r => r.CustomerID.PadRight(3) == "AL").CountAsync(),
                    asserter: (_, a) =>
                    {
                        var len = (int)a;
                        Assert.Equal(len, 0);
                        AssertSql(@"SELECT COUNT(*)
FROM `Customers` AS `r`
WHERE RPAD(`r`.`CustomerID`, 3, ' ') = 'AL'");
                    }
                );
        }

        [ConditionalTheory]
        [MemberData("IsAsyncData")]
        public async Task PadRight_with_second_arg(bool isAsync)
        {
            await AssertSingleResult<Customer>(isAsync,
                  customer => customer.Where(r => r.CustomerID.PadRight(4, 'c') == "AL").Count(),
                  customer => customer.Where(r => r.CustomerID.PadRight(4, 'c') == "AL").CountAsync(),
                  asserter: (_, a) =>
                  {
                      var len = (int)a;
                      Assert.Equal(len, 0);
                      AssertSql(@"SELECT COUNT(*)
FROM `Customers` AS `r`
WHERE RPAD(`r`.`CustomerID`, 4, 'c') = 'AL'");
                  }
              );
        }

        [ConditionalTheory]
        [InlineData(StringComparison.OrdinalIgnoreCase, 1, false)]
        [InlineData(StringComparison.OrdinalIgnoreCase, 1, true)]
        [InlineData(StringComparison.CurrentCultureIgnoreCase, 1, false)]
        [InlineData(StringComparison.CurrentCultureIgnoreCase, 1, true)]
        [InlineData(StringComparison.InvariantCultureIgnoreCase, 1, false)]
        [InlineData(StringComparison.InvariantCultureIgnoreCase, 1, true)]
        [InlineData(StringComparison.Ordinal, 0, false)]
        [InlineData(StringComparison.Ordinal, 0, true)]
        [InlineData(StringComparison.CurrentCulture, 0, false)]
        [InlineData(StringComparison.CurrentCulture, 0, true)]
        [InlineData(StringComparison.InvariantCulture, 0, false)]
        [InlineData(StringComparison.InvariantCulture, 0, true)]
        public async Task StringEquals_with_comparison(StringComparison comparison, int expected, bool isAsync)
        {
            await AssertSingleResult<Customer>(isAsync,
                customer => customer.Where(c => c.CustomerID.Equals("anton", comparison)).Count(),
                customer => customer.Where(c => c.CustomerID.Equals("anton", comparison)).CountAsync(),
                asserter: (_, a) =>
                {
                    Assert.Equal(expected, (int)a);
                    // When the comparison parameter is not a constant, we have to use a case
                    // statement
                    AssertSql($"@__comparison_0='{comparison:D}'" + @"

SELECT COUNT(*)
FROM `Customers` AS `c`
WHERE CASE
    WHEN @__comparison_0 IN (4, 0, 2) THEN CASE
        WHEN `c`.`CustomerID` = CONVERT('anton' USING utf8mb4) COLLATE utf8mb4_bin
        THEN TRUE ELSE FALSE
    END
    ELSE CASE
        WHEN LCASE(`c`.`CustomerID`) = CONVERT(LCASE('anton') USING utf8mb4) COLLATE utf8mb4_bin
        THEN TRUE ELSE FALSE
    END
END = TRUE");
                });
        }

        [ConditionalTheory]
        [MemberData("IsAsyncData")]
        public async Task StringEquals_ordinal(bool isAsync)
        {
            await AssertSingleResult<Customer>(isAsync,
                customer =>
                    customer.Where(c => c.CustomerID.Equals("anton", StringComparison.Ordinal))
                        .Count(),
                customer =>
                    customer.Where(c => c.CustomerID.Equals("anton", StringComparison.Ordinal))
                        .CountAsync(),
                asserter: (_, a) =>
                {
                    Assert.Equal(0, (int)a);
                    AssertSql(@"SELECT COUNT(*)
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` = CONVERT('anton' USING utf8mb4) COLLATE utf8mb4_bin");
                });
        }

        [ConditionalTheory]
        [MemberData("IsAsyncData")]
        public async Task StringEquals_invariant(bool isAsync)
        {
            await AssertSingleResult<Customer>(isAsync,
                customer =>
                    customer.Where(c => c.CustomerID.Equals("anton", StringComparison.CurrentCulture))
                        .Count(),
                customer =>
                    customer.Where(c => c.CustomerID.Equals("anton", StringComparison.CurrentCulture))
                        .CountAsync(),
                asserter: (_, a) =>
                {
                    Assert.Equal(0, (int)a);
                    AssertSql(@"SELECT COUNT(*)
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` = CONVERT('anton' USING utf8mb4) COLLATE utf8mb4_bin");
                });
        }

        [ConditionalTheory]
        [MemberData("IsAsyncData")]
        public async Task StringEquals_current(bool isAsync)
        {
            await AssertSingleResult<Customer>(isAsync,
                customer =>
                    customer.Where(c => c.CustomerID.Equals("anton", StringComparison.InvariantCulture))
                        .Count(),
                customer =>
                    customer.Where(c => c.CustomerID.Equals("anton", StringComparison.InvariantCulture))
                        .CountAsync(),
                asserter: (_, a) =>
                {
                    Assert.Equal(0, (int)a);
                    AssertSql(@"SELECT COUNT(*)
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` = CONVERT('anton' USING utf8mb4) COLLATE utf8mb4_bin");
                });
        }

        [ConditionalTheory]
        [MemberData("IsAsyncData")]
        public async Task StringEquals_ordinal_ignore_case(bool isAsync)
        {
            await AssertSingleResult<Customer>(isAsync,
                customer =>
                    customer.Where(c => c.CustomerID.Equals("anton", StringComparison.OrdinalIgnoreCase))
                        .Count(),
                customer =>
                    customer.Where(c => c.CustomerID.Equals("anton", StringComparison.OrdinalIgnoreCase))
                        .CountAsync(),
                asserter: (_, a) =>
                {
                    Assert.Equal(1, (int)a);
                    AssertSql(@"SELECT COUNT(*)
FROM `Customers` AS `c`
WHERE LCASE(`c`.`CustomerID`) = CONVERT(LCASE('anton') USING utf8mb4) COLLATE utf8mb4_bin");
                });
        }

        [ConditionalTheory]
        [MemberData("IsAsyncData")]
        public async Task StringEquals_current_ignore_case(bool isAsync)
        {
            await AssertSingleResult<Customer>(isAsync,
                customer =>
                    customer.Where(c => c.CustomerID.Equals("anton", StringComparison.CurrentCultureIgnoreCase))
                        .Count(),
                customer =>
                    customer.Where(c => c.CustomerID.Equals("anton", StringComparison.CurrentCultureIgnoreCase))
                        .CountAsync(),
                asserter: (_, a) =>
                {
                    Assert.Equal(1, (int)a);
                    AssertSql(@"SELECT COUNT(*)
FROM `Customers` AS `c`
WHERE LCASE(`c`.`CustomerID`) = CONVERT(LCASE('anton') USING utf8mb4) COLLATE utf8mb4_bin");
                });
        }

        [ConditionalTheory]
        [MemberData("IsAsyncData")]
        public async Task StringEquals_invariant_ignore_case(bool isAsync)
        {
            await AssertSingleResult<Customer>(isAsync,
                customer =>
                    customer.Where(c => c.CustomerID.Equals("anton", StringComparison.InvariantCultureIgnoreCase))
                        .Count(),
                customer =>
                    customer.Where(c => c.CustomerID.Equals("anton", StringComparison.InvariantCultureIgnoreCase))
                        .CountAsync(),
                asserter: (_, a) =>
                {
                    Assert.Equal(1, (int)a);
                    AssertSql(@"SELECT COUNT(*)
FROM `Customers` AS `c`
WHERE LCASE(`c`.`CustomerID`) = CONVERT(LCASE('anton') USING utf8mb4) COLLATE utf8mb4_bin");
                });
        }

        [ConditionalTheory]
        [InlineData(StringComparison.OrdinalIgnoreCase, 1, false)]
        [InlineData(StringComparison.OrdinalIgnoreCase, 1, true)]
        [InlineData(StringComparison.CurrentCultureIgnoreCase, 1, false)]
        [InlineData(StringComparison.CurrentCultureIgnoreCase, 1, true)]
        [InlineData(StringComparison.InvariantCultureIgnoreCase, 1, false)]
        [InlineData(StringComparison.InvariantCultureIgnoreCase, 1, true)]
        [InlineData(StringComparison.Ordinal, 0, false)]
        [InlineData(StringComparison.Ordinal, 0, true)]
        [InlineData(StringComparison.CurrentCulture, 0, false)]
        [InlineData(StringComparison.CurrentCulture, 0, true)]
        [InlineData(StringComparison.InvariantCulture, 0, false)]
        [InlineData(StringComparison.InvariantCulture, 0, true)]
        public async Task StaticStringEquals_with_comparison(StringComparison comparison, int expected, bool isAsync)
        {
            await AssertSingleResult<Customer>(isAsync,
                customer => customer.Where(c => string.Equals(c.CustomerID, "anton", comparison)).Count(),
                customer => customer.Where(c => string.Equals(c.CustomerID, "anton", comparison)).CountAsync(),
                asserter: (_, a) =>
                {
                    Assert.Equal(expected, (int)a);
                    // When the comparison parameter is not a constant, we have to use a case
                    // statement
                    AssertSql($"@__comparison_0='{comparison:D}'" + @"

SELECT COUNT(*)
FROM `Customers` AS `c`
WHERE CASE
    WHEN @__comparison_0 IN (4, 0, 2) THEN CASE
        WHEN `c`.`CustomerID` = CONVERT('anton' USING utf8mb4) COLLATE utf8mb4_bin
        THEN TRUE ELSE FALSE
    END
    ELSE CASE
        WHEN LCASE(`c`.`CustomerID`) = CONVERT(LCASE('anton') USING utf8mb4) COLLATE utf8mb4_bin
        THEN TRUE ELSE FALSE
    END
END = TRUE");
                });
        }

        [ConditionalTheory]
        [MemberData("IsAsyncData")]
        public async Task StaticStringEquals_ordinal(bool isAsync)
        {
            await AssertSingleResult<Customer>(isAsync,
                customer =>
                    customer.Where(c => string.Equals(c.CustomerID, "anton", StringComparison.Ordinal))
                        .Count(),
                customer =>
                    customer.Where(c => string.Equals(c.CustomerID, "anton", StringComparison.Ordinal))
                        .CountAsync(),
                asserter: (_, a) =>
                {
                    Assert.Equal(0, (int)a);
                    AssertSql(@"SELECT COUNT(*)
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` = CONVERT('anton' USING utf8mb4) COLLATE utf8mb4_bin");
                });
        }

        [ConditionalTheory]
        [MemberData("IsAsyncData")]
        public async Task StaticStringEquals_invariant(bool isAsync)
        {
            await AssertSingleResult<Customer>(isAsync,
                customer =>
                    customer.Where(c => string.Equals(c.CustomerID, "anton", StringComparison.CurrentCulture))
                        .Count(),
                customer =>
                    customer.Where(c => string.Equals(c.CustomerID, "anton", StringComparison.CurrentCulture))
                        .CountAsync(),
                asserter: (_, a) =>
                {
                    Assert.Equal(0, (int)a);
                    AssertSql(@"SELECT COUNT(*)
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` = CONVERT('anton' USING utf8mb4) COLLATE utf8mb4_bin");
                });
        }

        [ConditionalTheory]
        [MemberData("IsAsyncData")]
        public async Task StaticStringEquals_current(bool isAsync)
        {
            await AssertSingleResult<Customer>(isAsync,
                customer =>
                    customer.Where(c => string.Equals(c.CustomerID, "anton", StringComparison.InvariantCulture))
                        .Count(),
                customer =>
                    customer.Where(c => string.Equals(c.CustomerID, "anton", StringComparison.InvariantCulture))
                        .CountAsync(),
                asserter: (_, a) =>
                {
                    Assert.Equal(0, (int)a);
                    AssertSql(@"SELECT COUNT(*)
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` = CONVERT('anton' USING utf8mb4) COLLATE utf8mb4_bin");
                });
        }

        [ConditionalTheory]
        [MemberData("IsAsyncData")]
        public async Task StaticStringEquals_ordinal_ignore_case(bool isAsync)
        {
            await AssertSingleResult<Customer>(isAsync,
                customer =>
                    customer.Where(c => string.Equals(c.CustomerID, "anton", StringComparison.OrdinalIgnoreCase))
                        .Count(),
                customer =>
                    customer.Where(c => string.Equals(c.CustomerID, "anton", StringComparison.OrdinalIgnoreCase))
                        .CountAsync(),
                asserter: (_, a) =>
                {
                    Assert.Equal(1, (int)a);
                    AssertSql(@"SELECT COUNT(*)
FROM `Customers` AS `c`
WHERE LCASE(`c`.`CustomerID`) = CONVERT(LCASE('anton') USING utf8mb4) COLLATE utf8mb4_bin");
                });
        }

        [ConditionalTheory]
        [MemberData("IsAsyncData")]
        public async Task StaticStringEquals_current_ignore_case(bool isAsync)
        {
            await AssertSingleResult<Customer>(isAsync,
                customer =>
                    customer.Where(c => string.Equals(c.CustomerID, "anton", StringComparison.CurrentCultureIgnoreCase))
                        .Count(),
                customer =>
                    customer.Where(c => string.Equals(c.CustomerID, "anton", StringComparison.CurrentCultureIgnoreCase))
                        .CountAsync(),
                asserter: (_, a) =>
                {
                    Assert.Equal(1, (int)a);
                    AssertSql(@"SELECT COUNT(*)
FROM `Customers` AS `c`
WHERE LCASE(`c`.`CustomerID`) = CONVERT(LCASE('anton') USING utf8mb4) COLLATE utf8mb4_bin");
                });
        }

        [ConditionalTheory]
        [MemberData("IsAsyncData")]
        public async Task StaticStringEquals_invariant_ignore_case(bool isAsync)
        {
            await AssertSingleResult<Customer>(isAsync,
                customer =>
                    customer.Where(c => string.Equals(c.CustomerID, "anton", StringComparison.InvariantCultureIgnoreCase))
                        .Count(),
                customer =>
                    customer.Where(c => string.Equals(c.CustomerID, "anton", StringComparison.InvariantCultureIgnoreCase))
                        .CountAsync(),
                asserter: (_, a) =>
                {
                    Assert.Equal(1, (int)a);
                    AssertSql(@"SELECT COUNT(*)
FROM `Customers` AS `c`
WHERE LCASE(`c`.`CustomerID`) = CONVERT(LCASE('anton') USING utf8mb4) COLLATE utf8mb4_bin");
                });
        }

#if NETCOREAPP2_2
        [ConditionalTheory]
        [InlineData(StringComparison.OrdinalIgnoreCase, 1, false)]
        [InlineData(StringComparison.OrdinalIgnoreCase, 1, true)]
        [InlineData(StringComparison.CurrentCultureIgnoreCase, 1, false)]
        [InlineData(StringComparison.CurrentCultureIgnoreCase, 1, true)]
        [InlineData(StringComparison.InvariantCultureIgnoreCase, 1, false)]
        [InlineData(StringComparison.InvariantCultureIgnoreCase, 1, true)]
        [InlineData(StringComparison.Ordinal, 0, false)]
        [InlineData(StringComparison.Ordinal, 0, true)]
        [InlineData(StringComparison.CurrentCulture, 0, false)]
        [InlineData(StringComparison.CurrentCulture, 0, true)]
        [InlineData(StringComparison.InvariantCulture, 0, false)]
        [InlineData(StringComparison.InvariantCulture, 0, true)]
        public async Task StringContains_with_comparison(StringComparison comparison, int expected, bool isAsync)
        {
            await AssertSingleResult<Customer>(isAsync,
                customer => customer.Where(c => c.CustomerID.Contains("nto", comparison)).Count(),
                customer => customer.Where(c => c.CustomerID.Contains("nto", comparison)).CountAsync(),
                asserter: (_, a) =>
                {
                    Assert.Equal(expected, (int)a);
                    // When the comparison parameter is not a constant, we have to use a case
                    // statement
                    AssertSql($"@__comparison_0='{comparison:D}'" + @"

SELECT COUNT(*)
FROM `Customers` AS `c`
WHERE CASE
    WHEN @__comparison_0 IN (4, 0, 2) THEN CASE
        WHEN LOCATE(CONVERT('nto' USING utf8mb4) COLLATE utf8mb4_bin, `c`.`CustomerID`) > 0
        THEN TRUE ELSE FALSE
    END
    ELSE CASE
        WHEN LOCATE(CONVERT(LCASE('nto') USING utf8mb4) COLLATE utf8mb4_bin, LCASE(`c`.`CustomerID`)) > 0
        THEN TRUE ELSE FALSE
    END
END = TRUE");
                });
        }

        [ConditionalTheory]
        [MemberData("IsAsyncData")]
        public async Task StringContains_ordinal(bool isAsync)
        {
            await AssertSingleResult<Customer>(isAsync,
                customer =>
                    customer.Where(c => c.CustomerID.Contains("nto", StringComparison.Ordinal))
                        .Count(),
                customer =>
                    customer.Where(c => c.CustomerID.Contains("nto", StringComparison.Ordinal))
                        .CountAsync(),
                asserter: (_, a) =>
                {
                    Assert.Equal(0, (int)a);
                    AssertSql(@"SELECT COUNT(*)
FROM `Customers` AS `c`
WHERE LOCATE(CONVERT('nto' USING utf8mb4) COLLATE utf8mb4_bin, `c`.`CustomerID`) > 0");
                });
        }

        [ConditionalTheory]
        [MemberData("IsAsyncData")]
        public async Task StringContains_invariant(bool isAsync)
        {
            await AssertSingleResult<Customer>(isAsync,
                customer =>
                    customer.Where(c => c.CustomerID.Contains("nto", StringComparison.CurrentCulture))
                        .Count(),
                customer =>
                    customer.Where(c => c.CustomerID.Contains("nto", StringComparison.CurrentCulture))
                        .CountAsync(),
                asserter: (_, a) =>
                {
                    Assert.Equal(0, (int)a);
                    AssertSql(@"SELECT COUNT(*)
FROM `Customers` AS `c`
WHERE LOCATE(CONVERT('nto' USING utf8mb4) COLLATE utf8mb4_bin, `c`.`CustomerID`) > 0");
                });
        }

        [ConditionalTheory]
        [MemberData("IsAsyncData")]
        public async Task StringContains_current(bool isAsync)
        {
            await AssertSingleResult<Customer>(isAsync,
                customer =>
                    customer.Where(c => c.CustomerID.Contains("nto", StringComparison.InvariantCulture))
                        .Count(),
                customer =>
                    customer.Where(c => c.CustomerID.Contains("nto", StringComparison.InvariantCulture))
                        .CountAsync(),
                asserter: (_, a) =>
                {
                    Assert.Equal(0, (int)a);
                    AssertSql(@"SELECT COUNT(*)
FROM `Customers` AS `c`
WHERE LOCATE(CONVERT('nto' USING utf8mb4) COLLATE utf8mb4_bin, `c`.`CustomerID`) > 0");
                });
        }

        [ConditionalTheory]
        [MemberData("IsAsyncData")]
        public async Task StringContains_ordinal_ignore_case(bool isAsync)
        {
            await AssertSingleResult<Customer>(isAsync,
                customer =>
                    customer.Where(c => c.CustomerID.Contains("nto", StringComparison.OrdinalIgnoreCase))
                        .Count(),
                customer =>
                    customer.Where(c => c.CustomerID.Contains("nto", StringComparison.OrdinalIgnoreCase))
                        .CountAsync(),
                asserter: (_, a) =>
                {
                    Assert.Equal(1, (int)a);
                    AssertSql(@"SELECT COUNT(*)
FROM `Customers` AS `c`
WHERE LOCATE(CONVERT(LCASE('nto') USING utf8mb4) COLLATE utf8mb4_bin, LCASE(`c`.`CustomerID`)) > 0");
                });
        }

        [ConditionalTheory]
        [MemberData("IsAsyncData")]
        public async Task StringContains_current_ignore_case(bool isAsync)
        {
            await AssertSingleResult<Customer>(isAsync,
                customer =>
                    customer.Where(c => c.CustomerID.Contains("nto", StringComparison.CurrentCultureIgnoreCase))
                        .Count(),
                customer =>
                    customer.Where(c => c.CustomerID.Contains("nto", StringComparison.CurrentCultureIgnoreCase))
                        .CountAsync(),
                asserter: (_, a) =>
                {
                    Assert.Equal(1, (int)a);
                    AssertSql(@"SELECT COUNT(*)
FROM `Customers` AS `c`
WHERE LOCATE(CONVERT(LCASE('nto') USING utf8mb4) COLLATE utf8mb4_bin, LCASE(`c`.`CustomerID`)) > 0");
                });
        }

        [ConditionalTheory]
        [MemberData("IsAsyncData")]
        public async Task StringContains_invariant_ignore_case(bool isAsync)
        {
            await AssertSingleResult<Customer>(isAsync,
                customer =>
                    customer.Where(c => c.CustomerID.Contains("nto", StringComparison.InvariantCultureIgnoreCase))
                        .Count(),
                customer =>
                    customer.Where(c => c.CustomerID.Contains("nto", StringComparison.InvariantCultureIgnoreCase))
                        .CountAsync(),
                asserter: (_, a) =>
                {
                    Assert.Equal(1, (int)a);
                    AssertSql(@"SELECT COUNT(*)
FROM `Customers` AS `c`
WHERE LOCATE(CONVERT(LCASE('nto') USING utf8mb4) COLLATE utf8mb4_bin, LCASE(`c`.`CustomerID`)) > 0");
                });
        }
#endif

        [ConditionalTheory]
        [InlineData(StringComparison.OrdinalIgnoreCase, 1, false)]
        [InlineData(StringComparison.OrdinalIgnoreCase, 1, true)]
        [InlineData(StringComparison.CurrentCultureIgnoreCase, 1, false)]
        [InlineData(StringComparison.CurrentCultureIgnoreCase, 1, true)]
        [InlineData(StringComparison.InvariantCultureIgnoreCase, 1, false)]
        [InlineData(StringComparison.InvariantCultureIgnoreCase, 1, true)]
        [InlineData(StringComparison.Ordinal, 0, false)]
        [InlineData(StringComparison.Ordinal, 0, true)]
        [InlineData(StringComparison.CurrentCulture, 0, false)]
        [InlineData(StringComparison.CurrentCulture, 0, true)]
        [InlineData(StringComparison.InvariantCulture, 0, false)]
        [InlineData(StringComparison.InvariantCulture, 0, true)]
        public async Task StringStartsWith_with_comparison(StringComparison comparison, int expected, bool isAsync)
        {
            await AssertSingleResult<Customer>(isAsync,
                customer => customer.Where(c => c.CustomerID.StartsWith("anto", comparison)).Count(),
                customer => customer.Where(c => c.CustomerID.StartsWith("anto", comparison)).CountAsync(),
                asserter: (_, a) =>
                {
                    Assert.Equal(expected, (int)a);
                    // When the comparison parameter is not a constant, we have to use a case
                    // statement
                    AssertSql($"@__comparison_0='{comparison:D}'" + @"

SELECT COUNT(*)
FROM `Customers` AS `c`
WHERE CASE
    WHEN @__comparison_0 IN (4, 0, 2) THEN CASE
        WHEN `c`.`CustomerID` LIKE CONCAT('anto', '%') AND (LEFT(`c`.`CustomerID`, CHAR_LENGTH(CONVERT('anto' USING utf8mb4) COLLATE utf8mb4_bin)) = CONVERT('anto' USING utf8mb4) COLLATE utf8mb4_bin)
        THEN TRUE ELSE FALSE
    END
    ELSE CASE
        WHEN LCASE(`c`.`CustomerID`) LIKE CONCAT(CONVERT(LCASE('anto') USING utf8mb4) COLLATE utf8mb4_bin, '%') AND (LEFT(LCASE(`c`.`CustomerID`), CHAR_LENGTH(CONVERT(LCASE('anto') USING utf8mb4) COLLATE utf8mb4_bin)) = CONVERT(LCASE('anto') USING utf8mb4) COLLATE utf8mb4_bin)
        THEN TRUE ELSE FALSE
    END
END = TRUE");
                });
        }

        [ConditionalTheory]
        [MemberData("IsAsyncData")]
        public async Task StringStartsWith_ordinal(bool isAsync)
        {
            await AssertSingleResult<Customer>(isAsync,
                customer =>
                    customer.Where(c => c.CustomerID.StartsWith("anto", StringComparison.Ordinal))
                        .Count(),
                customer =>
                    customer.Where(c => c.CustomerID.StartsWith("anto", StringComparison.Ordinal))
                        .CountAsync(),
                asserter: (_, a) =>
                {
                    Assert.Equal(0, (int)a);
                    AssertSql(@"SELECT COUNT(*)
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` LIKE CONCAT('anto', '%') AND " +
                        "(LEFT(`c`.`CustomerID`, CHAR_LENGTH(CONVERT('anto' USING utf8mb4) COLLATE utf8mb4_bin)) = CONVERT('anto' USING utf8mb4) COLLATE utf8mb4_bin)");
                });
        }

        [ConditionalTheory]
        [MemberData("IsAsyncData")]
        public async Task StringStartsWith_invariant(bool isAsync)
        {
            await AssertSingleResult<Customer>(isAsync,
                customer =>
                    customer.Where(c => c.CustomerID.StartsWith("anto", StringComparison.CurrentCulture))
                        .Count(),
                customer =>
                    customer.Where(c => c.CustomerID.StartsWith("anto", StringComparison.CurrentCulture))
                        .CountAsync(),
                asserter: (_, a) =>
                {
                    Assert.Equal(0, (int)a);
                    AssertSql(@"SELECT COUNT(*)
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` LIKE CONCAT('anto', '%') AND " +
                        "(LEFT(`c`.`CustomerID`, CHAR_LENGTH(CONVERT('anto' USING utf8mb4) COLLATE utf8mb4_bin)) = CONVERT('anto' USING utf8mb4) COLLATE utf8mb4_bin)");
                });
        }

        [ConditionalTheory]
        [MemberData("IsAsyncData")]
        public async Task StringStartsWith_current(bool isAsync)
        {
            await AssertSingleResult<Customer>(isAsync,
                customer =>
                    customer.Where(c => c.CustomerID.StartsWith("anto", StringComparison.InvariantCulture))
                        .Count(),
                customer =>
                    customer.Where(c => c.CustomerID.StartsWith("anto", StringComparison.InvariantCulture))
                        .CountAsync(),
                asserter: (_, a) =>
                {
                    Assert.Equal(0, (int)a);
                    AssertSql(@"SELECT COUNT(*)
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` LIKE CONCAT('anto', '%') AND " +
                        "(LEFT(`c`.`CustomerID`, CHAR_LENGTH(CONVERT('anto' USING utf8mb4) COLLATE utf8mb4_bin)) = CONVERT('anto' USING utf8mb4) COLLATE utf8mb4_bin)");
                });
        }

        [ConditionalTheory]
        [MemberData("IsAsyncData")]
        public async Task StringStartsWith_ordinal_ignore_case(bool isAsync)
        {
            await AssertSingleResult<Customer>(isAsync,
                customer =>
                    customer.Where(c => c.CustomerID.StartsWith("anto", StringComparison.OrdinalIgnoreCase))
                        .Count(),
                customer =>
                    customer.Where(c => c.CustomerID.StartsWith("anto", StringComparison.OrdinalIgnoreCase))
                        .CountAsync(),
                asserter: (_, a) =>
                {
                    Assert.Equal(1, (int)a);
                    AssertSql(@"SELECT COUNT(*)
FROM `Customers` AS `c`
WHERE LCASE(`c`.`CustomerID`) LIKE CONCAT(CONVERT(LCASE('anto') USING utf8mb4) COLLATE utf8mb4_bin, '%') AND " +
                        "(LEFT(LCASE(`c`.`CustomerID`), CHAR_LENGTH(CONVERT(LCASE('anto') USING utf8mb4) COLLATE utf8mb4_bin)) = CONVERT(LCASE('anto') USING utf8mb4) COLLATE utf8mb4_bin)");
                });
        }

        [ConditionalTheory]
        [MemberData("IsAsyncData")]
        public async Task StringStartsWith_current_ignore_case(bool isAsync)
        {
            await AssertSingleResult<Customer>(isAsync,
                customer =>
                    customer.Where(c => c.CustomerID.StartsWith("anto", StringComparison.CurrentCultureIgnoreCase))
                        .Count(),
                customer =>
                    customer.Where(c => c.CustomerID.StartsWith("anto", StringComparison.CurrentCultureIgnoreCase))
                        .CountAsync(),
                asserter: (_, a) =>
                {
                    Assert.Equal(1, (int)a);
                    AssertSql(@"SELECT COUNT(*)
FROM `Customers` AS `c`
WHERE LCASE(`c`.`CustomerID`) LIKE CONCAT(CONVERT(LCASE('anto') USING utf8mb4) COLLATE utf8mb4_bin, '%') AND " +
                        "(LEFT(LCASE(`c`.`CustomerID`), CHAR_LENGTH(CONVERT(LCASE('anto') USING utf8mb4) COLLATE utf8mb4_bin)) = CONVERT(LCASE('anto') USING utf8mb4) COLLATE utf8mb4_bin)");
                });
        }

        [ConditionalTheory]
        [MemberData("IsAsyncData")]
        public async Task StringStartsWith_invariant_ignore_case(bool isAsync)
        {
            await AssertSingleResult<Customer>(isAsync,
                customer =>
                    customer.Where(c => c.CustomerID.StartsWith("anto", StringComparison.InvariantCultureIgnoreCase))
                        .Count(),
                customer =>
                    customer.Where(c => c.CustomerID.StartsWith("anto", StringComparison.InvariantCultureIgnoreCase))
                        .CountAsync(),
                asserter: (_, a) =>
                {
                    Assert.Equal(1, (int)a);
                    AssertSql(@"SELECT COUNT(*)
FROM `Customers` AS `c`
WHERE LCASE(`c`.`CustomerID`) LIKE CONCAT(CONVERT(LCASE('anto') USING utf8mb4) COLLATE utf8mb4_bin, '%') AND " +
                        "(LEFT(LCASE(`c`.`CustomerID`), CHAR_LENGTH(CONVERT(LCASE('anto') USING utf8mb4) COLLATE utf8mb4_bin)) = CONVERT(LCASE('anto') USING utf8mb4) COLLATE utf8mb4_bin)");
                });
        }

        [ConditionalTheory]
        [InlineData(StringComparison.OrdinalIgnoreCase, 1, false)]
        [InlineData(StringComparison.OrdinalIgnoreCase, 1, true)]
        [InlineData(StringComparison.CurrentCultureIgnoreCase, 1, false)]
        [InlineData(StringComparison.CurrentCultureIgnoreCase, 1, true)]
        [InlineData(StringComparison.InvariantCultureIgnoreCase, 1, false)]
        [InlineData(StringComparison.InvariantCultureIgnoreCase, 1, true)]
        [InlineData(StringComparison.Ordinal, 0, false)]
        [InlineData(StringComparison.Ordinal, 0, true)]
        [InlineData(StringComparison.CurrentCulture, 0, false)]
        [InlineData(StringComparison.CurrentCulture, 0, true)]
        [InlineData(StringComparison.InvariantCulture, 0, false)]
        [InlineData(StringComparison.InvariantCulture, 0, true)]
        public async Task StringEndsWith_with_comparison(StringComparison comparison, int expected, bool isAsync)
        {
            await AssertSingleResult<Customer>(isAsync,
                customer => customer.Where(c => c.CustomerID.EndsWith("nton", comparison)).Count(),
                customer => customer.Where(c => c.CustomerID.EndsWith("nton", comparison)).CountAsync(),
                asserter: (_, a) =>
                {
                    Assert.Equal(expected, (int)a);
                    // When the comparison parameter is not a constant, we have to use a case
                    // statement
                    AssertSql($"@__comparison_0='{comparison:D}'" + @"

SELECT COUNT(*)
FROM `Customers` AS `c`
WHERE CASE
    WHEN @__comparison_0 IN (4, 0, 2) THEN CASE
        WHEN RIGHT(`c`.`CustomerID`, CHAR_LENGTH(CONVERT('nton' USING utf8mb4) COLLATE utf8mb4_bin)) = CONVERT('nton' USING utf8mb4) COLLATE utf8mb4_bin
        THEN TRUE ELSE FALSE
    END
    ELSE CASE
        WHEN RIGHT(LCASE(`c`.`CustomerID`), CHAR_LENGTH(CONVERT(LCASE('nton') USING utf8mb4) COLLATE utf8mb4_bin)) = CONVERT(LCASE('nton') USING utf8mb4) COLLATE utf8mb4_bin
        THEN TRUE ELSE FALSE
    END
END = TRUE");
                });
        }

        [ConditionalTheory]
        [MemberData("IsAsyncData")]
        public async Task StringEndsWith_ordinal(bool isAsync)
        {
            await AssertSingleResult<Customer>(isAsync,
                customer =>
                    customer.Where(c => c.CustomerID.EndsWith("nton", StringComparison.Ordinal))
                        .Count(),
                customer =>
                    customer.Where(c => c.CustomerID.EndsWith("nton", StringComparison.Ordinal))
                        .CountAsync(),
                asserter: (_, a) =>
                {
                    Assert.Equal(0, (int)a);
                    AssertSql(@"SELECT COUNT(*)
FROM `Customers` AS `c`
WHERE RIGHT(`c`.`CustomerID`, CHAR_LENGTH(CONVERT('nton' USING utf8mb4) COLLATE utf8mb4_bin)) = CONVERT('nton' USING utf8mb4) COLLATE utf8mb4_bin");
                });
        }

        [ConditionalTheory]
        [MemberData("IsAsyncData")]
        public async Task StringEndsWith_invariant(bool isAsync)
        {
            await AssertSingleResult<Customer>(isAsync,
                customer =>
                    customer.Where(c => c.CustomerID.EndsWith("nton", StringComparison.CurrentCulture))
                        .Count(),
                customer =>
                    customer.Where(c => c.CustomerID.EndsWith("nton", StringComparison.CurrentCulture))
                        .CountAsync(),
                asserter: (_, a) =>
                {
                    Assert.Equal(0, (int)a);
                    AssertSql(@"SELECT COUNT(*)
FROM `Customers` AS `c`
WHERE RIGHT(`c`.`CustomerID`, CHAR_LENGTH(CONVERT('nton' USING utf8mb4) COLLATE utf8mb4_bin)) = CONVERT('nton' USING utf8mb4) COLLATE utf8mb4_bin");
                });
        }

        [ConditionalTheory]
        [MemberData("IsAsyncData")]
        public async Task StringEndsWith_current(bool isAsync)
        {
            await AssertSingleResult<Customer>(isAsync,
                customer =>
                    customer.Where(c => c.CustomerID.EndsWith("nton", StringComparison.InvariantCulture))
                        .Count(),
                customer =>
                    customer.Where(c => c.CustomerID.EndsWith("nton", StringComparison.InvariantCulture))
                        .CountAsync(),
                asserter: (_, a) =>
                {
                    Assert.Equal(0, (int)a);
                    AssertSql(@"SELECT COUNT(*)
FROM `Customers` AS `c`
WHERE RIGHT(`c`.`CustomerID`, CHAR_LENGTH(CONVERT('nton' USING utf8mb4) COLLATE utf8mb4_bin)) = CONVERT('nton' USING utf8mb4) COLLATE utf8mb4_bin");
                });
        }

        [ConditionalTheory]
        [MemberData("IsAsyncData")]
        public async Task StringEndsWith_ordinal_ignore_case(bool isAsync)
        {
            await AssertSingleResult<Customer>(isAsync,
                customer =>
                    customer.Where(c => c.CustomerID.EndsWith("nton", StringComparison.OrdinalIgnoreCase))
                        .Count(),
                customer =>
                    customer.Where(c => c.CustomerID.EndsWith("nton", StringComparison.OrdinalIgnoreCase))
                        .CountAsync(),
                asserter: (_, a) =>
                {
                    Assert.Equal(1, (int)a);
                    AssertSql(@"SELECT COUNT(*)
FROM `Customers` AS `c`
WHERE RIGHT(LCASE(`c`.`CustomerID`), CHAR_LENGTH(CONVERT(LCASE('nton') USING utf8mb4) COLLATE utf8mb4_bin)) = CONVERT(LCASE('nton') USING utf8mb4) COLLATE utf8mb4_bin");
                });
        }

        [ConditionalTheory]
        [MemberData("IsAsyncData")]
        public async Task StringEndsWith_current_ignore_case(bool isAsync)
        {
            await AssertSingleResult<Customer>(isAsync,
                customer =>
                    customer.Where(c => c.CustomerID.EndsWith("nton", StringComparison.CurrentCultureIgnoreCase))
                        .Count(),
                customer =>
                    customer.Where(c => c.CustomerID.EndsWith("nton", StringComparison.CurrentCultureIgnoreCase))
                        .CountAsync(),
                asserter: (_, a) =>
                {
                    Assert.Equal(1, (int)a);
                    AssertSql(@"SELECT COUNT(*)
FROM `Customers` AS `c`
WHERE RIGHT(LCASE(`c`.`CustomerID`), CHAR_LENGTH(CONVERT(LCASE('nton') USING utf8mb4) COLLATE utf8mb4_bin)) = CONVERT(LCASE('nton') USING utf8mb4) COLLATE utf8mb4_bin");
                });
        }

        [ConditionalTheory]
        [MemberData("IsAsyncData")]
        public async Task StringEndsWith_invariant_ignore_case(bool isAsync)
        {
            await AssertSingleResult<Customer>(isAsync,
                customer =>
                    customer.Where(c => c.CustomerID.EndsWith("nton", StringComparison.InvariantCultureIgnoreCase))
                        .Count(),
                customer =>
                    customer.Where(c => c.CustomerID.EndsWith("nton", StringComparison.InvariantCultureIgnoreCase))
                        .CountAsync(),
                asserter: (_, a) =>
                {
                    Assert.Equal(1, (int)a);
                    AssertSql(@"SELECT COUNT(*)
FROM `Customers` AS `c`
WHERE RIGHT(LCASE(`c`.`CustomerID`), CHAR_LENGTH(CONVERT(LCASE('nton') USING utf8mb4) COLLATE utf8mb4_bin)) = CONVERT(LCASE('nton') USING utf8mb4) COLLATE utf8mb4_bin");
                });
        }

        [ConditionalTheory]
        [InlineData(StringComparison.OrdinalIgnoreCase, 1, false)]
        [InlineData(StringComparison.OrdinalIgnoreCase, 1, true)]
        [InlineData(StringComparison.CurrentCultureIgnoreCase, 1, false)]
        [InlineData(StringComparison.CurrentCultureIgnoreCase, 1, true)]
        [InlineData(StringComparison.InvariantCultureIgnoreCase, 1, false)]
        [InlineData(StringComparison.InvariantCultureIgnoreCase, 1, true)]
        [InlineData(StringComparison.Ordinal, 0, false)]
        [InlineData(StringComparison.Ordinal, 0, true)]
        [InlineData(StringComparison.CurrentCulture, 0, false)]
        [InlineData(StringComparison.CurrentCulture, 0, true)]
        [InlineData(StringComparison.InvariantCulture, 0, false)]
        [InlineData(StringComparison.InvariantCulture, 0, true)]
        public async Task StringIndexOf_with_comparison(StringComparison comparison, int expected, bool isAsync)
        {
            await AssertSingleResult<Customer>(isAsync,
                customer => customer.Where(c => c.CustomerID.IndexOf("nt", comparison) == 1).Count(),
                customer => customer.Where(c => c.CustomerID.IndexOf("nt", comparison) == 1).CountAsync(),
                asserter: (_, a) =>
                {
                    Assert.Equal(expected, (int)a);
                    // When the comparison parameter is not a constant, we have to use a case
                    // statement
                    AssertSql($"@__comparison_0='{comparison:D}'" + @"

SELECT COUNT(*)
FROM `Customers` AS `c`
WHERE CASE
    WHEN @__comparison_0 IN (4, 0, 2) THEN LOCATE(CONVERT('nt' USING utf8mb4) COLLATE utf8mb4_bin, `c`.`CustomerID`) - 1
    ELSE LOCATE(CONVERT(LCASE('nt') USING utf8mb4) COLLATE utf8mb4_bin, LCASE(`c`.`CustomerID`)) - 1
END = 1");
                });
        }

        [ConditionalTheory]
        [MemberData("IsAsyncData")]
        public async Task StringIndexOf_ordinal(bool isAsync)
        {
            await AssertSingleResult<Customer>(isAsync,
                customer =>
                    customer.Where(c => c.CustomerID.IndexOf("nt", StringComparison.Ordinal) == 1)
                        .Count(),
                customer =>
                    customer.Where(c => c.CustomerID.IndexOf("nt", StringComparison.Ordinal) == 1)
                        .CountAsync(),
                asserter: (_, a) =>
                {
                    Assert.Equal(0, (int)a);
                    AssertSql(@"SELECT COUNT(*)
FROM `Customers` AS `c`
WHERE (LOCATE(CONVERT('nt' USING utf8mb4) COLLATE utf8mb4_bin, `c`.`CustomerID`) - 1) = 1");
                });
        }

        [ConditionalTheory]
        [MemberData("IsAsyncData")]
        public async Task StringIndexOf_invariant(bool isAsync)
        {
            await AssertSingleResult<Customer>(isAsync,
                customer =>
                    customer.Where(c => c.CustomerID.IndexOf("nt", StringComparison.CurrentCulture) == 1)
                        .Count(),
                customer =>
                    customer.Where(c => c.CustomerID.IndexOf("nt", StringComparison.CurrentCulture) == 1)
                        .CountAsync(),
                asserter: (_, a) =>
                {
                    Assert.Equal(0, (int)a);
                    AssertSql(@"SELECT COUNT(*)
FROM `Customers` AS `c`
WHERE (LOCATE(CONVERT('nt' USING utf8mb4) COLLATE utf8mb4_bin, `c`.`CustomerID`) - 1) = 1");
                });
        }

        [ConditionalTheory]
        [MemberData("IsAsyncData")]
        public async Task StringIndexOf_current(bool isAsync)
        {
            await AssertSingleResult<Customer>(isAsync,
                customer =>
                    customer.Where(c => c.CustomerID.IndexOf("nt", StringComparison.InvariantCulture) == 1)
                        .Count(),
                customer =>
                    customer.Where(c => c.CustomerID.IndexOf("nt", StringComparison.InvariantCulture) == 1)
                        .CountAsync(),
                asserter: (_, a) =>
                {
                    Assert.Equal(0, (int)a);
                    AssertSql(@"SELECT COUNT(*)
FROM `Customers` AS `c`
WHERE (LOCATE(CONVERT('nt' USING utf8mb4) COLLATE utf8mb4_bin, `c`.`CustomerID`) - 1) = 1");
                });
        }

        [ConditionalTheory]
        [MemberData("IsAsyncData")]
        public async Task StringIndexOf_ordinal_ignore_case(bool isAsync)
        {
            await AssertSingleResult<Customer>(isAsync,
                customer =>
                    customer.Where(c => c.CustomerID.IndexOf("nt", StringComparison.OrdinalIgnoreCase) == 1)
                        .Count(),
                customer =>
                    customer.Where(c => c.CustomerID.IndexOf("nt", StringComparison.OrdinalIgnoreCase) == 1)
                        .CountAsync(),
                asserter: (_, a) =>
                {
                    Assert.Equal(1, (int)a);
                    AssertSql(@"SELECT COUNT(*)
FROM `Customers` AS `c`
WHERE (LOCATE(CONVERT(LCASE('nt') USING utf8mb4) COLLATE utf8mb4_bin, LCASE(`c`.`CustomerID`)) - 1) = 1");
                });
        }

        [ConditionalTheory]
        [MemberData("IsAsyncData")]
        public async Task StringIndexOf_current_ignore_case(bool isAsync)
        {
            await AssertSingleResult<Customer>(isAsync,
                customer =>
                    customer.Where(c => c.CustomerID.IndexOf("nt", StringComparison.CurrentCultureIgnoreCase) == 1)
                        .Count(),
                customer =>
                    customer.Where(c => c.CustomerID.IndexOf("nt", StringComparison.CurrentCultureIgnoreCase) == 1)
                        .CountAsync(),
                asserter: (_, a) =>
                {
                    Assert.Equal(1, (int)a);
                    AssertSql(@"SELECT COUNT(*)
FROM `Customers` AS `c`
WHERE (LOCATE(CONVERT(LCASE('nt') USING utf8mb4) COLLATE utf8mb4_bin, LCASE(`c`.`CustomerID`)) - 1) = 1");
                });
        }

        [ConditionalTheory]
        [MemberData("IsAsyncData")]
        public async Task StringIndexOf_invariant_ignore_case(bool isAsync)
        {
            await AssertSingleResult<Customer>(isAsync,
                customer =>
                    customer.Where(c => c.CustomerID.IndexOf("nt", StringComparison.InvariantCultureIgnoreCase) == 1)
                        .Count(),
                customer =>
                    customer.Where(c => c.CustomerID.IndexOf("nt", StringComparison.InvariantCultureIgnoreCase) == 1)
                        .CountAsync(),
                asserter: (_, a) =>
                {
                    Assert.Equal(1, (int)a);
                    AssertSql(@"SELECT COUNT(*)
FROM `Customers` AS `c`
WHERE (LOCATE(CONVERT(LCASE('nt') USING utf8mb4) COLLATE utf8mb4_bin, LCASE(`c`.`CustomerID`)) - 1) = 1");
                });
        }

        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);
    }
}
