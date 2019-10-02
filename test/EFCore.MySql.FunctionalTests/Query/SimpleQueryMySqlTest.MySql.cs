using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestModels.Northwind;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities.Attributes;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public partial class SimpleQueryMySqlTest : SimpleQueryTestBase<NorthwindQueryMySqlFixture<NoopModelCustomizer>>
    {
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
FROM `Customers` AS `c`
WHERE LPAD(`c`.`CustomerID`, 2, ' ') = 'AL'");
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
FROM `Customers` AS `c`
WHERE LPAD(`c`.`CustomerID`, 3, 'x') = 'AL'");
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
FROM `Customers` AS `c`
WHERE RPAD(`c`.`CustomerID`, 3, ' ') = 'AL'");
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
FROM `Customers` AS `c`
WHERE RPAD(`c`.`CustomerID`, 4, 'c') = 'AL'");
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
        public async Task StringEquals_with_comparison_parameter(StringComparison comparison, int expected, bool isAsync)
        {
            await AssertSingleResult<Customer>(isAsync,
                customer => customer.Where(c => c.CustomerID.Equals("anton", comparison)).Count(),
                customer => customer.Where(c => c.CustomerID.Equals("anton", comparison)).CountAsync(),
                asserter: (_, a) =>
                {
                    Assert.Equal(expected, (int)a);
                    // When the comparison parameter is not a constant, we have to use a case
                    // statement
                    AssertSql(
                        $@"@__comparison_0='{comparison:D}'

SELECT COUNT(*)
FROM `Customers` AS `c`
WHERE CASE
    WHEN @__comparison_0 IN (4, 0, 2) THEN `c`.`CustomerID` = CONVERT('anton' USING utf8mb4) COLLATE utf8mb4_bin
    ELSE LCASE(`c`.`CustomerID`) = CONVERT(LCASE('anton') USING utf8mb4) COLLATE utf8mb4_bin
END");
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
                    AssertSql(
                        $@"@__comparison_0='{comparison:D}'

SELECT COUNT(*)
FROM `Customers` AS `c`
WHERE CASE
    WHEN @__comparison_0 IN (4, 0, 2) THEN `c`.`CustomerID` = CONVERT('anton' USING utf8mb4) COLLATE utf8mb4_bin
    ELSE LCASE(`c`.`CustomerID`) = CONVERT(LCASE('anton') USING utf8mb4) COLLATE utf8mb4_bin
END");
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
                    AssertSql(
                        $@"@__comparison_0='{comparison:D}'

SELECT COUNT(*)
FROM `Customers` AS `c`
WHERE CASE
    WHEN @__comparison_0 IN (4, 0, 2) THEN LOCATE(CONVERT('nto' USING utf8mb4) COLLATE utf8mb4_bin, `c`.`CustomerID`) > 0
    ELSE LOCATE(CONVERT(LCASE('nto') USING utf8mb4) COLLATE utf8mb4_bin, LCASE(`c`.`CustomerID`)) > 0
END");
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
                    AssertSql(
                        $@"@__comparison_0='{comparison:D}'

SELECT COUNT(*)
FROM `Customers` AS `c`
WHERE CASE
    WHEN @__comparison_0 IN (4, 0, 2) THEN (`c`.`CustomerID` LIKE CONCAT('anto', '%')) AND (LEFT(`c`.`CustomerID`, CHAR_LENGTH(CONVERT('anto' USING utf8mb4) COLLATE utf8mb4_bin)) = CONVERT('anto' USING utf8mb4) COLLATE utf8mb4_bin)
    ELSE (LCASE(`c`.`CustomerID`) LIKE CONCAT(CONVERT(LCASE('anto') USING utf8mb4) COLLATE utf8mb4_bin, '%')) AND (LEFT(LCASE(`c`.`CustomerID`), CHAR_LENGTH(CONVERT(LCASE('anto') USING utf8mb4) COLLATE utf8mb4_bin)) = CONVERT(LCASE('anto') USING utf8mb4) COLLATE utf8mb4_bin)
END");
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
WHERE (`c`.`CustomerID` LIKE CONCAT('anto', '%')) AND " +
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
WHERE (`c`.`CustomerID` LIKE CONCAT('anto', '%')) AND " +
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
WHERE (`c`.`CustomerID` LIKE CONCAT('anto', '%')) AND " +
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
WHERE (LCASE(`c`.`CustomerID`) LIKE CONCAT(CONVERT(LCASE('anto') USING utf8mb4) COLLATE utf8mb4_bin, '%')) AND " +
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
WHERE (LCASE(`c`.`CustomerID`) LIKE CONCAT(CONVERT(LCASE('anto') USING utf8mb4) COLLATE utf8mb4_bin, '%')) AND " +
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
WHERE (LCASE(`c`.`CustomerID`) LIKE CONCAT(CONVERT(LCASE('anto') USING utf8mb4) COLLATE utf8mb4_bin, '%')) AND " +
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
 AssertSql(
                $@"@__comparison_0='{comparison:D}'

SELECT COUNT(*)
FROM `Customers` AS `c`
WHERE CASE
    WHEN @__comparison_0 IN (4, 0, 2) THEN RIGHT(`c`.`CustomerID`, CHAR_LENGTH(CONVERT('nton' USING utf8mb4) COLLATE utf8mb4_bin)) = CONVERT('nton' USING utf8mb4) COLLATE utf8mb4_bin
    ELSE RIGHT(LCASE(`c`.`CustomerID`), CHAR_LENGTH(CONVERT(LCASE('nton') USING utf8mb4) COLLATE utf8mb4_bin)) = CONVERT(LCASE('nton') USING utf8mb4) COLLATE utf8mb4_bin
END");
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

        [SupportedServerVersionLessThanTheory(ServerVersion.CrossApplyMySqlSupportVersionString)]
        [MemberData("IsAsyncData")]
        public virtual Task CrossApply_not_supported_throws(bool isAsync)
        {
            return Assert.ThrowsAsync<InvalidOperationException>(() => base.SelectMany_correlated_with_outer_1(isAsync));
        }

        [SupportedServerVersionLessThanTheory(ServerVersion.OuterApplyMySqlSupportVersionString)]
        [MemberData("IsAsyncData")]
        public virtual Task OuterApply_not_supported_throws(bool isAsync)
        {
            return Assert.ThrowsAsync<InvalidOperationException>(() => base.SelectMany_correlated_with_outer_3(isAsync));
        }
    }
}
