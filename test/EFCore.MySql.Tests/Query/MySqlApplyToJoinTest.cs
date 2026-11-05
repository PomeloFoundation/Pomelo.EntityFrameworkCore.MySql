using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microting.EntityFrameworkCore.MySql.Infrastructure;
using Xunit;

namespace Microting.EntityFrameworkCore.MySql.Query
{
    /// <summary>
    /// Database-less translation tests (<c>ToQueryString()</c>) for the conversion of <c>OUTER APPLY</c>/<c>CROSS APPLY</c> to
    /// joins on servers without <c>LATERAL</c> support (MariaDB, MySQL &lt; 8.0.14).
    /// See https://github.com/microting/Pomelo.EntityFrameworkCore.MySql/issues/467 and https://github.com/dotnet/efcore/issues/39220.
    /// </summary>
    public class MySqlApplyToJoinTest
    {
        private static readonly ServerVersion MariaDb = new MariaDbServerVersion(new Version(10, 11, 0));
        private static readonly ServerVersion MySql8 = new MySqlServerVersion(new Version(8, 0, 30));

        [Fact]
        public void Grouped_subquery_with_correlated_key_and_DefaultIfEmpty_is_translated_to_left_join_on_MariaDb()
        {
            using var context = new TestContext(MariaDb);

            var sql = GroupedSubqueryWithDefaultIfEmpty(context).ToQueryString();

            AssertSql(
                """
SELECT `u`.`Id`, COALESCE(`t0`.`Amount`, 0)
FROM `Users` AS `u`
LEFT JOIN (
    SELECT `t1`.`Amount`, `t1`.`UserId`
    FROM (
        SELECT COALESCE(SUM(`t`.`Amount`), 0) AS `Amount`, `t`.`UserId`
        FROM `Transactions` AS `t`
        GROUP BY `t`.`UserId`
    ) AS `t1`
) AS `t0` ON `t0`.`UserId` = `u`.`Id`
""",
                sql);
        }

        [Fact]
        public void Grouped_subquery_with_correlated_key_and_DefaultIfEmpty_is_translated_to_left_join_on_MySql_without_lateral()
        {
            using var context = new TestContext(new MySqlServerVersion(new Version(5, 7, 0)));

            var sql = GroupedSubqueryWithDefaultIfEmpty(context).ToQueryString();

            Assert.Contains("LEFT JOIN (", sql);
            Assert.DoesNotContain("LATERAL", sql);
            Assert.Contains("ON `t0`.`UserId` = `u`.`Id`", sql);
        }

        [Fact]
        public void Grouped_subquery_with_correlated_key_and_DefaultIfEmpty_still_uses_lateral_on_MySql_8()
        {
            using var context = new TestContext(MySql8);

            var sql = GroupedSubqueryWithDefaultIfEmpty(context).ToQueryString();

            Assert.Contains("LEFT JOIN LATERAL (", sql);
        }

        [Fact]
        public void Grouped_subquery_with_additional_having_predicate_keeps_it_inside_the_subquery_on_MariaDb()
        {
            using var context = new TestContext(MariaDb);

            var subQuery = from t in context.Transactions
                           group t.Amount by t.UserId into g
                           select new { g.Key, Amount = g.Sum() };
            var query = from u in context.Users
                        from t in subQuery.Where(j => j.Key == u.Id && j.Amount > 5).DefaultIfEmpty()
                        select new { u.Id, Amount = (int?)t.Amount };

            AssertSql(
                """
SELECT `u`.`Id`, `t0`.`Amount`
FROM `Users` AS `u`
LEFT JOIN (
    SELECT COALESCE(SUM(`t`.`Amount`), 0) AS `Amount`, `t`.`UserId`
    FROM `Transactions` AS `t`
    GROUP BY `t`.`UserId`
    HAVING COALESCE(SUM(`t`.`Amount`), 0) > 5
) AS `t0` ON `t0`.`UserId` = `u`.`Id`
""",
                query.ToQueryString());
        }

        [Fact]
        public void Distinct_subquery_with_correlated_key_and_DefaultIfEmpty_is_translated_to_left_join_on_MariaDb()
        {
            using var context = new TestContext(MariaDb);

            var query = from u in context.Users
                        from userId in context.Transactions.Select(t => t.UserId).Distinct().Where(x => x == u.Id).DefaultIfEmpty()
                        select new { u.Id, userId };

            AssertSql(
                """
SELECT `u`.`Id`, `t0`.`c` AS `userId`
FROM `Users` AS `u`
LEFT JOIN (
    SELECT DISTINCT COALESCE(`t`.`UserId`, 0) AS `c`, `t`.`UserId`
    FROM `Transactions` AS `t`
) AS `t0` ON `t0`.`UserId` = `u`.`Id`
""",
                query.ToQueryString());
        }

        [Fact]
        public void Subquery_correlated_outside_of_a_grouping_key_is_still_rejected_on_MariaDb()
        {
            using var context = new TestContext(MariaDb);

            // The outer column is used inside the aggregate, so the apply cannot be turned into a join.
            var query = from u in context.Users
                        from sum in context.Transactions
                            .Where(t => t.UserId == u.Id)
                            .GroupBy(t => t.UserId)
                            .Select(g => g.Sum(x => x.Amount + u.Id))
                            .DefaultIfEmpty()
                        select new { u.Id, sum };

            var exception = Assert.Throws<InvalidOperationException>(() => query.ToQueryString());

            Assert.Contains("OUTER APPLY", exception.Message);
        }

        // The query of https://github.com/microting/Pomelo.EntityFrameworkCore.MySql/issues/467.
        private static IQueryable<object> GroupedSubqueryWithDefaultIfEmpty(TestContext context)
        {
            var subQuery = from t in context.Transactions
                           group t.Amount by t.UserId into g
                           select new { g.Key, Amount = g.Sum() };

            return from u in context.Users
                   from t in subQuery.Where(j => j.Key == u.Id).DefaultIfEmpty()
                   select (object)new { u.Id, Amount = (int?)t.Amount ?? 0 };
        }

        private static void AssertSql(string expected, string actual)
            => Assert.Equal(
                expected.Replace("\r\n", "\n").Trim(),
                actual.Replace("\r\n", "\n").Trim());

        public class User
        {
            public int Id { get; set; }
        }

        public class Transaction
        {
            public int Id { get; set; }
            public int UserId { get; set; }
            public int Amount { get; set; }
        }

        private class TestContext : DbContext
        {
            private readonly ServerVersion _serverVersion;

            public TestContext(ServerVersion serverVersion)
                => _serverVersion = serverVersion;

            public DbSet<User> Users { get; set; }
            public DbSet<Transaction> Transactions { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseMySql("Server=localhost;Database=test;", _serverVersion);
        }
    }
}
