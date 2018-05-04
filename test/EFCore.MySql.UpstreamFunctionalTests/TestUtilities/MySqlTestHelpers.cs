using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;

//ReSharper disable once CheckNamespace
namespace EFCore.MySql.UpstreamFunctionalTests.TestUtilities
{
    public class MySqlTestHelpers : TestHelpers
    {
        protected MySqlTestHelpers()
        {
        }

        public static MySqlTestHelpers Instance { get; } = new MySqlTestHelpers();

        public override IServiceCollection AddProviderServices(IServiceCollection services)
            => services.AddEntityFrameworkMySql();

        protected override void UseProviderOptions(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseMySql("Database=DummyDatabase");
    }
}
