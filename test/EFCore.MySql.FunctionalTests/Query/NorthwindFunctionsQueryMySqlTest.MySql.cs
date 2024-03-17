using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.TestModels.Northwind;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public partial class NorthwindFunctionsQueryMySqlTest
    {
        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task PadLeft_without_second_arg(bool async)
        {
            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(r => r.CustomerID.PadLeft(8) == "   ALFKI"));

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE LPAD(`c`.`CustomerID`, 8, ' ') = '   ALFKI'");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task PadLeft_with_second_arg(bool async)
        {
            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(r => r.CustomerID.PadLeft(8, 'x') == "xxxALFKI"));

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE LPAD(`c`.`CustomerID`, 8, 'x') = 'xxxALFKI'");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task PadRight_without_second_arg(bool async)
        {
            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(r => r.CustomerID.PadRight(8) == "ALFKI   "));

            AssertSql(@"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE RPAD(`c`.`CustomerID`, 8, ' ') = 'ALFKI   '");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task PadRight_with_second_arg(bool async)
        {
            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(r => r.CustomerID.PadRight(8, 'c') == "ALFKIccc"));

            AssertSql(@"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE RPAD(`c`.`CustomerID`, 8, 'c') = 'ALFKIccc'");
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
        public async Task StringEquals_with_comparison_parameter(StringComparison comparison, int expected, bool async)
        {
            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => c.CustomerID.Equals("anton", comparison)),
                assertEmpty: expected == 0);

            // When the comparison parameter is not a constant, we have to use a case
            // statement
            AssertSql(
                $@"@__comparison_0='{comparison:D}'

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE CASE
    WHEN @__comparison_0 IN (4, 0, 2) THEN `c`.`CustomerID` = CONVERT('anton' USING utf8mb4) COLLATE utf8mb4_bin
    ELSE (LCASE(`c`.`CustomerID`) = CONVERT(LCASE('anton') USING utf8mb4) COLLATE utf8mb4_bin) AND CONVERT(LCASE('anton') USING utf8mb4) COLLATE utf8mb4_bin IS NOT NULL
END");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task StringEquals_ordinal(bool async)
        {
            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => c.CustomerID.Equals("anton", StringComparison.Ordinal)),
                assertEmpty: true);

            AssertSql(@"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` = CONVERT('anton' USING utf8mb4) COLLATE utf8mb4_bin");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task StringEquals_invariant(bool async)
        {
            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => c.CustomerID.Equals("anton", StringComparison.CurrentCulture)),
                assertEmpty: true);

            AssertSql(@"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` = CONVERT('anton' USING utf8mb4) COLLATE utf8mb4_bin");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task StringEquals_current(bool async)
        {
            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => c.CustomerID.Equals("anton", StringComparison.InvariantCulture)),
                assertEmpty: true);

            AssertSql(@"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` = CONVERT('anton' USING utf8mb4) COLLATE utf8mb4_bin");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task StringEquals_ordinal_ignore_case(bool async)
        {
            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => c.CustomerID.Equals("anton", StringComparison.OrdinalIgnoreCase)));

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE LCASE(`c`.`CustomerID`) = CONVERT(LCASE('anton') USING utf8mb4) COLLATE utf8mb4_bin");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task StringEquals_current_ignore_case(bool async)
        {
            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => c.CustomerID.Equals("anton", StringComparison.CurrentCultureIgnoreCase)));

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE LCASE(`c`.`CustomerID`) = CONVERT(LCASE('anton') USING utf8mb4) COLLATE utf8mb4_bin");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task StringEquals_invariant_ignore_case(bool async)
        {
            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => c.CustomerID.Equals("anton", StringComparison.InvariantCultureIgnoreCase)));

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE LCASE(`c`.`CustomerID`) = CONVERT(LCASE('anton') USING utf8mb4) COLLATE utf8mb4_bin");
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
        public async Task StaticStringEquals_with_comparison(StringComparison comparison, int expected, bool async)
        {
            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => string.Equals(c.CustomerID, "anton", comparison)),
                assertEmpty: expected == 0);

            // When the comparison parameter is not a constant, we have to use a case
            // statement
            AssertSql(
                $@"@__comparison_0='{comparison:D}'

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE CASE
    WHEN @__comparison_0 IN (4, 0, 2) THEN `c`.`CustomerID` = CONVERT('anton' USING utf8mb4) COLLATE utf8mb4_bin
    ELSE (LCASE(`c`.`CustomerID`) = CONVERT(LCASE('anton') USING utf8mb4) COLLATE utf8mb4_bin) AND CONVERT(LCASE('anton') USING utf8mb4) COLLATE utf8mb4_bin IS NOT NULL
END");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task StaticStringEquals_ordinal(bool async)
        {
            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => string.Equals(c.CustomerID, "anton", StringComparison.Ordinal)),
                assertEmpty: true);

            AssertSql(@"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` = CONVERT('anton' USING utf8mb4) COLLATE utf8mb4_bin");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task StaticStringEquals_invariant(bool async)
        {
            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => string.Equals(c.CustomerID, "anton", StringComparison.CurrentCulture)),
                assertEmpty: true);

            AssertSql(@"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` = CONVERT('anton' USING utf8mb4) COLLATE utf8mb4_bin");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task StaticStringEquals_current(bool async)
        {
            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => string.Equals(c.CustomerID, "anton", StringComparison.InvariantCulture)),
                assertEmpty: true);

            AssertSql(@"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` = CONVERT('anton' USING utf8mb4) COLLATE utf8mb4_bin");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task StaticStringEquals_ordinal_ignore_case(bool async)
        {
            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => string.Equals(c.CustomerID, "anton", StringComparison.OrdinalIgnoreCase)));

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE LCASE(`c`.`CustomerID`) = CONVERT(LCASE('anton') USING utf8mb4) COLLATE utf8mb4_bin");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task StaticStringEquals_current_ignore_case(bool async)
        {
            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => string.Equals(c.CustomerID, "anton", StringComparison.CurrentCultureIgnoreCase)));

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE LCASE(`c`.`CustomerID`) = CONVERT(LCASE('anton') USING utf8mb4) COLLATE utf8mb4_bin");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task StaticStringEquals_invariant_ignore_case(bool async)
        {
            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => string.Equals(c.CustomerID, "anton", StringComparison.InvariantCultureIgnoreCase)));

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE LCASE(`c`.`CustomerID`) = CONVERT(LCASE('anton') USING utf8mb4) COLLATE utf8mb4_bin");
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
        public async Task StringContains_with_comparison(StringComparison comparison, int expected, bool async)
        {
            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => c.CustomerID.Contains("nto", comparison)),
                assertEmpty: expected == 0);

            // When the comparison parameter is not a constant, we have to use a case
            // statement
            AssertSql(
                $@"@__comparison_0='{comparison:D}'

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE CASE
    WHEN @__comparison_0 IN (4, 0, 2) THEN `c`.`CustomerID` LIKE CONVERT('%nto%' USING utf8mb4) COLLATE utf8mb4_bin
    ELSE ((LCASE(`c`.`CustomerID`) LIKE CONVERT(LCASE('%nto%') USING utf8mb4) COLLATE utf8mb4_bin) AND TRUE) AND CONVERT(LCASE('%nto%') USING utf8mb4) COLLATE utf8mb4_bin IS NOT NULL
END");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task StringContains_ordinal(bool async)
        {
            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => c.CustomerID.Contains("nto", StringComparison.Ordinal)),
                assertEmpty: true);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` LIKE CONVERT('%nto%' USING utf8mb4) COLLATE utf8mb4_bin");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task StringContains_invariant(bool async)
        {
            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => c.CustomerID.Contains("nto", StringComparison.CurrentCulture)),
                assertEmpty: true);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` LIKE CONVERT('%nto%' USING utf8mb4) COLLATE utf8mb4_bin");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task StringContains_current(bool async)
        {
            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => c.CustomerID.Contains("nto", StringComparison.InvariantCulture)),
                assertEmpty: true);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` LIKE CONVERT('%nto%' USING utf8mb4) COLLATE utf8mb4_bin");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task StringContains_ordinal_ignore_case(bool async)
        {
            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => c.CustomerID.Contains("nto", StringComparison.OrdinalIgnoreCase)));

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE LCASE(`c`.`CustomerID`) LIKE CONVERT(LCASE('%nto%') USING utf8mb4) COLLATE utf8mb4_bin");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task StringContains_current_ignore_case(bool async)
        {
            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => c.CustomerID.Contains("nto", StringComparison.CurrentCultureIgnoreCase)));

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE LCASE(`c`.`CustomerID`) LIKE CONVERT(LCASE('%nto%') USING utf8mb4) COLLATE utf8mb4_bin");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task StringContains_invariant_ignore_case(bool async)
        {
            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => c.CustomerID.Contains("nto", StringComparison.InvariantCultureIgnoreCase)));

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE LCASE(`c`.`CustomerID`) LIKE CONVERT(LCASE('%nto%') USING utf8mb4) COLLATE utf8mb4_bin");
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
        public async Task StringStartsWith_with_comparison(StringComparison comparison, int expected, bool async)
        {
            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => c.CustomerID.StartsWith("anto", comparison)),
                assertEmpty: expected == 0);

            // When the comparison parameter is not a constant, we have to use a case
            // statement
            AssertSql(
                $@"@__comparison_0='{comparison:D}'

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE CASE
    WHEN @__comparison_0 IN (4, 0, 2) THEN `c`.`CustomerID` LIKE CONVERT('anto' USING utf8mb4) COLLATE utf8mb4_bin
    ELSE ((LCASE(`c`.`CustomerID`) LIKE CONVERT(LCASE('anto%') USING utf8mb4) COLLATE utf8mb4_bin) AND TRUE) AND CONVERT(LCASE('anto%') USING utf8mb4) COLLATE utf8mb4_bin IS NOT NULL
END");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task StringStartsWith_ordinal(bool async)
        {
            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => c.CustomerID.StartsWith("anto", StringComparison.Ordinal)),
                assertEmpty: true);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` LIKE CONVERT('anto%' USING utf8mb4) COLLATE utf8mb4_bin");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task StringStartsWith_invariant(bool async)
        {
            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => c.CustomerID.StartsWith("anto", StringComparison.CurrentCulture)),
                assertEmpty: true);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` LIKE CONVERT('anto%' USING utf8mb4) COLLATE utf8mb4_bin");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task StringStartsWith_current(bool async)
        {
            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => c.CustomerID.StartsWith("anto", StringComparison.InvariantCulture)),
                assertEmpty: true);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` LIKE CONVERT('anto%' USING utf8mb4) COLLATE utf8mb4_bin");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task StringStartsWith_ordinal_ignore_case(bool async)
        {
            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => c.CustomerID.StartsWith("anto", StringComparison.OrdinalIgnoreCase)));

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE LCASE(LCASE(`c`.`CustomerID`)) LIKE CONVERT(LCASE('anto%') USING utf8mb4) COLLATE utf8mb4_bin");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task StringStartsWith_current_ignore_case(bool async)
        {
            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => c.CustomerID.StartsWith("anto", StringComparison.CurrentCultureIgnoreCase)));

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE LCASE(LCASE(`c`.`CustomerID`)) LIKE CONVERT(LCASE('anto%') USING utf8mb4) COLLATE utf8mb4_bin");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task StringStartsWith_invariant_ignore_case(bool async)
        {
            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => c.CustomerID.StartsWith("anto", StringComparison.InvariantCultureIgnoreCase)));

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE LCASE(LCASE(`c`.`CustomerID`)) LIKE CONVERT(LCASE('anto%') USING utf8mb4) COLLATE utf8mb4_bin");
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
        public async Task StringEndsWith_with_comparison(StringComparison comparison, int expected, bool async)
        {
            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => c.CustomerID.EndsWith("nton", comparison)),
                assertEmpty: expected == 0);

            AssertSql(
                $@"@__comparison_0='{comparison:D}'

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE CASE
    WHEN @__comparison_0 IN (4, 0, 2) THEN `c`.`CustomerID` LIKE CONVERT('%nton' USING utf8mb4) COLLATE utf8mb4_bin
    ELSE ((LCASE(`c`.`CustomerID`) LIKE CONVERT(LCASE('%nton') USING utf8mb4) COLLATE utf8mb4_bin) AND TRUE) AND CONVERT(LCASE('%nton') USING utf8mb4) COLLATE utf8mb4_bin IS NOT NULL
END");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task StringEndsWith_ordinal(bool async)
        {
            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => c.CustomerID.EndsWith("nton", StringComparison.Ordinal)),
                assertEmpty: true);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` LIKE CONVERT('%nton' USING utf8mb4) COLLATE utf8mb4_bin");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task StringEndsWith_invariant(bool async)
        {
            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => c.CustomerID.EndsWith("nton", StringComparison.CurrentCulture)),
                assertEmpty: true);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` LIKE CONVERT('%nton' USING utf8mb4) COLLATE utf8mb4_bin");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task StringEndsWith_current(bool async)
        {
            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => c.CustomerID.EndsWith("nton", StringComparison.InvariantCulture)),
                assertEmpty: true);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` LIKE CONVERT('%nton' USING utf8mb4) COLLATE utf8mb4_bin");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task StringEndsWith_ordinal_ignore_case(bool async)
        {
            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => c.CustomerID.EndsWith("nton", StringComparison.OrdinalIgnoreCase)));

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE LCASE(`c`.`CustomerID`) LIKE CONVERT(LCASE('%nton') USING utf8mb4) COLLATE utf8mb4_bin");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task StringEndsWith_current_ignore_case(bool async)
        {
            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => c.CustomerID.EndsWith("nton", StringComparison.CurrentCultureIgnoreCase)));

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE LCASE(`c`.`CustomerID`) LIKE CONVERT(LCASE('%nton') USING utf8mb4) COLLATE utf8mb4_bin");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task StringEndsWith_invariant_ignore_case(bool async)
        {
            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => c.CustomerID.EndsWith("nton", StringComparison.InvariantCultureIgnoreCase)));

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE LCASE(`c`.`CustomerID`) LIKE CONVERT(LCASE('%nton') USING utf8mb4) COLLATE utf8mb4_bin");
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
        public async Task StringIndexOf_with_comparison(StringComparison comparison, int expected, bool async)
        {
            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => c.CustomerID.IndexOf("nt", comparison) == 1),
                assertEmpty: expected == 0);

            // When the comparison parameter is not a constant, we have to use a case
            // statement
            AssertSql($"@__comparison_0='{comparison:D}'" + @"

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE CASE
    WHEN @__comparison_0 IN (4, 0, 2) THEN LOCATE(CONVERT('nt' USING utf8mb4) COLLATE utf8mb4_bin, `c`.`CustomerID`) - 1
    ELSE LOCATE(CONVERT(LCASE('nt') USING utf8mb4) COLLATE utf8mb4_bin, LCASE(`c`.`CustomerID`)) - 1
END = 1");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task StringIndexOf_with_constant_start_index(bool async)
        {
            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => c.CustomerID.IndexOf("nt", 0, StringComparison.OrdinalIgnoreCase) == 1));

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE (LOCATE(CONVERT(LCASE('nt') USING utf8mb4) COLLATE utf8mb4_bin, LCASE(`c`.`CustomerID`), 1) - 1) = 1");
        }

        [ConditionalTheory]
        [InlineData(0, 1, false)]
        [InlineData(2, 0, false)]
        [InlineData(0, 1, true)]
        [InlineData(2, 0, true)]
        public async Task StringIndexOf_with_parameter_start_index(int startIndex, int expected, bool async)
        {
            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => c.CustomerID.IndexOf("nt", startIndex, StringComparison.OrdinalIgnoreCase) == 1),
                assertEmpty: expected == 0);

            AssertSql(
     @$"@__startIndex_0='{startIndex}'

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE (LOCATE(CONVERT(LCASE('nt') USING utf8mb4) COLLATE utf8mb4_bin, LCASE(`c`.`CustomerID`), @__startIndex_0 + 1) - 1) = 1");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task StringIndexOf_ordinal(bool async)
        {
            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => c.CustomerID.IndexOf("nt", StringComparison.Ordinal) == 1),
                assertEmpty: true);

            AssertSql(@"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE (LOCATE(CONVERT('nt' USING utf8mb4) COLLATE utf8mb4_bin, `c`.`CustomerID`) - 1) = 1");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task StringIndexOf_invariant(bool async)
        {
            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => c.CustomerID.IndexOf("nt", StringComparison.InvariantCulture) == 1),
                assertEmpty: true);

            AssertSql(@"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE (LOCATE(CONVERT('nt' USING utf8mb4) COLLATE utf8mb4_bin, `c`.`CustomerID`) - 1) = 1");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task StringIndexOf_current(bool async)
        {
            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => c.CustomerID.IndexOf("nt", StringComparison.CurrentCulture) == 1),
                assertEmpty: true);

            AssertSql(@"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE (LOCATE(CONVERT('nt' USING utf8mb4) COLLATE utf8mb4_bin, `c`.`CustomerID`) - 1) = 1");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task StringIndexOf_ordinal_ignore_case(bool async)
        {
            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => c.CustomerID.IndexOf("nt", StringComparison.OrdinalIgnoreCase) == 1));

            AssertSql(@"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE (LOCATE(CONVERT(LCASE('nt') USING utf8mb4) COLLATE utf8mb4_bin, LCASE(`c`.`CustomerID`)) - 1) = 1");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task StringIndexOf_current_ignore_case(bool async)
        {
            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => c.CustomerID.IndexOf("nt", StringComparison.CurrentCultureIgnoreCase) == 1));

            AssertSql(@"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE (LOCATE(CONVERT(LCASE('nt') USING utf8mb4) COLLATE utf8mb4_bin, LCASE(`c`.`CustomerID`)) - 1) = 1");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task StringIndexOf_invariant_ignore_case(bool async)
        {
            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => c.CustomerID.IndexOf("nt", StringComparison.InvariantCultureIgnoreCase) == 1));

            AssertSql(@"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE (LOCATE(CONVERT(LCASE('nt') USING utf8mb4) COLLATE utf8mb4_bin, LCASE(`c`.`CustomerID`)) - 1) = 1");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Where_math_log_new_base2(bool async)
        {
            // The original `Where_math_log_new_base` test will succeed even if the number and base are swapped by accident.
            await AssertQueryScalar(
                async,
                ss => ss.Set<OrderDetail>().Where(od => od.OrderID == 11077 && od.Discount > 0).Where(od => Math.Log(od.Discount, 7) < -1).Select(od => Math.Log(od.Discount, 7)));

            AssertSql(
$"""
SELECT LOG(7.0, CAST(`o`.`Discount` AS double))
FROM `Order Details` AS `o`
WHERE ((`o`.`OrderID` = 11077) AND (`o`.`Discount` > 0)) AND (LOG(7.0, {MySqlTestHelpers.CastAsDouble("`o`.`Discount`")}) < -1.0)
""");
        }
    }
}
