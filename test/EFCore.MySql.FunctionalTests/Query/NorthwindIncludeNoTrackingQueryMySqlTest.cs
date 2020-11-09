﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestModels.Northwind;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Storage;
using Pomelo.EntityFrameworkCore.MySql.Tests.TestUtilities.Attributes;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class NorthwindIncludeNoTrackingQueryMySqlTest : NorthwindIncludeNoTrackingQueryTestBase<NorthwindQueryMySqlFixture<NoopModelCustomizer>>
    {
        // ReSharper disable once UnusedParameter.Local
        public NorthwindIncludeNoTrackingQueryMySqlTest(NorthwindQueryMySqlFixture<NoopModelCustomizer> fixture)
            : base(fixture)
        {
            ClearLog();
        }

        public override async Task Include_collection_with_last_no_orderby(bool async)
            => Assert.Equal(
                RelationalStrings.LastUsedWithoutOrderBy(nameof(Enumerable.Last)),
                (await Assert.ThrowsAsync<InvalidOperationException>(
                    () => base.Include_collection_with_last_no_orderby(async))).Message);

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override Task Include_in_let_followed_by_FirstOrDefault(bool async)
        {
            return base.Include_in_let_followed_by_FirstOrDefault(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.CrossApply))]
        public override Task Include_collection_with_cross_apply_with_filter(bool async)
        {
            return base.Include_collection_with_cross_apply_with_filter(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterApply))]
        public override Task Include_collection_with_outer_apply_with_filter(bool async)
        {
            return base.Include_collection_with_outer_apply_with_filter(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterApply))]
        public override Task Include_collection_with_outer_apply_with_filter_non_equality(bool async)
        {
            return base.Include_collection_with_outer_apply_with_filter_non_equality(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterApply))]
        public override Task Filtered_include_with_multiple_ordering(bool async)
        {
            return base.Filtered_include_with_multiple_ordering(async);
        }

        public override Task Include_duplicate_collection_result_operator2(bool async)
        {
            // The order of `Orders` can be different, becaues it is not explicitly sorted.
            // The order of the end result can be different as well.
            // This is the case on MariaDB.
            return AssertQuery(
                async,
                ss => (from c1 in ss.Set<Customer>().Include(c => c.Orders).OrderBy(c => c.CustomerID).ThenBy(c => c.Orders.FirstOrDefault() != null ? c.Orders.FirstOrDefault().CustomerID : null).Take(2)
                    from c2 in ss.Set<Customer>().OrderBy(c => c.CustomerID).Skip(2).Take(2)
                    select new { c1, c2 }).OrderBy(t => t.c1.CustomerID).ThenBy(t => t.c2.CustomerID).Take(1),
                elementSorter: e => (e.c1.CustomerID, e.c2.CustomerID),
                elementAsserter: (e, a) =>
                {
                    AssertInclude(e.c1, a.c1, new ExpectedInclude<Customer>(c => c.Orders));
                    AssertEqual(e.c2, a.c2);
                },
                entryCount: 8);
        }

        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);

        protected override void ClearLog()
            => Fixture.TestSqlLoggerFactory.Clear();
    }
}
