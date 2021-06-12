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
        public virtual Task Select_with_function_using_having_clause(bool async)
        {
            return AssertQuery(
                async,
                ss => ss.Set<Order>()
                    .Where(o => o.CustomerID == "ALFKI" &&
                                o.OrderDate != null)
                    .GroupBy(o => new {o.CustomerID, o.OrderDate.Value.Year})
                    .Select(g => new {g.Key.Year, Count = g.Count()})
                    .Where(k => k.Year == 1995)
                    .OrderBy(k => k.Year),
                assertOrder: true);
        }
    }
}
