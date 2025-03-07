using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestModels.Northwind;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.Tests.TestUtilities.Attributes;
using Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class NorthwindWhereQueryMySqlTest : NorthwindWhereQueryRelationalTestBase<
        NorthwindQueryMySqlFixture<NoopModelCustomizer>>
    {
        public NorthwindWhereQueryMySqlTest(
            NorthwindQueryMySqlFixture<NoopModelCustomizer> fixture,
            ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            ClearLog();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        [ConditionalTheory]
        public override async Task Where_datetime_now(bool async)
        {
            await base.Where_datetime_now(async);

            AssertSql(
                @"@__myDatetime_0='2015-04-10T00:00:00.0000000' (DbType = DateTime)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE CURRENT_TIMESTAMP(6) <> @__myDatetime_0");
        }

        [ConditionalTheory]
        public override async Task Where_datetime_utcnow(bool async)
        {
            await base.Where_datetime_utcnow(async);

            AssertSql(
                @"@__myDatetime_0='2015-04-10T00:00:00.0000000' (DbType = DateTime)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE UTC_TIMESTAMP(6) <> @__myDatetime_0");
        }

        [ConditionalTheory]
        public override async Task Where_datetime_today(bool async)
        {
            await base.Where_datetime_today(async);

            AssertSql(
                @"SELECT `e`.`EmployeeID`, `e`.`City`, `e`.`Country`, `e`.`FirstName`, `e`.`ReportsTo`, `e`.`Title`
FROM `Employees` AS `e`
WHERE CONVERT(CURRENT_TIMESTAMP(6), date) = CURDATE()");
        }

        [ConditionalTheory]
        public override async Task Where_datetime_date_component(bool async)
        {
            await base.Where_datetime_date_component(async);

            AssertSql(
                @"@__myDatetime_0='1998-05-04T00:00:00.0000000' (DbType = DateTime)

SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM `Orders` AS `o`
WHERE CONVERT(`o`.`OrderDate`, date) = @__myDatetime_0");
        }

        [ConditionalTheory]
        public override async Task Where_datetime_year_component(bool async)
        {
            await base.Where_datetime_year_component(async);

            AssertSql(
                @"SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM `Orders` AS `o`
WHERE EXTRACT(year FROM `o`.`OrderDate`) = 1998");
        }

        [ConditionalTheory]
        public override async Task Where_datetime_month_component(bool async)
        {
            await base.Where_datetime_month_component(async);

            AssertSql(
                @"SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM `Orders` AS `o`
WHERE EXTRACT(month FROM `o`.`OrderDate`) = 4");
        }

        [ConditionalTheory]
        public override async Task Where_datetime_dayOfYear_component(bool async)
        {
            await base.Where_datetime_dayOfYear_component(async);

            AssertSql(
                @"SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM `Orders` AS `o`
WHERE DAYOFYEAR(`o`.`OrderDate`) = 68");
        }

        [ConditionalTheory]
        public override async Task Where_datetime_day_component(bool async)
        {
            await base.Where_datetime_day_component(async);

            AssertSql(
                @"SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM `Orders` AS `o`
WHERE EXTRACT(day FROM `o`.`OrderDate`) = 4");
        }

        [ConditionalTheory]
        public override async Task Where_datetime_hour_component(bool async)
        {
            await base.Where_datetime_hour_component(async);

            AssertSql(
"""
SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM `Orders` AS `o`
WHERE EXTRACT(hour FROM `o`.`OrderDate`) = 0
""");
        }

        [ConditionalTheory]
        public override async Task Where_datetime_minute_component(bool async)
        {
            await base.Where_datetime_minute_component(async);

            AssertSql(
"""
SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM `Orders` AS `o`
WHERE EXTRACT(minute FROM `o`.`OrderDate`) = 0
""");
        }

        [ConditionalTheory]
        public override async Task Where_datetime_second_component(bool async)
        {
            await base.Where_datetime_second_component(async);

            AssertSql(
"""
SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM `Orders` AS `o`
WHERE EXTRACT(second FROM `o`.`OrderDate`) = 0
""");
        }

        [ConditionalTheory]
        public override async Task Where_datetime_millisecond_component(bool async)
        {
            await base.Where_datetime_millisecond_component(async);

            AssertSql(
"""
SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM `Orders` AS `o`
WHERE (EXTRACT(microsecond FROM `o`.`OrderDate`)) DIV (1000) = 0
""");
        }

        [ConditionalTheory]
        public override async Task Where_string_length(bool async)
        {
            await base.Where_string_length(async);
            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE CHAR_LENGTH(`c`.`City`) = 6");
        }

        [ConditionalTheory]
        public override async Task Where_string_indexof(bool async)
        {
            await base.Where_string_indexof(async);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE ((LOCATE('Sea', `c`.`City`) - 1) <> -1) OR `c`.`City` IS NULL");
        }

        [ConditionalTheory]
        public override async Task Where_string_replace(bool async)
        {
            await base.Where_string_replace(async);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE REPLACE(`c`.`City`, 'Sea', 'Rea') = 'Reattle'");
        }

        [ConditionalTheory]
        public override async Task Where_string_substring(bool async)
        {
            await base.Where_string_substring(async);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE SUBSTRING(`c`.`City`, 1 + 1, 2) = 'ea'");
        }

        [ConditionalTheory(Skip = "issue #573")]
        public override Task Where_as_queryable_expression(bool async)
        {
            return base.Where_as_queryable_expression(async);
        }

        [ConditionalTheory(Skip = "issue #552")]
        public override Task Where_multiple_contains_in_subquery_with_and(bool async)
        {
            return base.Where_multiple_contains_in_subquery_with_and(async);
        }

        [ConditionalTheory(Skip = "issue #552")]
        public override Task Where_multiple_contains_in_subquery_with_or(bool async)
        {
            return base.Where_multiple_contains_in_subquery_with_or(async);
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Where_string_remove(bool async)
        {
            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => c.City.Remove(3) == "Sea"));

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE SUBSTRING(`c`.`City`, 1, 3) = 'Sea'");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Where_string_remove_count(bool async)
        {
            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => c.City.Remove(3, 1) == "Seatle"));

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE CONCAT(SUBSTRING(`c`.`City`, 1, 3), SUBSTRING(`c`.`City`, (3 + 1) + 1, CHAR_LENGTH(`c`.`City`) - (3 + 1))) = 'Seatle'");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Where_guid(bool async)
        {
            var guidParameter = new Guid("4D68FE70-DDB0-47D7-B6DB-437684FA3E1F");

            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => guidParameter == Guid.NewGuid()),
                assertEmpty: true);

            AssertSql(
                @"@__guidParameter_0='4d68fe70-ddb0-47d7-b6db-437684fa3e1f'

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE @__guidParameter_0 = UUID()");
        }

        public override async Task Where_string_concat_method_comparison_2(bool async)
        {
            await base.Where_string_concat_method_comparison_2(async);

            AssertSql(
"""
@__i_0='A' (Size = 4000)
@__j_1='B' (Size = 4000)

SELECT `c`.`CustomerID`
FROM `Customers` AS `c`
WHERE CONCAT(@__i_0, @__j_1, `c`.`CustomerID`) = 'ABANATR'
""");
        }

        public override async Task Where_string_concat_method_comparison_3(bool async)
        {
            await base.Where_string_concat_method_comparison_3(async);

            AssertSql(
"""
@__i_0='A' (Size = 4000)
@__j_1='B' (Size = 4000)
@__k_2='C' (Size = 4000)

SELECT `c`.`CustomerID`
FROM `Customers` AS `c`
WHERE CONCAT(@__i_0, @__j_1, @__k_2, `c`.`CustomerID`) = 'ABCANTON'
""");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Where_string_concat_method_comparison_single_object(bool async)
        {
            object i = 1;

            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => string.Concat(i) == c.CompanyName).Select(c => c.CustomerID),
                assertEmpty: true);

            AssertSql(
                @"@__Concat_0='1' (Size = 40)

SELECT `c`.`CustomerID`
FROM `Customers` AS `c`
WHERE @__Concat_0 = `c`.`CompanyName`");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Where_string_concat_method_comparison_object(bool async)
        {
            object i = 1;

            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => string.Concat(i, c.CustomerID) == c.CompanyName).Select(c => c.CustomerID),
                assertEmpty: true);

            AssertSql(
                @"@__i_0='1' (Size = 4000)

SELECT `c`.`CustomerID`
FROM `Customers` AS `c`
WHERE CONCAT(@__i_0, `c`.`CustomerID`) = `c`.`CompanyName`");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Where_string_concat_method_comparison_object_2(bool async)
        {
            object i = 1;
            object j = 2;

            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => string.Concat(i, j, c.CustomerID) == c.CompanyName).Select(c => c.CustomerID),
                assertEmpty: true);

            AssertSql(
                @"@__i_0='1' (Size = 4000)
@__j_1='2' (Size = 4000)

SELECT `c`.`CustomerID`
FROM `Customers` AS `c`
WHERE CONCAT(@__i_0, @__j_1, `c`.`CustomerID`) = `c`.`CompanyName`");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Where_string_concat_method_comparison_object_3(bool async)
        {
            object i = 1;
            object j = 2;
            object k = 3;

            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => string.Concat(new[] { i, j, k, c.CustomerID }) == c.CompanyName).Select(c => c.CustomerID),
                assertEmpty: true);

        AssertSql(
"""
@__i_0='1' (Size = 4000)
@__j_1='2' (Size = 4000)
@__k_2='3' (Size = 4000)

SELECT `c`.`CustomerID`
FROM `Customers` AS `c`
WHERE CONCAT(@__i_0, @__j_1, @__k_2, `c`.`CustomerID`) = `c`.`CompanyName`
""");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Where_string_concat_method_comparison_params_string_array(bool async)
        {
            var i = "A";
            var j = "B";
            var k = "C";
            var m = "D";

            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => string.Concat(new[] { i, j, k, m, c.CustomerID }) == c.CompanyName).Select(c => c.CustomerID),
                assertEmpty: true);

        AssertSql(
"""
@__i_0='A' (Size = 4000)
@__j_1='B' (Size = 4000)
@__k_2='C' (Size = 4000)
@__m_3='D' (Size = 4000)

SELECT `c`.`CustomerID`
FROM `Customers` AS `c`
WHERE CONCAT(@__i_0, @__j_1, @__k_2, @__m_3, `c`.`CustomerID`) = `c`.`CompanyName`
""");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Where_string_concat_method_comparison_explicit_string_array(bool async)
        {
            var array = new[] {"A", "B", "C", "D"};

            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => string.Concat(array) == c.CompanyName).Select(c => c.CustomerID),
                assertEmpty: true);

            AssertSql(
                @"@__Concat_0='ABCD' (Size = 40)

SELECT `c`.`CustomerID`
FROM `Customers` AS `c`
WHERE @__Concat_0 = `c`.`CompanyName`");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Where_string_concat_method_comparison_explicit_string_array_single_element(bool async)
        {
            var array = new[] {"A"};

            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => string.Concat(array) == c.CompanyName).Select(c => c.CustomerID),
                assertEmpty: true);

            AssertSql(
                @"@__Concat_0='A' (Size = 40)

SELECT `c`.`CustomerID`
FROM `Customers` AS `c`
WHERE @__Concat_0 = `c`.`CompanyName`");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Where_string_concat_method_comparison_params_object_array(bool async)
        {
            object i = 1;
            object j = 2;
            object k = 3;
            object m = 4;

            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => string.Concat(new[] { i, j, k, m, c.CustomerID }) == c.CompanyName).Select(c => c.CustomerID),
                assertEmpty: true);

        AssertSql(
"""
@__i_0='1' (Size = 4000)
@__j_1='2' (Size = 4000)
@__k_2='3' (Size = 4000)
@__m_3='4' (Size = 4000)

SELECT `c`.`CustomerID`
FROM `Customers` AS `c`
WHERE CONCAT(@__i_0, @__j_1, @__k_2, @__m_3, `c`.`CustomerID`) = `c`.`CompanyName`
""");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Where_string_concat_method_comparison_explicit_object_array(bool async)
        {
            var array = new object[] {1, 2, 3, 4};

            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => string.Concat(array) == c.CompanyName).Select(c => c.CustomerID),
                assertEmpty: true);

            AssertSql(
                @"@__Concat_0='1234' (Size = 40)

SELECT `c`.`CustomerID`
FROM `Customers` AS `c`
WHERE @__Concat_0 = `c`.`CompanyName`");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Where_string_concat_method_comparison_explicit_object_array_single_element(bool async)
        {
            var array = new object[] {1};

            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => string.Concat(array) == c.CompanyName).Select(c => c.CustomerID),
                assertEmpty: true);

            AssertSql(
                @"@__Concat_0='1' (Size = 40)

SELECT `c`.`CustomerID`
FROM `Customers` AS `c`
WHERE @__Concat_0 = `c`.`CompanyName`");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Where_string_concat_method_comparison_string_enumerable(bool async)
        {
            IEnumerable<string> enumerable = new[] {"A", "B", "C", "D"};

            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => string.Concat(enumerable) == c.CompanyName).Select(c => c.CustomerID),
                assertEmpty: true);

            AssertSql(
                @"@__Concat_0='ABCD' (Size = 40)

SELECT `c`.`CustomerID`
FROM `Customers` AS `c`
WHERE @__Concat_0 = `c`.`CompanyName`");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Where_string_concat_method_comparison_string_enumerable_single_element(bool async)
        {
            IEnumerable<string> enumerable = new[] {"A"};

            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => string.Concat(enumerable) == c.CompanyName).Select(c => c.CustomerID),
                assertEmpty: true);

            AssertSql(
                @"@__Concat_0='A' (Size = 40)

SELECT `c`.`CustomerID`
FROM `Customers` AS `c`
WHERE @__Concat_0 = `c`.`CompanyName`");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Where_string_concat_method_comparison_generic_enumerable(bool async)
        {
            IEnumerable<int> enumerable = new[] {1, 2, 3, 4};

            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => string.Concat(enumerable) == c.CompanyName).Select(c => c.CustomerID),
                assertEmpty: true);

            AssertSql(
                @"@__Concat_0='1234' (Size = 40)

SELECT `c`.`CustomerID`
FROM `Customers` AS `c`
WHERE @__Concat_0 = `c`.`CompanyName`");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Where_string_concat_method_comparison_generic_enumerable_single_element(bool async)
        {
            IEnumerable<int> enumerable = new[] {1};

            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => string.Concat(enumerable) == c.CompanyName).Select(c => c.CustomerID),
                assertEmpty: true);

            AssertSql(
                @"@__Concat_0='1' (Size = 40)

SELECT `c`.`CustomerID`
FROM `Customers` AS `c`
WHERE @__Concat_0 = `c`.`CompanyName`");
        }

        public override async Task Where_bitwise_xor(bool async)
        {
            await base.Where_bitwise_xor(async);

            AssertSql(
"""
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE (`c`.`CustomerID` = 'ALFKI') ^ TRUE
""");
        }

        // TODO: 9.0
        [SupportedServerVersionBetweenCondition("11.4.2-mariadb", "11.5.0-mariadb", Invert = true, Skip =
"""
There is some strange collation behavior with MariaDB 11.4.x and this test (seems fixed in 11.5).
The default utf8mb4 collation was changed in 11.4.2 from utf8mb4_general_ci to utf8mb4_uca1400_ai_ci.
We changed MariaDbServerVersion.DefaultUtf8CiCollation and MariaDbServerVersion.DefaultUtf8CsCollation accordingly.
If we run the this test against a Ubuntu hosted MariaDB, the test always works.
If we run the this test against a Windows hosted MariaDB, the test fails only on the first execution (when the database does not preexist). But it works on all consecutive runs.
If we change MariaDbServerVersion.DefaultUtf8CiCollation and MariaDbServerVersion.DefaultUtf8CsCollation to use the new default collations only from 11.5.0, the Ubuntu/Windows behavior flips to Windows always working and Ubuntu not successfully executing the test on the first run.
The error is:
    MySqlConnector.MySqlException : Illegal mix of collations (utf8mb4_bin,NONE) and (utf8mb4_general_ci,IMPLICIT) for operation '='
""")]
        public override Task Where_concat_string_int_comparison1(bool async)
        {
            return base.Where_concat_string_int_comparison1(async);
        }

        // TODO: 9.0
        [SupportedServerVersionBetweenCondition("11.4.2-mariadb", "11.5.0-mariadb", Invert = true, Skip =
"""
There is some strange collation behavior with MariaDB 11.4.x and this test (seems fixed in 11.5).
The default utf8mb4 collation was changed in 11.4.2 from utf8mb4_general_ci to utf8mb4_uca1400_ai_ci.
We changed MariaDbServerVersion.DefaultUtf8CiCollation and MariaDbServerVersion.DefaultUtf8CsCollation accordingly.
If we run the this test against a Ubuntu hosted MariaDB, the test always works.
If we run the this test against a Windows hosted MariaDB, the test fails only on the first execution (when the database does not preexist). But it works on all consecutive runs.
If we change MariaDbServerVersion.DefaultUtf8CiCollation and MariaDbServerVersion.DefaultUtf8CsCollation to use the new default collations only from 11.5.0, the Ubuntu/Windows behavior flips to Windows always working and Ubuntu not successfully executing the test on the first run.
The error is:
    MySqlConnector.MySqlException : Illegal mix of collations (utf8mb4_bin,NONE) and (utf8mb4_general_ci,IMPLICIT) for operation '='
""")]
        public override Task Where_concat_string_int_comparison2(bool async)
        {
            return base.Where_concat_string_int_comparison2(async);
        }

        // TODO: 9.0
        [SupportedServerVersionBetweenCondition("11.4.2-mariadb", "11.5.0-mariadb", Invert = true, Skip =
"""
There is some strange collation behavior with MariaDB 11.4.x and this test (seems fixed in 11.5).
The default utf8mb4 collation was changed in 11.4.2 from utf8mb4_general_ci to utf8mb4_uca1400_ai_ci.
We changed MariaDbServerVersion.DefaultUtf8CiCollation and MariaDbServerVersion.DefaultUtf8CsCollation accordingly.
If we run the this test against a Ubuntu hosted MariaDB, the test always works.
If we run the this test against a Windows hosted MariaDB, the test fails only on the first execution (when the database does not preexist). But it works on all consecutive runs.
If we change MariaDbServerVersion.DefaultUtf8CiCollation and MariaDbServerVersion.DefaultUtf8CsCollation to use the new default collations only from 11.5.0, the Ubuntu/Windows behavior flips to Windows always working and Ubuntu not successfully executing the test on the first run.
The error is:
    MySqlConnector.MySqlException : Illegal mix of collations (utf8mb4_bin,NONE) and (utf8mb4_general_ci,IMPLICIT) for operation '='
""")]
        public override Task Where_concat_string_int_comparison3(bool async)
        {
            return base.Where_concat_string_int_comparison3(async);
        }

        // TODO: 9.0
        [SupportedServerVersionBetweenCondition("11.4.2-mariadb", "11.5.0-mariadb", Invert = true, Skip =
"""
There is some strange collation behavior with MariaDB 11.4.x and this test (seems fixed in 11.5).
The default utf8mb4 collation was changed in 11.4.2 from utf8mb4_general_ci to utf8mb4_uca1400_ai_ci.
We changed MariaDbServerVersion.DefaultUtf8CiCollation and MariaDbServerVersion.DefaultUtf8CsCollation accordingly.
If we run the this test against a Ubuntu hosted MariaDB, the test always works.
If we run the this test against a Windows hosted MariaDB, the test fails only on the first execution (when the database does not preexist). But it works on all consecutive runs.
If we change MariaDbServerVersion.DefaultUtf8CiCollation and MariaDbServerVersion.DefaultUtf8CsCollation to use the new default collations only from 11.5.0, the Ubuntu/Windows behavior flips to Windows always working and Ubuntu not successfully executing the test on the first run.
The error is:
    MySqlConnector.MySqlException : Illegal mix of collations (utf8mb4_bin,NONE) and (utf8mb4_general_ci,IMPLICIT) for operation '='
""")]
        public override Task Where_concat_string_int_comparison4(bool async)
        {
            return base.Where_concat_string_int_comparison4(async);
        }

        // TODO: 9.0
        [SupportedServerVersionBetweenCondition("11.4.2-mariadb", "11.5.0-mariadb", Invert = true, Skip =
"""
There is some strange collation behavior with MariaDB 11.4.x and this test (seems fixed in 11.5).
The default utf8mb4 collation was changed in 11.4.2 from utf8mb4_general_ci to utf8mb4_uca1400_ai_ci.
We changed MariaDbServerVersion.DefaultUtf8CiCollation and MariaDbServerVersion.DefaultUtf8CsCollation accordingly.
If we run the this test against a Ubuntu hosted MariaDB, the test always works.
If we run the this test against a Windows hosted MariaDB, the test fails only on the first execution (when the database does not preexist). But it works on all consecutive runs.
If we change MariaDbServerVersion.DefaultUtf8CiCollation and MariaDbServerVersion.DefaultUtf8CsCollation to use the new default collations only from 11.5.0, the Ubuntu/Windows behavior flips to Windows always working and Ubuntu not successfully executing the test on the first run.
The error is:
    MySqlConnector.MySqlException : Illegal mix of collations (utf8mb4_bin,NONE) and (utf8mb4_uca1400_ai_ci,COERCIBLE) for operation '='
""")]
        public override Task Using_same_parameter_twice_in_query_generates_one_sql_parameter(bool async)
        {
            return base.Using_same_parameter_twice_in_query_generates_one_sql_parameter(async);
        }

        public override async Task Where_compare_constructed_equal(bool async)
        {
            //  Anonymous type to constant comparison. Issue #14672.
            await AssertTranslationFailed(() => base.Where_compare_constructed_equal(async));

            AssertSql();
        }

        public override async Task Where_compare_constructed_multi_value_equal(bool async)
        {
            //  Anonymous type to constant comparison. Issue #14672.
            await AssertTranslationFailed(() => base.Where_compare_constructed_multi_value_equal(async));

            AssertSql();
        }

        public override async Task Where_compare_constructed_multi_value_not_equal(bool async)
        {
            //  Anonymous type to constant comparison. Issue #14672.
            await AssertTranslationFailed(() => base.Where_compare_constructed_multi_value_not_equal(async));

            AssertSql();
        }

        public override async Task Where_compare_tuple_create_constructed_equal(bool async)
        {
            //  Anonymous type to constant comparison. Issue #14672.
            await AssertTranslationFailed(() => base.Where_compare_tuple_create_constructed_equal(async));

            AssertSql();
        }

        public override async Task Where_compare_tuple_create_constructed_multi_value_equal(bool async)
        {
            //  Anonymous type to constant comparison. Issue #14672.
            await AssertTranslationFailed(() => base.Where_compare_tuple_create_constructed_multi_value_equal(async));

            AssertSql();
        }

        public override async Task Where_compare_tuple_create_constructed_multi_value_not_equal(bool async)
        {
            //  Anonymous type to constant comparison. Issue #14672.
            await AssertTranslationFailed(() => base.Where_compare_tuple_create_constructed_multi_value_not_equal(async));

            AssertSql();
        }

        public override async Task Where_compare_tuple_constructed_equal(bool async)
        {
            // Anonymous type to constant comparison. Issue #14672.
            await AssertTranslationFailed(() => base.Where_compare_tuple_constructed_equal(async));

            AssertSql();
        }

        public override async Task Where_compare_tuple_constructed_multi_value_equal(bool async)
        {
            // Anonymous type to constant comparison. Issue #14672.
            await AssertTranslationFailed(() => base.Where_compare_tuple_constructed_multi_value_equal(async));

            AssertSql();
        }

        public override async Task Where_compare_tuple_constructed_multi_value_not_equal(bool async)
        {
            // Anonymous type to constant comparison. Issue #14672.
            await AssertTranslationFailed(() => base.Where_compare_tuple_constructed_multi_value_not_equal(async));

            AssertSql();
        }

        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);

        protected override void ClearLog()
            => Fixture.TestSqlLoggerFactory.Clear();
    }
}
