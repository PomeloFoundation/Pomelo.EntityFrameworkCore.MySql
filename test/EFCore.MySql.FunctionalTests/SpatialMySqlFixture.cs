using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
{
    public class SpatialMySqlFixture : SpatialFixtureBase
    {
        protected override ITestStoreFactory TestStoreFactory
            => MySqlTestStoreFactory.Instance;

        protected override IServiceCollection AddServices(IServiceCollection serviceCollection)
            => base.AddServices(serviceCollection)
                /*.AddEntityFrameworkMySqlNetTopologySuite()*/;

        public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
        {
            var optionsBuilder = base.AddOptions(builder);
            new MySqlDbContextOptionsBuilder(optionsBuilder)/*.UseNetTopologySuite()*/;

            return optionsBuilder;
        }
    }
}
