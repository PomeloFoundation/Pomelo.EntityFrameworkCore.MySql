using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.EntityFrameworkCore.Design.Internal
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
