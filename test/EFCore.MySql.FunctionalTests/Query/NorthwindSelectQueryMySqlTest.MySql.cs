using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.TestModels.Northwind;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Tests.TestUtilities.Attributes;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public partial class NorthwindSelectQueryMySqlTest
    {
        [SupportedServerVersionLessThanCondition(nameof(ServerVersionSupport.CrossApply))]
        [MemberData(nameof(IsAsyncData))]
        public virtual Task CrossApply_not_supported_throws(bool async)
        {
            return Assert.ThrowsAsync<InvalidOperationException>(() => base.SelectMany_correlated_with_outer_1(async));
        }

        [SupportedServerVersionLessThanCondition(nameof(ServerVersionSupport.OuterApply))]
        [MemberData(nameof(IsAsyncData))]
        public virtual Task OuterApply_not_supported_throws(bool async)
        {
            return Assert.ThrowsAsync<InvalidOperationException>(() => base.SelectMany_correlated_with_outer_3(async));
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Select_with_function_using_having_clause(bool async)
        {
            await AssertQuery(
                async,
                ss => ss.Set<Order>()
                    .Where(o => o.CustomerID == "ALFKI" &&
                                o.OrderDate != null)
                    .GroupBy(o => new {o.CustomerID, o.OrderDate.Value.Date})
                    .Select(g => new {g.Key.Date.Year, Count = g.Count()})
                    .Where(k => k.Year == 1995)
                    .OrderBy(k => k.Year),
                assertOrder: true,
                assertEmpty: true); // TODO: Use a linq query that does not return an empty result.

            AssertSql(
                @"SELECT `o1`.`Year`, `o1`.`Count`
FROM (
    SELECT EXTRACT(year FROM `o0`.`Date`) AS `Year`, COUNT(*) AS `Count`, (EXTRACT(year FROM `o0`.`Date`) = 1995) AND EXTRACT(year FROM `o0`.`Date`) IS NOT NULL AS `c`
    FROM (
        SELECT `o`.`CustomerID`, CONVERT(`o`.`OrderDate`, date) AS `Date`
        FROM `Orders` AS `o`
        WHERE (`o`.`CustomerID` = 'ALFKI') AND `o`.`OrderDate` IS NOT NULL
    ) AS `o0`
    GROUP BY `o0`.`CustomerID`, `o0`.`Date`, `c`
    HAVING `c`
) AS `o1`
ORDER BY `o1`.`Year`");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Select_with_function_using_having_clause_concatenated(bool async)
        {
            await AssertQueryScalar(
                async,
                ss => ss.Set<Order>()
                    .Where(o => o.CustomerID == "ALFKI" &&
                                o.OrderDate != null)
                    .GroupBy(o => new {o.CustomerID, o.OrderDate.Value.Year})
                    .Select(g => new {g.Key.Year, Count = g.Count()})
                    .Where(k => k.Year == 1995)
                    .Select(k => k.Year)
                    .Concat(ss.Set<Order>()
                        .Where(o => o.OrderDate != null)
                        .GroupBy(o => o.OrderDate.Value.Year)
                        .Select(g => new {Year = g.Key, Count = g.Count()})
                        .Where(k => k.Count > 0)
                        .Select(k => k.Year)),
                assertOrder: true);

        AssertSql(
"""
SELECT `o3`.`Year`
FROM (
    SELECT `o0`.`Year`, (`o0`.`Year` = 1995) AND `o0`.`Year` IS NOT NULL AS `c`
    FROM (
        SELECT `o`.`CustomerID`, EXTRACT(year FROM `o`.`OrderDate`) AS `Year`
        FROM `Orders` AS `o`
        WHERE (`o`.`CustomerID` = 'ALFKI') AND `o`.`OrderDate` IS NOT NULL
    ) AS `o0`
    GROUP BY `o0`.`CustomerID`, `o0`.`Year`, `c`
    HAVING `c`
) AS `o3`
UNION ALL
SELECT `o2`.`Key` AS `Year`
FROM (
    SELECT EXTRACT(year FROM `o1`.`OrderDate`) AS `Key`
    FROM `Orders` AS `o1`
    WHERE `o1`.`OrderDate` IS NOT NULL
) AS `o2`
GROUP BY `o2`.`Key`
HAVING COUNT(*) > 0
""");
        }
    }
}
