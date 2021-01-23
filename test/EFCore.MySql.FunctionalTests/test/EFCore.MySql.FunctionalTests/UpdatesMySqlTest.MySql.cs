using System;
using System.Linq;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
{
    public partial class UpdatesMySqlTest
    {
        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);

        [ConditionalFact]
        public virtual void Select_where_without_default_value_after_insert()
        {
            ExecuteWithStrategyInTransaction(
                context =>
                {
                    context.Set<UpdatesMySqlFixture.Models.Issue1300.Flavor>().Add(
                        new UpdatesMySqlFixture.Models.Issue1300.Flavor());

                    context.SaveChanges();

                    AssertSql(
                        @"INSERT INTO `Flavor` ()
VALUES ();
SELECT `DiscoveryDate`, `FlavorId`
FROM `Flavor`
WHERE ROW_COUNT() = 1 AND `FlavorId` = LAST_INSERT_ID();");
                },
                context =>
                {
                    var flavors = context.Set<UpdatesMySqlFixture.Models.Issue1300.Flavor>()
                        .ToList();

                    Assert.Single(flavors);
                    Assert.Equal(1, flavors[0].FlavorId);
                    Assert.True(DateTime.Now - flavors[0].DiscoveryDate < TimeSpan.FromDays(1));
                });
        }
    }
}
