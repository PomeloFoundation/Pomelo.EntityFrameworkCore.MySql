using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public partial class NorthwindFunctionsQueryMySqlTest : NorthwindFunctionsQueryRelationalTestBase<
        NorthwindQueryMySqlFixture<NoopModelCustomizer>>
    {
        public NorthwindFunctionsQueryMySqlTest(
            NorthwindQueryMySqlFixture<NoopModelCustomizer> fixture,
            ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            ClearLog();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        [ConditionalTheory]
        public override async Task String_StartsWith_Literal(bool async)
        {
            await base.String_StartsWith_Literal(async);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`ContactName` IS NOT NULL AND (`c`.`ContactName` LIKE 'M%')");
        }

        [ConditionalTheory]
        public override async Task String_StartsWith_Identity(bool async)
        {
            await base.String_StartsWith_Identity(async);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE (`c`.`ContactName` = '') OR (`c`.`ContactName` IS NOT NULL AND (LEFT(`c`.`ContactName`, CHAR_LENGTH(`c`.`ContactName`)) = `c`.`ContactName`))");
        }

        [ConditionalTheory]
        public override async Task String_StartsWith_Column(bool async)
        {
            await base.String_StartsWith_Column(async);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE (`c`.`ContactName` = '') OR (`c`.`ContactName` IS NOT NULL AND (LEFT(`c`.`ContactName`, CHAR_LENGTH(`c`.`ContactName`)) = `c`.`ContactName`))");
        }

        [ConditionalTheory]
        public override async Task String_StartsWith_MethodCall(bool async)
        {
            await base.String_StartsWith_MethodCall(async);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`ContactName` IS NOT NULL AND (`c`.`ContactName` LIKE 'M%')");
        }

        [ConditionalTheory]
        public override async Task String_EndsWith_Literal(bool async)
        {
            await base.String_EndsWith_Literal(async);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`ContactName` IS NOT NULL AND (`c`.`ContactName` LIKE '%b')");
        }

        [ConditionalTheory]
        public override async Task String_EndsWith_Identity(bool async)
        {
            await base.String_EndsWith_Identity(async);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE (`c`.`ContactName` = '') OR (`c`.`ContactName` IS NOT NULL AND (RIGHT(`c`.`ContactName`, CHAR_LENGTH(`c`.`ContactName`)) = `c`.`ContactName`))");
        }

        [ConditionalTheory]
        public override async Task String_EndsWith_Column(bool async)
        {
            await base.String_EndsWith_Column(async);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE (`c`.`ContactName` = '') OR (`c`.`ContactName` IS NOT NULL AND (RIGHT(`c`.`ContactName`, CHAR_LENGTH(`c`.`ContactName`)) = `c`.`ContactName`))");
        }

        [ConditionalTheory]
        public override async Task String_EndsWith_MethodCall(bool async)
        {
            await base.String_EndsWith_MethodCall(async);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`ContactName` IS NOT NULL AND (`c`.`ContactName` LIKE '%m')");
        }

        [ConditionalTheory]
        public override async Task String_Contains_Literal(bool async)
        {
            await base.String_Contains_Literal(async);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`ContactName` LIKE '%M%'");
        }

        [ConditionalTheory]
        public override async Task String_Contains_Identity(bool async)
        {
            await base.String_Contains_Identity(async);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE LOCATE(`c`.`ContactName`, `c`.`ContactName`) > 0");
        }

        [ConditionalTheory]
        public override async Task String_Contains_Column(bool async)
        {
            await base.String_Contains_Column(async);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE LOCATE(`c`.`ContactName`, `c`.`ContactName`) > 0");
        }

        [ConditionalTheory]
        public override async Task String_Contains_MethodCall(bool async)
        {
            await base.String_Contains_MethodCall(async);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`ContactName` LIKE '%M%'");
        }

        [ConditionalTheory]
        public override async Task IsNullOrWhiteSpace_in_predicate(bool async)
        {
            await base.IsNullOrWhiteSpace_in_predicate(async);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`Region` IS NULL OR (TRIM(`c`.`Region`) = '')");
        }

        [ConditionalTheory]
        public override async Task Indexof_with_emptystring(bool async)
        {
            await base.Indexof_with_emptystring(async);

            AssertSql(
                @"SELECT LOCATE('', `c`.`ContactName`) - 1
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` = 'ALFKI'");
        }

        [ConditionalTheory]
        public override async Task Replace_with_emptystring(bool async)
        {
            await base.Replace_with_emptystring(async);

            AssertSql(
                @"SELECT REPLACE(`c`.`ContactName`, 'ari', '')
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` = 'ALFKI'");
        }

        [ConditionalTheory]
        public override async Task Substring_with_zero_startindex(bool async)
        {
            await base.Substring_with_zero_startindex(async);

            AssertSql(
                @"SELECT SUBSTRING(`c`.`ContactName`, 0 + 1, 3)
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` = 'ALFKI'");
        }

        [ConditionalTheory]
        public override async Task Substring_with_constant(bool async)
        {
            await base.Substring_with_constant(async);

            AssertSql(
                @"SELECT SUBSTRING(`c`.`ContactName`, 1 + 1, 3)
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` = 'ALFKI'");
        }

        [ConditionalTheory]
        public override async Task Substring_with_closure(bool async)
        {
            await base.Substring_with_closure(async);

            AssertSql(
                @"@__start_0='2'

SELECT SUBSTRING(`c`.`ContactName`, @__start_0 + 1, 3)
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` = 'ALFKI'");
        }

        [ConditionalTheory]
        public override async Task Substring_with_zero_length(bool async)
        {
            await base.Substring_with_zero_length(async);

            AssertSql(
                @"SELECT SUBSTRING(`c`.`ContactName`, 2 + 1, 0)
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` = 'ALFKI'");
        }

        [ConditionalTheory]
        public override async Task Where_math_abs1(bool async)
        {
            await base.Where_math_abs1(async);

            AssertSql(
                @"SELECT `o`.`OrderID`, `o`.`ProductID`, `o`.`Discount`, `o`.`Quantity`, `o`.`UnitPrice`
FROM `Order Details` AS `o`
WHERE ABS(`o`.`ProductID`) > 10");
        }

        [ConditionalTheory]
        public override async Task Where_math_abs2(bool async)
        {
            await base.Where_math_abs2(async);

            AssertSql(
                @"SELECT `o`.`OrderID`, `o`.`ProductID`, `o`.`Discount`, `o`.`Quantity`, `o`.`UnitPrice`
FROM `Order Details` AS `o`
WHERE ABS(`o`.`Quantity`) > 10");
        }

        [ConditionalTheory]
        public override async Task Where_math_abs_uncorrelated(bool async)
        {
            await base.Where_math_abs_uncorrelated(async);

            AssertSql(
                @"SELECT `o`.`OrderID`, `o`.`ProductID`, `o`.`Discount`, `o`.`Quantity`, `o`.`UnitPrice`
FROM `Order Details` AS `o`
WHERE 10 < `o`.`ProductID`");
        }

        [ConditionalTheory]
        public override async Task Select_math_round_int(bool async)
        {
            await base.Select_math_round_int(async);

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
        public override async Task Where_math_min(bool async)
        {
            await base.Where_math_min(async);

            AssertSql(
                @"SELECT `o`.`OrderID`, `o`.`ProductID`, `o`.`Discount`, `o`.`Quantity`, `o`.`UnitPrice`
FROM `Order Details` AS `o`
WHERE (`o`.`OrderID` = 11077) AND (LEAST(`o`.`OrderID`, `o`.`ProductID`) = `o`.`ProductID`)");
        }

        [ConditionalTheory]
        public override async Task Where_math_max(bool async)
        {
            await base.Where_math_max(async);

            AssertSql(
                @"SELECT `o`.`OrderID`, `o`.`ProductID`, `o`.`Discount`, `o`.`Quantity`, `o`.`UnitPrice`
FROM `Order Details` AS `o`
WHERE (`o`.`OrderID` = 11077) AND (GREATEST(`o`.`OrderID`, `o`.`ProductID`) = `o`.`OrderID`)");
        }

        [ConditionalTheory]
        public override async Task Where_string_to_lower(bool async)
        {
            await base.Where_string_to_lower(async);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE LOWER(`c`.`CustomerID`) = 'alfki'");
        }

        [ConditionalTheory]
        public override async Task Where_string_to_upper(bool async)
        {
            await base.Where_string_to_upper(async);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE UPPER(`c`.`CustomerID`) = 'ALFKI'");
        }

        [ConditionalTheory]
        public override async Task TrimStart_without_arguments_in_predicate(bool async)
        {
            await base.TrimStart_without_arguments_in_predicate(async);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE TRIM(LEADING FROM `c`.`ContactTitle`) = 'Owner'");
        }

        [ConditionalTheory]
        public override async Task TrimStart_with_char_argument_in_predicate(bool async)
        {
            await base.TrimStart_with_char_argument_in_predicate(async);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE TRIM(LEADING 'O' FROM `c`.`ContactTitle`) = 'wner'");
        }

        [ConditionalTheory]
        public override Task TrimStart_with_char_array_argument_in_predicate(bool async)
        {
            // MySQL only supports a string (characters in fixed order) as the parameter specifying what should be trimmed.
            // String.TrimStart has a different behavior, where any single character in any order will be trimmed.
            // Therefore, calling String.TrimStart with more than one char to trim, triggers client eval.
            return Assert.ThrowsAsync<InvalidOperationException>(() => base.TrimStart_with_char_array_argument_in_predicate(async));
        }

        [ConditionalTheory]
        public override async Task TrimEnd_without_arguments_in_predicate(bool async)
        {
            await base.TrimEnd_without_arguments_in_predicate(async);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE TRIM(TRAILING FROM `c`.`ContactTitle`) = 'Owner'");
        }

        [ConditionalTheory]
        public override async Task TrimEnd_with_char_argument_in_predicate(bool async)
        {
            await base.TrimEnd_with_char_argument_in_predicate(async);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE TRIM(TRAILING 'r' FROM `c`.`ContactTitle`) = 'Owne'");
        }

        [ConditionalTheory]
        public override Task TrimEnd_with_char_array_argument_in_predicate(bool async)
        {
            // MySQL only supports a string (characters in fixed order) as the parameter specifying what should be trimmed.
            // String.TrimEnd has a different behavior, where any single character in any order will be trimmed.
            // Therefore, calling String.TrimEnd with more than one char to trim, triggers client eval.
            return Assert.ThrowsAsync<InvalidOperationException>(() => base.TrimEnd_with_char_array_argument_in_predicate(async));
        }

        [ConditionalTheory]
        public override async Task Trim_without_argument_in_predicate(bool async)
        {
            await base.Trim_without_argument_in_predicate(async);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE TRIM(`c`.`ContactTitle`) = 'Owner'");
        }

        [ConditionalTheory]
        public override async Task Trim_with_char_argument_in_predicate(bool async)
        {
            await base.Trim_with_char_argument_in_predicate(async);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE TRIM('O' FROM `c`.`ContactTitle`) = 'wner'");
        }

        [ConditionalTheory]
        public override Task Trim_with_char_array_argument_in_predicate(bool async)
        {
            // MySQL only supports a string (characters in fixed order) as the parameter specifying what should be trimmed.
            // String.Trim has a different behavior, where any single character in any order will be trimmed.
            // Therefore, calling String.Trim with more than one char to trim, triggers client eval.
            return Assert.ThrowsAsync<InvalidOperationException>(() => base.Trim_with_char_array_argument_in_predicate(async));
        }

        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);

        protected override void ClearLog()
            => Fixture.TestSqlLoggerFactory.Clear();
    }
}
