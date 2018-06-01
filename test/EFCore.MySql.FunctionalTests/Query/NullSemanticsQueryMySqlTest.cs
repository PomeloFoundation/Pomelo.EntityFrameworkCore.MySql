using System.Linq;
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
        }

        [Fact]
        public override void From_sql_composed_with_relational_null_comparison()
        {
            using (var context = CreateContext(useRelationalNulls: true))
            {
                var actual = context.Entities1
                    .FromSql(@"SELECT * FROM `Entities1`")
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
