using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.EntityFrameworkCore.Scaffolding.Internal
{
    public class MySqlDesignTimeServices
    {
        public virtual IServiceCollection ConfigureDesignTimeServices(IServiceCollection serviceCollection)
            => serviceCollection
                .AddSingleton<IScaffoldingModelFactory, MySqlScaffoldingModelFactory>()
                .AddSingleton<IRelationalAnnotationProvider, MySqlAnnotationProvider>()
                .AddSingleton<IDatabaseModelFactory, MySqlDatabaseModelFactory>()
                .AddSingleton<MySqlTypeMapper>()
                .AddScoped<IRelationalTypeMapper, MySqlDesignTimeScopedTypeMapper>();
    }
}
