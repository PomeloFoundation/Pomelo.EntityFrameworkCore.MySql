using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestModels.Northwind;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities.Attributes;
using Pomelo.EntityFrameworkCore.MySql.Storage;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public partial class SimpleQueryMySqlTest : SimpleQueryTestBase<NorthwindQueryMySqlFixture<NoopModelCustomizer>>
    {
        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task PadLeft_without_second_arg(bool isAsync)
        {
            await AssertQuery(
                isAsync,
                ss => ss.Set<Customer>().Where(r => r.CustomerID.PadLeft(8) == "   ALFKI"),
                entryCount: 1);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE LPAD(`c`.`CustomerID`, 8, ' ') = '   ALFKI'");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task PadLeft_with_second_arg(bool isAsync)
        {
            await AssertQuery(
                isAsync,
                ss => ss.Set<Customer>().Where(r => r.CustomerID.PadLeft(8, 'x') == "xxxALFKI"),
                entryCount: 1);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE LPAD(`c`.`CustomerID`, 8, 'x') = 'xxxALFKI'");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task PadRight_without_second_arg(bool isAsync)
        {
            await AssertQuery(
                isAsync,
                ss => ss.Set<Customer>().Where(r => r.CustomerID.PadRight(8) == "ALFKI   "),
                entryCount: 1);

            AssertSql(@"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE RPAD(`c`.`CustomerID`, 8, ' ') = 'ALFKI   '");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task PadRight_with_second_arg(bool isAsync)
        {
            await AssertQuery(
                isAsync,
                ss => ss.Set<Customer>().Where(r => r.CustomerID.PadRight(8, 'c') == "ALFKIccc"),
                entryCount: 1);

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
        public async Task StringEquals_with_comparison_parameter(StringComparison comparison, int expected, bool isAsync)
        {
            await AssertQuery(
                isAsync,
                ss => ss.Set<Customer>().Where(c => c.CustomerID.Equals("anton", comparison)),
                entryCount: expected);

            // When the comparison parameter is not a constant, we have to use a case
            // statement
            AssertSql(
                $@"@__comparison_0='{comparison:D}'

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE CASE
    WHEN @__comparison_0 IN (4, 0, 2) THEN `c`.`CustomerID` = CONVERT('anton' USING utf8mb4) COLLATE utf8mb4_bin
    ELSE ((LCASE(`c`.`CustomerID`) = CONVERT(LCASE('anton') USING utf8mb4) COLLATE utf8mb4_bin) AND (LCASE(`c`.`CustomerID`) IS NOT NULL AND CONVERT(LCASE('anton') USING utf8mb4) COLLATE utf8mb4_bin IS NOT NULL)) OR (LCASE(`c`.`CustomerID`) IS NULL AND CONVERT(LCASE('anton') USING utf8mb4) COLLATE utf8mb4_bin IS NULL)
END");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task StringEquals_ordinal(bool isAsync)
        {
            await AssertQuery(
                isAsync,
                ss => ss.Set<Customer>().Where(c => c.CustomerID.Equals("anton", StringComparison.Ordinal)),
                entryCount: 0);

            AssertSql(@"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` = CONVERT('anton' USING utf8mb4) COLLATE utf8mb4_bin");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task StringEquals_invariant(bool isAsync)
        {
            await AssertQuery(
                isAsync,
                ss => ss.Set<Customer>().Where(c => c.CustomerID.Equals("anton", StringComparison.CurrentCulture)),
                entryCount: 0);

            AssertSql(@"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` = CONVERT('anton' USING utf8mb4) COLLATE utf8mb4_bin");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task StringEquals_current(bool isAsync)
        {
            await AssertQuery(
                isAsync,
                ss => ss.Set<Customer>().Where(c => c.CustomerID.Equals("anton", StringComparison.InvariantCulture)),
                entryCount: 0);

            AssertSql(@"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` = CONVERT('anton' USING utf8mb4) COLLATE utf8mb4_bin");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task StringEquals_ordinal_ignore_case(bool isAsync)
        {
            await AssertQuery(
                isAsync,
                ss => ss.Set<Customer>().Where(c => c.CustomerID.Equals("anton", StringComparison.OrdinalIgnoreCase)),
                entryCount: 1);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE (LCASE(`c`.`CustomerID`) = CONVERT(LCASE('anton') USING utf8mb4) COLLATE utf8mb4_bin) OR (LCASE(`c`.`CustomerID`) IS NULL AND CONVERT(LCASE('anton') USING utf8mb4) COLLATE utf8mb4_bin IS NULL)");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task StringEquals_current_ignore_case(bool isAsync)
        {
            await AssertQuery(
                isAsync,
                ss => ss.Set<Customer>().Where(c => c.CustomerID.Equals("anton", StringComparison.CurrentCultureIgnoreCase)),
                entryCount: 1);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE (LCASE(`c`.`CustomerID`) = CONVERT(LCASE('anton') USING utf8mb4) COLLATE utf8mb4_bin) OR (LCASE(`c`.`CustomerID`) IS NULL AND CONVERT(LCASE('anton') USING utf8mb4) COLLATE utf8mb4_bin IS NULL)");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task StringEquals_invariant_ignore_case(bool isAsync)
        {
            await AssertQuery(
                isAsync,
                ss => ss.Set<Customer>().Where(c => c.CustomerID.Equals("anton", StringComparison.InvariantCultureIgnoreCase)),
                entryCount: 1);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE (LCASE(`c`.`CustomerID`) = CONVERT(LCASE('anton') USING utf8mb4) COLLATE utf8mb4_bin) OR (LCASE(`c`.`CustomerID`) IS NULL AND CONVERT(LCASE('anton') USING utf8mb4) COLLATE utf8mb4_bin IS NULL)");
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
            await AssertQuery(
                isAsync,
                ss => ss.Set<Customer>().Where(c => string.Equals(c.CustomerID, "anton", comparison)),
                entryCount: expected);

            // When the comparison parameter is not a constant, we have to use a case
            // statement
            AssertSql(
                $@"@__comparison_0='{comparison:D}'

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE CASE
    WHEN @__comparison_0 IN (4, 0, 2) THEN `c`.`CustomerID` = CONVERT('anton' USING utf8mb4) COLLATE utf8mb4_bin
    ELSE ((LCASE(`c`.`CustomerID`) = CONVERT(LCASE('anton') USING utf8mb4) COLLATE utf8mb4_bin) AND (LCASE(`c`.`CustomerID`) IS NOT NULL AND CONVERT(LCASE('anton') USING utf8mb4) COLLATE utf8mb4_bin IS NOT NULL)) OR (LCASE(`c`.`CustomerID`) IS NULL AND CONVERT(LCASE('anton') USING utf8mb4) COLLATE utf8mb4_bin IS NULL)
END");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task StaticStringEquals_ordinal(bool isAsync)
        {
            await AssertQuery(
                isAsync,
                ss => ss.Set<Customer>().Where(c => string.Equals(c.CustomerID, "anton", StringComparison.Ordinal)),
                entryCount: 0);

            AssertSql(@"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` = CONVERT('anton' USING utf8mb4) COLLATE utf8mb4_bin");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task StaticStringEquals_invariant(bool isAsync)
        {
            await AssertQuery(
                isAsync,
                ss => ss.Set<Customer>().Where(c => string.Equals(c.CustomerID, "anton", StringComparison.CurrentCulture)),
                entryCount: 0);

            AssertSql(@"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` = CONVERT('anton' USING utf8mb4) COLLATE utf8mb4_bin");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task StaticStringEquals_current(bool isAsync)
        {
            await AssertQuery(
                isAsync,
                ss => ss.Set<Customer>().Where(c => string.Equals(c.CustomerID, "anton", StringComparison.InvariantCulture)),
                entryCount: 0);

            AssertSql(@"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` = CONVERT('anton' USING utf8mb4) COLLATE utf8mb4_bin");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task StaticStringEquals_ordinal_ignore_case(bool isAsync)
        {
            await AssertQuery(
                isAsync,
                ss => ss.Set<Customer>().Where(c => string.Equals(c.CustomerID, "anton", StringComparison.OrdinalIgnoreCase)),
                entryCount: 1);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE (LCASE(`c`.`CustomerID`) = CONVERT(LCASE('anton') USING utf8mb4) COLLATE utf8mb4_bin) OR (LCASE(`c`.`CustomerID`) IS NULL AND CONVERT(LCASE('anton') USING utf8mb4) COLLATE utf8mb4_bin IS NULL)");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task StaticStringEquals_current_ignore_case(bool isAsync)
        {
            await AssertQuery(
                isAsync,
                ss => ss.Set<Customer>().Where(c => string.Equals(c.CustomerID, "anton", StringComparison.CurrentCultureIgnoreCase)),
                entryCount: 1);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE (LCASE(`c`.`CustomerID`) = CONVERT(LCASE('anton') USING utf8mb4) COLLATE utf8mb4_bin) OR (LCASE(`c`.`CustomerID`) IS NULL AND CONVERT(LCASE('anton') USING utf8mb4) COLLATE utf8mb4_bin IS NULL)");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task StaticStringEquals_invariant_ignore_case(bool isAsync)
        {
            await AssertQuery(
                isAsync,
                ss => ss.Set<Customer>().Where(c => string.Equals(c.CustomerID, "anton", StringComparison.InvariantCultureIgnoreCase)),
                entryCount: 1);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE (LCASE(`c`.`CustomerID`) = CONVERT(LCASE('anton') USING utf8mb4) COLLATE utf8mb4_bin) OR (LCASE(`c`.`CustomerID`) IS NULL AND CONVERT(LCASE('anton') USING utf8mb4) COLLATE utf8mb4_bin IS NULL)");
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
        public async Task StringContains_with_comparison(StringComparison comparison, int expected, bool isAsync)
        {
            await AssertQuery(
                isAsync,
                ss => ss.Set<Customer>().Where(c => c.CustomerID.Contains("nto", comparison)),
                entryCount: expected);

            // When the comparison parameter is not a constant, we have to use a case
            // statement
            AssertSql(
                $@"@__comparison_0='{comparison:D}'

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE CASE
    WHEN @__comparison_0 IN (4, 0, 2) THEN LOCATE(CONVERT('nto' USING utf8mb4) COLLATE utf8mb4_bin, `c`.`CustomerID`) > 0
    ELSE LOCATE(CONVERT(LCASE('nto') USING utf8mb4) COLLATE utf8mb4_bin, LCASE(`c`.`CustomerID`)) > 0
END");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task StringContains_ordinal(bool isAsync)
        {
            await AssertQuery(
                isAsync,
                ss => ss.Set<Customer>().Where(c => c.CustomerID.Contains("nto", StringComparison.Ordinal)),
                entryCount: 0);

            AssertSql(@"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE LOCATE(CONVERT('nto' USING utf8mb4) COLLATE utf8mb4_bin, `c`.`CustomerID`) > 0");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task StringContains_invariant(bool isAsync)
        {
            await AssertQuery(
                isAsync,
                ss => ss.Set<Customer>().Where(c => c.CustomerID.Contains("nto", StringComparison.CurrentCulture)),
                entryCount: 0);

            AssertSql(@"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE LOCATE(CONVERT('nto' USING utf8mb4) COLLATE utf8mb4_bin, `c`.`CustomerID`) > 0");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task StringContains_current(bool isAsync)
        {
            await AssertQuery(
                isAsync,
                ss => ss.Set<Customer>().Where(c => c.CustomerID.Contains("nto", StringComparison.InvariantCulture)),
                entryCount: 0);

            AssertSql(@"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE LOCATE(CONVERT('nto' USING utf8mb4) COLLATE utf8mb4_bin, `c`.`CustomerID`) > 0");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task StringContains_ordinal_ignore_case(bool isAsync)
        {
            await AssertQuery(
                isAsync,
                ss => ss.Set<Customer>().Where(c => c.CustomerID.Contains("nto", StringComparison.OrdinalIgnoreCase)),
                entryCount: 1);

            AssertSql(@"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE LOCATE(CONVERT(LCASE('nto') USING utf8mb4) COLLATE utf8mb4_bin, LCASE(`c`.`CustomerID`)) > 0");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task StringContains_current_ignore_case(bool isAsync)
        {
            await AssertQuery(
                isAsync,
                ss => ss.Set<Customer>().Where(c => c.CustomerID.Contains("nto", StringComparison.CurrentCultureIgnoreCase)),
                entryCount: 1);

            AssertSql(@"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE LOCATE(CONVERT(LCASE('nto') USING utf8mb4) COLLATE utf8mb4_bin, LCASE(`c`.`CustomerID`)) > 0");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task StringContains_invariant_ignore_case(bool isAsync)
        {
            await AssertQuery(
                isAsync,
                ss => ss.Set<Customer>().Where(c => c.CustomerID.Contains("nto", StringComparison.InvariantCultureIgnoreCase)),
                entryCount: 1);

            AssertSql(@"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE LOCATE(CONVERT(LCASE('nto') USING utf8mb4) COLLATE utf8mb4_bin, LCASE(`c`.`CustomerID`)) > 0");
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
        public async Task StringStartsWith_with_comparison(StringComparison comparison, int expected, bool isAsync)
        {
            await AssertQuery(
                isAsync,
                ss => ss.Set<Customer>().Where(c => c.CustomerID.StartsWith("anto", comparison)),
                entryCount: expected);

            // When the comparison parameter is not a constant, we have to use a case
            // statement
            AssertSql(
                $@"@__comparison_0='{comparison:D}'

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE CASE
    WHEN @__comparison_0 IN (4, 0, 2) THEN (`c`.`CustomerID` LIKE CONCAT('anto', '%')) AND ((LEFT(`c`.`CustomerID`, CHAR_LENGTH(CONVERT('anto' USING utf8mb4) COLLATE utf8mb4_bin)) = CONVERT('anto' USING utf8mb4) COLLATE utf8mb4_bin) AND LEFT(`c`.`CustomerID`, CHAR_LENGTH(CONVERT('anto' USING utf8mb4) COLLATE utf8mb4_bin)) IS NOT NULL)
    ELSE (LCASE(`c`.`CustomerID`) LIKE CONCAT(CONVERT(LCASE('anto') USING utf8mb4) COLLATE utf8mb4_bin, '%')) AND (((LEFT(LCASE(`c`.`CustomerID`), CHAR_LENGTH(CONVERT(LCASE('anto') USING utf8mb4) COLLATE utf8mb4_bin)) = CONVERT(LCASE('anto') USING utf8mb4) COLLATE utf8mb4_bin) AND (LEFT(LCASE(`c`.`CustomerID`), CHAR_LENGTH(CONVERT(LCASE('anto') USING utf8mb4) COLLATE utf8mb4_bin)) IS NOT NULL AND CONVERT(LCASE('anto') USING utf8mb4) COLLATE utf8mb4_bin IS NOT NULL)) OR (LEFT(LCASE(`c`.`CustomerID`), CHAR_LENGTH(CONVERT(LCASE('anto') USING utf8mb4) COLLATE utf8mb4_bin)) IS NULL AND CONVERT(LCASE('anto') USING utf8mb4) COLLATE utf8mb4_bin IS NULL))
END");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task StringStartsWith_ordinal(bool isAsync)
        {
            await AssertQuery(
                isAsync,
                ss => ss.Set<Customer>().Where(c => c.CustomerID.StartsWith("anto", StringComparison.Ordinal)),
                entryCount: 0);

            AssertSql(@"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE (`c`.`CustomerID` LIKE CONCAT('anto', '%')) AND " +
                      "(LEFT(`c`.`CustomerID`, CHAR_LENGTH(CONVERT('anto' USING utf8mb4) COLLATE utf8mb4_bin)) = CONVERT('anto' USING utf8mb4) COLLATE utf8mb4_bin)");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task StringStartsWith_invariant(bool isAsync)
        {
            await AssertQuery(
                isAsync,
                ss => ss.Set<Customer>().Where(c => c.CustomerID.StartsWith("anto", StringComparison.CurrentCulture)),
                entryCount: 0);

            AssertSql(@"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE (`c`.`CustomerID` LIKE CONCAT('anto', '%')) AND " +
                      "(LEFT(`c`.`CustomerID`, CHAR_LENGTH(CONVERT('anto' USING utf8mb4) COLLATE utf8mb4_bin)) = CONVERT('anto' USING utf8mb4) COLLATE utf8mb4_bin)");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task StringStartsWith_current(bool isAsync)
        {
            await AssertQuery(
                isAsync,
                ss => ss.Set<Customer>().Where(c => c.CustomerID.StartsWith("anto", StringComparison.InvariantCulture)),
                entryCount: 0);

            AssertSql(@"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE (`c`.`CustomerID` LIKE CONCAT('anto', '%')) AND " +
                      "(LEFT(`c`.`CustomerID`, CHAR_LENGTH(CONVERT('anto' USING utf8mb4) COLLATE utf8mb4_bin)) = CONVERT('anto' USING utf8mb4) COLLATE utf8mb4_bin)");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task StringStartsWith_ordinal_ignore_case(bool isAsync)
        {
            await AssertQuery(
                isAsync,
                ss => ss.Set<Customer>().Where(c => c.CustomerID.StartsWith("anto", StringComparison.OrdinalIgnoreCase)),
                entryCount: 1);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE (LCASE(`c`.`CustomerID`) LIKE CONCAT(CONVERT(LCASE('anto') USING utf8mb4) COLLATE utf8mb4_bin, '%')) AND ((LEFT(LCASE(`c`.`CustomerID`), CHAR_LENGTH(CONVERT(LCASE('anto') USING utf8mb4) COLLATE utf8mb4_bin)) = CONVERT(LCASE('anto') USING utf8mb4) COLLATE utf8mb4_bin) OR (LEFT(LCASE(`c`.`CustomerID`), CHAR_LENGTH(CONVERT(LCASE('anto') USING utf8mb4) COLLATE utf8mb4_bin)) IS NULL AND CONVERT(LCASE('anto') USING utf8mb4) COLLATE utf8mb4_bin IS NULL))");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task StringStartsWith_current_ignore_case(bool isAsync)
        {
            await AssertQuery(
                isAsync,
                ss => ss.Set<Customer>().Where(c => c.CustomerID.StartsWith("anto", StringComparison.CurrentCultureIgnoreCase)),
                entryCount: 1);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE (LCASE(`c`.`CustomerID`) LIKE CONCAT(CONVERT(LCASE('anto') USING utf8mb4) COLLATE utf8mb4_bin, '%')) AND ((LEFT(LCASE(`c`.`CustomerID`), CHAR_LENGTH(CONVERT(LCASE('anto') USING utf8mb4) COLLATE utf8mb4_bin)) = CONVERT(LCASE('anto') USING utf8mb4) COLLATE utf8mb4_bin) OR (LEFT(LCASE(`c`.`CustomerID`), CHAR_LENGTH(CONVERT(LCASE('anto') USING utf8mb4) COLLATE utf8mb4_bin)) IS NULL AND CONVERT(LCASE('anto') USING utf8mb4) COLLATE utf8mb4_bin IS NULL))");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task StringStartsWith_invariant_ignore_case(bool isAsync)
        {
            await AssertQuery(
                isAsync,
                ss => ss.Set<Customer>().Where(c => c.CustomerID.StartsWith("anto", StringComparison.InvariantCultureIgnoreCase)),
                entryCount: 1);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE (LCASE(`c`.`CustomerID`) LIKE CONCAT(CONVERT(LCASE('anto') USING utf8mb4) COLLATE utf8mb4_bin, '%')) AND ((LEFT(LCASE(`c`.`CustomerID`), CHAR_LENGTH(CONVERT(LCASE('anto') USING utf8mb4) COLLATE utf8mb4_bin)) = CONVERT(LCASE('anto') USING utf8mb4) COLLATE utf8mb4_bin) OR (LEFT(LCASE(`c`.`CustomerID`), CHAR_LENGTH(CONVERT(LCASE('anto') USING utf8mb4) COLLATE utf8mb4_bin)) IS NULL AND CONVERT(LCASE('anto') USING utf8mb4) COLLATE utf8mb4_bin IS NULL))");
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
            await AssertQuery(
                isAsync,
                ss => ss.Set<Customer>().Where(c => c.CustomerID.EndsWith("nton", comparison)),
                entryCount: expected);

            AssertSql(
                $@"@__comparison_0='{comparison:D}'

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE CASE
    WHEN @__comparison_0 IN (4, 0, 2) THEN (RIGHT(`c`.`CustomerID`, CHAR_LENGTH(CONVERT('nton' USING utf8mb4) COLLATE utf8mb4_bin)) = CONVERT('nton' USING utf8mb4) COLLATE utf8mb4_bin) AND RIGHT(`c`.`CustomerID`, CHAR_LENGTH(CONVERT('nton' USING utf8mb4) COLLATE utf8mb4_bin)) IS NOT NULL
    ELSE ((RIGHT(LCASE(`c`.`CustomerID`), CHAR_LENGTH(CONVERT(LCASE('nton') USING utf8mb4) COLLATE utf8mb4_bin)) = CONVERT(LCASE('nton') USING utf8mb4) COLLATE utf8mb4_bin) AND (RIGHT(LCASE(`c`.`CustomerID`), CHAR_LENGTH(CONVERT(LCASE('nton') USING utf8mb4) COLLATE utf8mb4_bin)) IS NOT NULL AND CONVERT(LCASE('nton') USING utf8mb4) COLLATE utf8mb4_bin IS NOT NULL)) OR (RIGHT(LCASE(`c`.`CustomerID`), CHAR_LENGTH(CONVERT(LCASE('nton') USING utf8mb4) COLLATE utf8mb4_bin)) IS NULL AND CONVERT(LCASE('nton') USING utf8mb4) COLLATE utf8mb4_bin IS NULL)
END");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task StringEndsWith_ordinal(bool isAsync)
        {
            await AssertQuery(
                isAsync,
                ss => ss.Set<Customer>().Where(c => c.CustomerID.EndsWith("nton", StringComparison.Ordinal)),
                entryCount: 0);

            AssertSql(@"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE RIGHT(`c`.`CustomerID`, CHAR_LENGTH(CONVERT('nton' USING utf8mb4) COLLATE utf8mb4_bin)) = CONVERT('nton' USING utf8mb4) COLLATE utf8mb4_bin");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task StringEndsWith_invariant(bool isAsync)
        {
            await AssertQuery(
                isAsync,
                ss => ss.Set<Customer>().Where(c => c.CustomerID.EndsWith("nton", StringComparison.CurrentCulture)),
                entryCount: 0);

            AssertSql(@"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE RIGHT(`c`.`CustomerID`, CHAR_LENGTH(CONVERT('nton' USING utf8mb4) COLLATE utf8mb4_bin)) = CONVERT('nton' USING utf8mb4) COLLATE utf8mb4_bin");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task StringEndsWith_current(bool isAsync)
        {
            await AssertQuery(
                isAsync,
                ss => ss.Set<Customer>().Where(c => c.CustomerID.EndsWith("nton", StringComparison.InvariantCulture)),
                entryCount: 0);

            AssertSql(@"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE RIGHT(`c`.`CustomerID`, CHAR_LENGTH(CONVERT('nton' USING utf8mb4) COLLATE utf8mb4_bin)) = CONVERT('nton' USING utf8mb4) COLLATE utf8mb4_bin");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task StringEndsWith_ordinal_ignore_case(bool isAsync)
        {
            await AssertQuery(
                isAsync,
                ss => ss.Set<Customer>().Where(c => c.CustomerID.EndsWith("nton", StringComparison.OrdinalIgnoreCase)),
                entryCount: 1);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE (RIGHT(LCASE(`c`.`CustomerID`), CHAR_LENGTH(CONVERT(LCASE('nton') USING utf8mb4) COLLATE utf8mb4_bin)) = CONVERT(LCASE('nton') USING utf8mb4) COLLATE utf8mb4_bin) OR (RIGHT(LCASE(`c`.`CustomerID`), CHAR_LENGTH(CONVERT(LCASE('nton') USING utf8mb4) COLLATE utf8mb4_bin)) IS NULL AND CONVERT(LCASE('nton') USING utf8mb4) COLLATE utf8mb4_bin IS NULL)");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task StringEndsWith_current_ignore_case(bool isAsync)
        {
            await AssertQuery(
                isAsync,
                ss => ss.Set<Customer>().Where(c => c.CustomerID.EndsWith("nton", StringComparison.CurrentCultureIgnoreCase)),
                entryCount: 1);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE (RIGHT(LCASE(`c`.`CustomerID`), CHAR_LENGTH(CONVERT(LCASE('nton') USING utf8mb4) COLLATE utf8mb4_bin)) = CONVERT(LCASE('nton') USING utf8mb4) COLLATE utf8mb4_bin) OR (RIGHT(LCASE(`c`.`CustomerID`), CHAR_LENGTH(CONVERT(LCASE('nton') USING utf8mb4) COLLATE utf8mb4_bin)) IS NULL AND CONVERT(LCASE('nton') USING utf8mb4) COLLATE utf8mb4_bin IS NULL)");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task StringEndsWith_invariant_ignore_case(bool isAsync)
        {
            await AssertQuery(
                isAsync,
                ss => ss.Set<Customer>().Where(c => c.CustomerID.EndsWith("nton", StringComparison.InvariantCultureIgnoreCase)),
                entryCount: 1);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE (RIGHT(LCASE(`c`.`CustomerID`), CHAR_LENGTH(CONVERT(LCASE('nton') USING utf8mb4) COLLATE utf8mb4_bin)) = CONVERT(LCASE('nton') USING utf8mb4) COLLATE utf8mb4_bin) OR (RIGHT(LCASE(`c`.`CustomerID`), CHAR_LENGTH(CONVERT(LCASE('nton') USING utf8mb4) COLLATE utf8mb4_bin)) IS NULL AND CONVERT(LCASE('nton') USING utf8mb4) COLLATE utf8mb4_bin IS NULL)");
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
            await AssertQuery(
                isAsync,
                ss => ss.Set<Customer>().Where(c => c.CustomerID.IndexOf("nt", comparison) == 1),
                entryCount: expected);

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
        public async Task StringIndexOf_ordinal(bool isAsync)
        {
            await AssertQuery(
                isAsync,
                ss => ss.Set<Customer>().Where(c => c.CustomerID.IndexOf("nt", StringComparison.Ordinal) == 1),
                entryCount: 0);

            AssertSql(@"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE (LOCATE(CONVERT('nt' USING utf8mb4) COLLATE utf8mb4_bin, `c`.`CustomerID`) - 1) = 1");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task StringIndexOf_invariant(bool isAsync)
        {
            await AssertQuery(
                isAsync,
                ss => ss.Set<Customer>().Where(c => c.CustomerID.IndexOf("nt", StringComparison.CurrentCulture) == 1),
                entryCount: 0);

            AssertSql(@"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE (LOCATE(CONVERT('nt' USING utf8mb4) COLLATE utf8mb4_bin, `c`.`CustomerID`) - 1) = 1");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task StringIndexOf_current(bool isAsync)
        {
            await AssertQuery(
                isAsync,
                ss => ss.Set<Customer>().Where(c => c.CustomerID.IndexOf("nt", StringComparison.InvariantCulture) == 1),
                entryCount: 0);

            AssertSql(@"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE (LOCATE(CONVERT('nt' USING utf8mb4) COLLATE utf8mb4_bin, `c`.`CustomerID`) - 1) = 1");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task StringIndexOf_ordinal_ignore_case(bool isAsync)
        {
            await AssertQuery(
                isAsync,
                ss => ss.Set<Customer>().Where(c => c.CustomerID.IndexOf("nt", StringComparison.OrdinalIgnoreCase) == 1),
                entryCount: 1);

            AssertSql(@"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE (LOCATE(CONVERT(LCASE('nt') USING utf8mb4) COLLATE utf8mb4_bin, LCASE(`c`.`CustomerID`)) - 1) = 1");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task StringIndexOf_current_ignore_case(bool isAsync)
        {
            await AssertQuery(
                isAsync,
                ss => ss.Set<Customer>().Where(c => c.CustomerID.IndexOf("nt", StringComparison.CurrentCultureIgnoreCase) == 1),
                entryCount: 1);

            AssertSql(@"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE (LOCATE(CONVERT(LCASE('nt') USING utf8mb4) COLLATE utf8mb4_bin, LCASE(`c`.`CustomerID`)) - 1) = 1");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task StringIndexOf_invariant_ignore_case(bool isAsync)
        {
            await AssertQuery(
                isAsync,
                ss => ss.Set<Customer>().Where(c => c.CustomerID.IndexOf("nt", StringComparison.InvariantCultureIgnoreCase) == 1),
                entryCount: 1);

            AssertSql(@"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE (LOCATE(CONVERT(LCASE('nt') USING utf8mb4) COLLATE utf8mb4_bin, LCASE(`c`.`CustomerID`)) - 1) = 1");
        }

        [SupportedServerVersionLessThanTheory(ServerVersion.CrossApplySupportKey)]
        [MemberData(nameof(IsAsyncData))]
        public virtual Task CrossApply_not_supported_throws(bool isAsync)
        {
            return Assert.ThrowsAsync<InvalidOperationException>(() => base.SelectMany_correlated_with_outer_1(isAsync));
        }

        [SupportedServerVersionLessThanTheory(ServerVersion.OuterApplySupportKey)]
        [MemberData(nameof(IsAsyncData))]
        public virtual Task OuterApply_not_supported_throws(bool isAsync)
        {
            return Assert.ThrowsAsync<InvalidOperationException>(() => base.SelectMany_correlated_with_outer_3(isAsync));
        }

        [SupportedServerVersionLessThanTheory(ServerVersion.WindowFunctionsSupportKey)]
        [MemberData(nameof(IsAsyncData))]
        public Task RowNumberOverPartitionBy_not_supported_throws(bool isAsync)
        {
            return Assert.ThrowsAsync<InvalidOperationException>(() => base.SelectMany_Joined_Take(isAsync));
        }
    }
}
