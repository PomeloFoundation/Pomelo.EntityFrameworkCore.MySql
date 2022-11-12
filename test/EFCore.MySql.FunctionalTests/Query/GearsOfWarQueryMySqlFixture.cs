using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestModels.GearsOfWarModel;
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class GearsOfWarQueryMySqlFixture : GearsOfWarQueryRelationalFixture, IQueryFixtureBase
    {
        protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;

        public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
        {
            var optionsBuilder = base.AddOptions(builder);

            new MySqlDbContextOptionsBuilder(optionsBuilder)
                .EnableIndexOptimizedBooleanColumns(true);

            return optionsBuilder;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
        {
            base.OnModelCreating(modelBuilder, context);

            modelBuilder.Entity<Weapon>().HasIndex(e => e.IsAutomatic);
        }

        public new ISetSource GetExpectedData()
        {
            var data = (GearsOfWarData)base.GetExpectedData();

            foreach (var mission in data.Missions)
            {
                mission.Timeline = MySqlTestHelpers.GetExpectedValue(mission.Timeline);
            }

            return data;
        }
    }
}
