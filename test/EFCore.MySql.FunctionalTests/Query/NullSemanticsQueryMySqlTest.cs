using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestModels.NullSemanticsModel;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class NullSemanticsQueryMySqlTest : NullSemanticsQueryTestBase<NullSemanticsQueryMySqlFixture>
    {
        public NullSemanticsQueryMySqlTest(NullSemanticsQueryMySqlFixture fixture)
            : base(fixture)
        {
            ClearLog();
        }

        protected override void ClearLog()
            => Fixture.TestSqlLoggerFactory.Clear();

        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);

        [Fact]
        public override void From_sql_composed_with_relational_null_comparison()
        {
            using (var context = CreateContext(useRelationalNulls: true))
            {
                var actual = context.Entities1
                    .FromSqlRaw(@"SELECT * FROM `Entities1`")
                    .Where(c => c.StringA == c.StringB)
                    .ToArray();

                Assert.Equal(15, actual.Length);
            }
        }

        [Fact(Skip = "issue #573")]
        public override void Projecting_nullable_bool_with_coalesce()
        {
            base.Projecting_nullable_bool_with_coalesce();
        }

        [Fact(Skip = "issue #573")]
        public override void Projecting_nullable_bool_with_coalesce_nested()
        {
            base.Projecting_nullable_bool_with_coalesce_nested();
        }

        [Fact]
        public virtual void Compare_left_bool_parameter_with_right_nullable_hasvalue()
        {
            bool prm = false;

            AssertQuery<NullSemanticsEntity1>(
                ss => ss.Where(e => prm == e.NullableBoolC.HasValue),
                ss => ss.Where(e => !e.NullableBoolC.HasValue),
                useRelationalNulls: false);

            AssertSql(
                @"@__prm_0='False'

SELECT `e`.`Id`
FROM `Entities1` AS `e`
WHERE @__prm_0 = (`e`.`NullableBoolC` IS NOT NULL)");
        }

        protected override NullSemanticsContext CreateContext(bool useRelationalNulls = false)
        {
            var options = new DbContextOptionsBuilder(Fixture.CreateOptions());
            if (useRelationalNulls)
            {
                new MySqlDbContextOptionsBuilder(options).UseRelationalNulls();
            }

            var context = new NullSemanticsContext(options.Options);

            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            return context;
        }
    }
}
