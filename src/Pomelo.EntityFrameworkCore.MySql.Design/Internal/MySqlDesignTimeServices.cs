using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.EntityFrameworkCore.Scaffolding.Internal
{
    public class MySqlDesignTimeServices
    {
        public virtual IServiceCollection ConfigureDesignTimeServices(/* [NotNull] */ IServiceCollection serviceCollection)
            => serviceCollection
                .AddSingleton<IScaffoldingModelFactory, RelationalScaffoldingModelFactory>()
                .AddSingleton<IRelationalAnnotationProvider, MySqlAnnotationProvider>()
                .AddSingleton<IRelationalTypeMapper, MySqlTypeMapper>()
                .AddSingleton<IDatabaseModelFactory, MySqlDatabaseModelFactory>();
    }
}
