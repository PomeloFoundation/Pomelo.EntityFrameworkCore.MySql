using System.Linq;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestModels.Northwind;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.EntityFrameworkCore.TestUtilities.Xunit;
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

        public override void Query_backed_by_database_view()
        {
            // Not present on SQLite
        }

        public override void Take_Skip()
        {
            base.Take_Skip();

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

        public override void Where_datetime_now()
        {
            base.Where_datetime_now();

            AssertSql(
                @"@__myDatetime_0='2015-04-10T00:00:00' (DbType = DateTime)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE CURRENT_TIMESTAMP() <> @__myDatetime_0");
        }

        public override void Where_datetime_utcnow()
        {
            base.Where_datetime_utcnow();

            AssertSql(
                @"@__myDatetime_0='2015-04-10T00:00:00' (DbType = DateTime)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE UTC_TIMESTAMP() <> @__myDatetime_0");
        }

        [ConditionalFact]
        public override void Where_datetime_today()
        {
            base.Where_datetime_today();

            AssertSql(
                @"SELECT `e`.`EmployeeID`, `e`.`City`, `e`.`Country`, `e`.`FirstName`, `e`.`ReportsTo`, `e`.`Title`
FROM `Employees` AS `e`
WHERE CONVERT(CURRENT_TIMESTAMP(), date) = CURDATE()");
        }

        public override void Where_datetime_date_component()
        {
            base.Where_datetime_date_component();

            AssertSql(
                @"@__myDatetime_0='1998-05-04T00:00:00' (DbType = DateTime)

SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM `Orders` AS `o`
WHERE CONVERT(`o`.`OrderDate`, date) = @__myDatetime_0");
        }

        public override void Where_datetime_year_component()
        {
            base.Where_datetime_year_component();

            AssertSql(
                @"SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM `Orders` AS `o`
WHERE EXTRACT(year FROM `o`.`OrderDate`) = 1998");
        }

        public override void Where_datetime_month_component()
        {
            base.Where_datetime_month_component();

            AssertSql(
                @"SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM `Orders` AS `o`
WHERE EXTRACT(month FROM `o`.`OrderDate`) = 4");
        }

        public override void Where_datetime_dayOfYear_component()
        {
            base.Where_datetime_dayOfYear_component();

            AssertSql(
                @"SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM `Orders` AS `o`
WHERE DAYOFYEAR(`o`.`OrderDate`) = 68");
        }

        public override void Where_datetime_day_component()
        {
            base.Where_datetime_day_component();

            AssertSql(
                @"SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM `Orders` AS `o`
WHERE EXTRACT(day FROM `o`.`OrderDate`) = 4");
        }

        public override void Where_datetime_hour_component()
        {
            base.Where_datetime_hour_component();

            AssertSql(
                @"SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM `Orders` AS `o`
WHERE EXTRACT(hour FROM `o`.`OrderDate`) = 14");
        }

        public override void Where_datetime_minute_component()
        {
            base.Where_datetime_minute_component();

            AssertSql(
                @"SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM `Orders` AS `o`
WHERE EXTRACT(minute FROM `o`.`OrderDate`) = 23");
        }

        public override void Where_datetime_second_component()
        {
            base.Where_datetime_second_component();

            AssertSql(
                @"SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM `Orders` AS `o`
WHERE EXTRACT(second FROM `o`.`OrderDate`) = 44");
        }

        public override void Where_datetime_millisecond_component()
        {
            base.Where_datetime_millisecond_component();

            AssertSql(
                @"SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM `Orders` AS `o`");
        }

        public override void String_StartsWith_Literal()
        {
            base.String_StartsWith_Literal();

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`ContactName` LIKE CONCAT(N'M', N'%') AND (LEFT(`c`.`ContactName`, CHAR_LENGTH(N'M')) = N'M')");
        }

        public override void String_StartsWith_Identity()
        {
            base.String_StartsWith_Identity();

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE (`c`.`ContactName` LIKE CONCAT(`c`.`ContactName`, N'%') AND (LEFT(`c`.`ContactName`, CHAR_LENGTH(`c`.`ContactName`)) = `c`.`ContactName`)) OR (`c`.`ContactName` = N'')");
        }

        public override void String_StartsWith_Column()
        {
            base.String_StartsWith_Column();

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE (`c`.`ContactName` LIKE CONCAT(`c`.`ContactName`, N'%') AND (LEFT(`c`.`ContactName`, CHAR_LENGTH(`c`.`ContactName`)) = `c`.`ContactName`)) OR (`c`.`ContactName` = N'')");
        }

        public override void String_StartsWith_MethodCall()
        {
            base.String_StartsWith_MethodCall();

            AssertSql(
                @"@__LocalMethod1_0='M' (Size = 4000)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE (`c`.`ContactName` LIKE CONCAT(@__LocalMethod1_0, N'%') AND (LEFT(`c`.`ContactName`, CHAR_LENGTH(@__LocalMethod1_0)) = @__LocalMethod1_0)) OR (@__LocalMethod1_0 = N'')");
        }

        public override void String_EndsWith_Literal()
        {
            base.String_EndsWith_Literal();

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE RIGHT(`c`.`ContactName`, CHAR_LENGTH(N'b')) = N'b'");
        }

        public override void String_EndsWith_Identity()
        {
            base.String_EndsWith_Identity();

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE (RIGHT(`c`.`ContactName`, CHAR_LENGTH(`c`.`ContactName`)) = `c`.`ContactName`) OR (`c`.`ContactName` = N'')");
        }

        public override void String_EndsWith_Column()
        {
            base.String_EndsWith_Column();

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE (RIGHT(`c`.`ContactName`, CHAR_LENGTH(`c`.`ContactName`)) = `c`.`ContactName`) OR (`c`.`ContactName` = N'')");
        }

        public override void String_EndsWith_MethodCall()
        {
            base.String_EndsWith_MethodCall();

            AssertSql(
                @"@__LocalMethod2_0='m' (Size = 4000)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE (RIGHT(`c`.`ContactName`, CHAR_LENGTH(@__LocalMethod2_0)) = @__LocalMethod2_0) OR (@__LocalMethod2_0 = N'')");
        }

        public override void String_Contains_Literal()
        {
            AssertQuery<Customer>(
                cs => cs.Where(c => c.ContactName.Contains("M")), // case-insensitive
                cs => cs.Where(c => c.ContactName.Contains("M") || c.ContactName.Contains("m")), // case-sensitive
                entryCount: 34);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE LOCATE(N'M', `c`.`ContactName`) > 0");
        }

        public override void String_Contains_Identity()
        {
            base.String_Contains_Identity();

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE (LOCATE(`c`.`ContactName`, `c`.`ContactName`) > 0) OR (`c`.`ContactName` = N'')");
        }

        public override void String_Contains_Column()
        {
            base.String_Contains_Column();

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE (LOCATE(`c`.`ContactName`, `c`.`ContactName`) > 0) OR (`c`.`ContactName` = N'')");
        }

        public override void String_Contains_MethodCall()
        {
            AssertQuery<Customer>(
                cs => cs.Where(c => c.ContactName.Contains(LocalMethod1())), // case-insensitive
                cs => cs.Where(c => c.ContactName.Contains(LocalMethod1().ToLower()) || c.ContactName.Contains(LocalMethod1().ToUpper())), // case-sensitive
                entryCount: 34);

            AssertSql(
                @"@__LocalMethod1_0='M' (Size = 4000)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE (LOCATE(@__LocalMethod1_0, `c`.`ContactName`) > 0) OR (@__LocalMethod1_0 = N'')");
        }

        public override void IsNullOrWhiteSpace_in_predicate()
        {
            base.IsNullOrWhiteSpace_in_predicate();

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`Region` IS NULL OR (LTRIM(RTRIM(`c`.`Region`)) = N'')");
        }

        public override void Where_string_length()
        {
            base.Where_string_length();

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE CHAR_LENGTH(`c`.`City`) = 6");
        }

        public override void Where_string_indexof()
        {
            base.Where_string_indexof();

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE (LOCATE(N'Sea', `c`.`City`) - 1) <> -1");
        }

        public override void Indexof_with_emptystring()
        {
            base.Indexof_with_emptystring();

            AssertSql(
                @"SELECT LOCATE(N'', `c`.`ContactName`) - 1
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` = N'ALFKI'");
        }

        public override void Where_string_replace()
        {
            base.Where_string_replace();

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE REPLACE(`c`.`City`, N'Sea', N'Rea') = N'Reattle'");
        }

        public override void Replace_with_emptystring()
        {
            base.Replace_with_emptystring();

            AssertSql(
                @"SELECT REPLACE(`c`.`ContactName`, N'ari', N'')
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` = N'ALFKI'");
        }

        public override void Where_string_substring()
        {
            base.Where_string_substring();

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE SUBSTRING(`c`.`City`, 2, 2) = N'ea'");
        }

        public override void Substring_with_zero_startindex()
        {
            base.Substring_with_zero_startindex();

            AssertSql(
                @"SELECT SUBSTRING(`c`.`ContactName`, 1, 3)
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` = N'ALFKI'");
        }

        public override void Substring_with_constant()
        {
            base.Substring_with_constant();

            AssertSql(
                @"SELECT SUBSTRING(`c`.`ContactName`, 2, 3)
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` = N'ALFKI'");
        }

        public override void Substring_with_closure()
        {
            base.Substring_with_closure();

            AssertSql(
                @"@__start_0='2'

SELECT SUBSTRING(`c`.`ContactName`, @__start_0 + 1, 3)
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` = N'ALFKI'");
        }

        public override void Substring_with_client_eval()
        {
            base.Substring_with_client_eval();

            AssertSql(
                @"SELECT `c`.`ContactName`
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` = N'ALFKI'");
        }

        public override void Substring_with_zero_length()
        {
            base.Substring_with_zero_length();

            AssertSql(
                @"SELECT SUBSTRING(`c`.`ContactName`, 3, 0)
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` = N'ALFKI'");
        }

        public override void Where_math_abs1()
        {
            base.Where_math_abs1();

            AssertSql(
                @"SELECT `od`.`OrderID`, `od`.`ProductID`, `od`.`Discount`, `od`.`Quantity`, `od`.`UnitPrice`
FROM `Order Details` AS `od`
WHERE ABS(`od`.`ProductID`) > 10");
        }

        public override void Where_math_abs2()
        {
            base.Where_math_abs2();

            AssertSql(
                @"SELECT `od`.`OrderID`, `od`.`ProductID`, `od`.`Discount`, `od`.`Quantity`, `od`.`UnitPrice`
FROM `Order Details` AS `od`
WHERE ABS(`od`.`Quantity`) > 10");
        }

        public override void Where_math_abs_uncorrelated()
        {
            base.Where_math_abs_uncorrelated();

            AssertSql(
                @"@__Abs_0='10'

SELECT `od`.`OrderID`, `od`.`ProductID`, `od`.`Discount`, `od`.`Quantity`, `od`.`UnitPrice`
FROM `Order Details` AS `od`
WHERE @__Abs_0 < `od`.`ProductID`");
        }

        [ConditionalFact(Skip = "issue #571")]
        public override void Select_math_round_int()
        {
            base.Select_math_round_int();

            AssertSql(
                @"SELECT round(`o`.`OrderID`) AS `A`
FROM `Orders` AS `o`
WHERE `o`.`OrderID` < 10250");
        }

        public override void Where_math_min()
        {
            base.Where_math_min();

            AssertSql(
                @"SELECT `od`.`OrderID`, `od`.`ProductID`, `od`.`Discount`, `od`.`Quantity`, `od`.`UnitPrice`
FROM `Order Details` AS `od`
WHERE `od`.`OrderID` = 11077");
        }

        public override void Where_math_max()
        {
            base.Where_math_max();

            AssertSql(
                @"SELECT `od`.`OrderID`, `od`.`ProductID`, `od`.`Discount`, `od`.`Quantity`, `od`.`UnitPrice`
FROM `Order Details` AS `od`
WHERE `od`.`OrderID` = 11077");
        }

        public override void Where_string_to_lower()
        {
            base.Where_string_to_lower();

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE LOWER(`c`.`CustomerID`) = N'alfki'");
        }

        public override void Where_string_to_upper()
        {
            base.Where_string_to_upper();

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE UPPER(`c`.`CustomerID`) = N'ALFKI'");
        }

        public override void TrimStart_without_arguments_in_predicate()
        {
            base.TrimStart_without_arguments_in_predicate();

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE LTRIM(`c`.`ContactTitle`) = N'Owner'");
        }

        public override void TrimStart_with_char_argument_in_predicate()
        {
            base.TrimStart_with_char_argument_in_predicate();

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`");
        }

        public override void TrimStart_with_char_array_argument_in_predicate()
        {
            base.TrimStart_with_char_array_argument_in_predicate();

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`");
        }

        public override void TrimEnd_without_arguments_in_predicate()
        {
            base.TrimEnd_without_arguments_in_predicate();

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE RTRIM(`c`.`ContactTitle`) = N'Owner'");
        }

        public override void TrimEnd_with_char_argument_in_predicate()
        {
            base.TrimEnd_with_char_argument_in_predicate();

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`");
        }

        public override void TrimEnd_with_char_array_argument_in_predicate()
        {
            base.TrimEnd_with_char_array_argument_in_predicate();

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`");
        }

        public override void Trim_without_argument_in_predicate()
        {
            base.Trim_without_argument_in_predicate();

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE LTRIM(RTRIM(`c`.`ContactTitle`)) = N'Owner'");
        }

        public override void Trim_with_char_argument_in_predicate()
        {
            base.Trim_with_char_argument_in_predicate();

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`");
        }

        public override void Trim_with_char_array_argument_in_predicate()
        {
            base.Trim_with_char_array_argument_in_predicate();

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`");
        }

        public override void Sum_with_coalesce()
        {
            base.Sum_with_coalesce();

            AssertSql(
                @"SELECT SUM(COALESCE(`p`.`UnitPrice`, 0.0))
FROM `Products` AS `p`
WHERE `p`.`ProductID` < 40");
        }

        public override void Select_datetime_year_component()
        {
            base.Select_datetime_year_component();

            AssertSql(
                @"SELECT EXTRACT(year FROM `o`.`OrderDate`)
FROM `Orders` AS `o`");
        }

        public override void Select_datetime_month_component()
        {
            base.Select_datetime_month_component();

            AssertSql(
                @"SELECT EXTRACT(month FROM `o`.`OrderDate`)
FROM `Orders` AS `o`");
        }

        public override void Select_datetime_day_of_year_component()
        {
            base.Select_datetime_day_of_year_component();

            AssertSql(
                @"SELECT DAYOFYEAR(`o`.`OrderDate`)
FROM `Orders` AS `o`");
        }

        public override void Select_datetime_day_component()
        {
            base.Select_datetime_day_component();

            AssertSql(
                @"SELECT EXTRACT(day FROM `o`.`OrderDate`)
FROM `Orders` AS `o`");
        }

        public override void Select_datetime_hour_component()
        {
            base.Select_datetime_hour_component();

            AssertSql(
                @"SELECT EXTRACT(hour FROM `o`.`OrderDate`)
FROM `Orders` AS `o`");
        }

        public override void Select_datetime_minute_component()
        {
            base.Select_datetime_minute_component();

            AssertSql(
                @"SELECT EXTRACT(minute FROM `o`.`OrderDate`)
FROM `Orders` AS `o`");
        }

        public override void Select_datetime_second_component()
        {
            base.Select_datetime_second_component();

            AssertSql(
                @"SELECT EXTRACT(second FROM `o`.`OrderDate`)
FROM `Orders` AS `o`");
        }

        public override void Select_datetime_millisecond_component()
        {
            base.Select_datetime_millisecond_component();

            AssertSql(
                @"SELECT `o`.`OrderDate`
FROM `Orders` AS `o`");
        }

        public override void Select_expression_references_are_updated_correctly_with_subquery()
        {
            base.Select_expression_references_are_updated_correctly_with_subquery();

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

        [ConditionalFact(Skip = "issue #571")]
        public override void Select_distinct_average()
        {
            base.Select_distinct_average();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override void Average_with_binary_expression()
        {
            base.Average_with_binary_expression();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override void Average_with_arg()
        {
            base.Average_with_arg();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override void Select_math_truncate_int()
        {
            base.Select_math_truncate_int();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override void Select_skip_average()
        {
            base.Select_skip_average();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override void Average_with_arg_expression()
        {
            base.Average_with_arg_expression();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override void Sum_on_float_column()
        {
            base.Sum_on_float_column();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override void Average_with_non_matching_types_in_projection_doesnt_produce_second_explicit_cast()
        {
            base.Average_with_non_matching_types_in_projection_doesnt_produce_second_explicit_cast();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override void Average_with_no_arg()
        {
            base.Average_with_no_arg();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override void Average_on_float_column()
        {
            base.Average_on_float_column();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override void Average_on_float_column_in_subquery_with_cast()
        {
            base.Average_on_float_column_in_subquery_with_cast();
        }

        [ConditionalFact]
        public override void Average_with_division_on_decimal_no_significant_digits()
        {
            AssertSingleResult<OrderDetail>(
                ods => ods.Average(od => od.Quantity / 2m),
                asserter: (e, a) => Assert.InRange((decimal)e - (decimal)a, -0.2m, 0.2m));
        }

        [ConditionalFact(Skip = "issue #571")]
        public override void Select_take_average()
        {
            base.Select_take_average();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override void Average_on_float_column_in_subquery()
        {
            base.Average_on_float_column_in_subquery();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override void Sum_on_float_column_in_subquery()
        {
            base.Sum_on_float_column_in_subquery();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override void Select_byte_constant()
        {
            base.Select_byte_constant();
        }

        [ConditionalFact(Skip = "issue #573")]
        public override void Project_single_element_from_collection_with_multiple_OrderBys_Take_and_FirstOrDefault_2()
        {
            base.Project_single_element_from_collection_with_multiple_OrderBys_Take_and_FirstOrDefault_2();
        }

        [ConditionalFact(Skip = "issue #573")]
        public override void Project_single_element_from_collection_with_OrderBy_Take_and_FirstOrDefault()
        {
            base.Project_single_element_from_collection_with_OrderBy_Take_and_FirstOrDefault();
        }

        [ConditionalFact(Skip = "issue #573")]
        public override void Project_single_element_from_collection_with_multiple_OrderBys_Take_and_FirstOrDefault()
        {
            base.Project_single_element_from_collection_with_multiple_OrderBys_Take_and_FirstOrDefault();
        }

        [ConditionalFact(Skip = "issue #573")]
        public override void Project_single_element_from_collection_with_OrderBy_Take_and_FirstOrDefault_with_parameter()
        {
            base.Project_single_element_from_collection_with_OrderBy_Take_and_FirstOrDefault_with_parameter();
        }

        [ConditionalFact(Skip = "issue #573")]
        public override void Project_single_element_from_collection_with_OrderBy_over_navigation_Take_and_FirstOrDefault()
        {
            base.Project_single_element_from_collection_with_OrderBy_over_navigation_Take_and_FirstOrDefault();
        }

        [ConditionalFact(Skip = "issue #552")]
        public override void Projection_containing_DateTime_subtraction()
        {
            base.Projection_containing_DateTime_subtraction();
        }

        [ConditionalFact(Skip = "issue #573")]
        public override void Where_as_queryable_expression()
        {
            base.Where_as_queryable_expression();
        }

        [ConditionalFact(Skip = "issue #552")]
        public override void Where_multiple_contains_in_subquery_with_and()
        {
            base.Where_multiple_contains_in_subquery_with_and();
        }

        [ConditionalFact(Skip = "issue #552")]
        public override void Where_multiple_contains_in_subquery_with_or()
        {
            base.Where_multiple_contains_in_subquery_with_or();
        }

        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);
    }
}
