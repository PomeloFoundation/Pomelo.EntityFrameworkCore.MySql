using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query;

public class CaseSensitiveWithStringComparisonNorthwindQueryMySqlFixture<TModelCustomizer> : CaseSensitiveNorthwindQueryMySqlFixture<TModelCustomizer>
    where TModelCustomizer : ITestModelCustomizer, new()
{
    public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
    {
        var optionsBuilder = base.AddOptions(builder);
        new MySqlDbContextOptionsBuilder(optionsBuilder).EnableStringComparisonTranslations();
        return optionsBuilder;
    }
}
