using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.EntityFrameworkCore.Design.Internal
{
    public class MySqlDesignTimeServices : IDesignTimeServices
    {
        public void ConfigureDesignTimeServices(IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddSingleton<IScaffoldingProviderCodeGenerator, MySqlScaffoldingCodeGenerator>()
                .AddSingleton<IDatabaseModelFactory, MySqlDatabaseModelFactory>()
                .AddSingleton<IAnnotationCodeGenerator, MySqlAnnotationCodeGenerator>()
                .AddSingleton<IRelationalTypeMapper, MySqlTypeMapper>();
        }
    }
}
