using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microting.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Xunit;

namespace Microting.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class NorthwindJoinQueryMySqlTest : NorthwindJoinQueryRelationalTestBase<NorthwindQueryMySqlFixture<NoopModelCustomizer>>
    {
        public NorthwindJoinQueryMySqlTest(NorthwindQueryMySqlFixture<NoopModelCustomizer> fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            ClearLog();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        /// <summary>
        /// Needs explicit ordering of views to work consistently with MySQL and MariaDB.
        /// But since CustomerViewModel is private, we can't even override the test case properly.
        /// </summary>
        [Theory(Skip = "Needs explicit ordering of views to work consistently with MySQL and MariaDB.")]
        public override async Task SelectMany_with_client_eval_with_constructor(bool async)
        {
            // await AssertQuery(
            //     async,
            //     ss => ss.Set<Customer>()
            //         .Where(c => c.CustomerID.StartsWith("A"))
            //         .OrderBy(c => c.CustomerID)
            //         .Select(
            //             c => new CustomerViewModel(
            //                 c.CustomerID, c.City,
            //                 c.Orders.SelectMany(
            //                         o => o.OrderDetails
            //                             .Where(od => od.OrderID < 11000)
            //                             .Select(od => new OrderDetailViewModel(od.OrderID, od.ProductID)))
            //                     .ToArray())),
            //     assertOrder: true);

            await base.SelectMany_with_client_eval_with_constructor(async);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`City`, `t0`.`OrderID`, `t0`.`ProductID`, `t0`.`OrderID0`
FROM `Customers` AS `c`
LEFT JOIN (
    SELECT `t`.`OrderID`, `t`.`ProductID`, `o`.`OrderID` AS `OrderID0`, `o`.`CustomerID`
    FROM `Orders` AS `o`
    INNER JOIN (
        SELECT `o0`.`OrderID`, `o0`.`ProductID`
        FROM `Order Details` AS `o0`
        WHERE `o0`.`OrderID` < 11000
    ) AS `t` ON `o`.`OrderID` = `t`.`OrderID`
) AS `t0` ON `c`.`CustomerID` = `t0`.`CustomerID`
WHERE `c`.`CustomerID` LIKE 'A%'
ORDER BY `c`.`CustomerID`, `t0`.`OrderID0`, `t0`.`OrderID`");
        }

        // https://github.com/npgsql/efcore.pg/issues/2759
        // EF Core 10 changed the translation for this test so it no longer requires primitive collections support.
        // The test now passes for both MySQL and MariaDB without needing JsonTable/primitive collections.
        public override Task Join_local_collection_int_closure_is_cached_correctly(bool async)
            => base.Join_local_collection_int_closure_is_cached_correctly(async);

        // Neither MySQL nor MariaDB support FULL JOIN, so the provider rejects it during translation.
        public override Task FullJoin(bool async)
            => AssertTranslationFailed(() => base.FullJoin(async));

        public override Task FullJoin_with_unmatched_rows_on_both_sides(bool async)
            => AssertTranslationFailed(() => base.FullJoin_with_unmatched_rows_on_both_sides(async));

        /// <summary>
        /// The base test joins an `int` column against the characters of a string and expects no matches, because
        /// LINQ to Objects compares the `int` against the character's numeric code point. MySQL and MariaDB instead
        /// coerce the string operands to numbers, so `EmployeeID` actually matches the '1' and '2' elements.
        /// </summary>
        [Theory(Skip = "MySQL and MariaDB implicitly coerce strings to numbers when comparing them against a numeric column, so this query returns rows where LINQ to Objects returns none.")]
        public override Task Join_local_string_closure_is_cached_correctly(bool async)
            => base.Join_local_string_closure_is_cached_correctly(async);

        /// <summary>
        /// The base test expects joining an `int` column against a `byte[]` to fail translation. MySQL and MariaDB
        /// translate the byte elements as numeric values, so the query is translated successfully.
        /// </summary>
        [Theory(Skip = "MySQL and MariaDB translate a join between a numeric column and byte[] elements, while the base test expects translation to fail.")]
        public override Task Join_local_bytes_closure_is_cached_correctly(bool async)
            => base.Join_local_bytes_closure_is_cached_correctly(async);

        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);

        protected override void ClearLog()
            => Fixture.TestSqlLoggerFactory.Clear();
    }
}
