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
                    .GroupBy(o => new {o.CustomerID, o.OrderDate.Value.Year})
                    .Select(g => new {g.Key.Year, Count = g.Count()})
                    .Where(k => k.Year == 1995)
                    .OrderBy(k => k.Year),
                assertOrder: true,
                assertEmpty: true); // TODO: Use a linq query that does not return an empty result.

            AssertSql(
                @"SELECT `t`.`Year`, `t`.`Count`
FROM (
    SELECT `t`.`Year`, COUNT(*) AS `Count`, `t`.`CustomerID`, `t`.`Year` = 1995 AS `c`
    FROM (
        SELECT `o`.`CustomerID`, EXTRACT(year FROM `o`.`OrderDate`) AS `Year`
        FROM `Orders` AS `o`
        WHERE (`o`.`CustomerID` = 'ALFKI') AND `o`.`OrderDate` IS NOT NULL
    ) AS `t`
    GROUP BY `t`.`CustomerID`, `t`.`Year`, `c`
    HAVING `c`
) AS `t`
ORDER BY `t`.`Year`");
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
                @"SELECT `t`.`Year`
FROM (
    SELECT `t`.`Year`, `t`.`CustomerID`, `t`.`Year` = 1995 AS `c`
    FROM (
        SELECT `o`.`CustomerID`, EXTRACT(year FROM `o`.`OrderDate`) AS `Year`
        FROM `Orders` AS `o`
        WHERE (`o`.`CustomerID` = 'ALFKI') AND `o`.`OrderDate` IS NOT NULL
    ) AS `t`
    GROUP BY `t`.`CustomerID`, `t`.`Year`, `c`
    HAVING `c`
) AS `t`
UNION ALL
SELECT `t1`.`Key` AS `Year`
FROM (
    SELECT EXTRACT(year FROM `o0`.`OrderDate`) AS `Key`
    FROM `Orders` AS `o0`
    WHERE `o0`.`OrderDate` IS NOT NULL
) AS `t1`
GROUP BY `t1`.`Key`
HAVING COUNT(*) > 0");
        }
    }
}
