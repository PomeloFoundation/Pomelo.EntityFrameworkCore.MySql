using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Pomelo.EntityFrameworkCore.MySql.TestUtilities.FakeProvider;

public class FakeRelationalOptionsExtension : RelationalOptionsExtension
{
    private DbContextOptionsExtensionInfo _info;

    public FakeRelationalOptionsExtension()
    {
    }

    protected FakeRelationalOptionsExtension(FakeRelationalOptionsExtension copyFrom)
        : base(copyFrom)
    {
    }

    public override DbContextOptionsExtensionInfo Info
        => _info ??= new ExtensionInfo(this);

    protected override RelationalOptionsExtension Clone()
        => new FakeRelationalOptionsExtension(this);

    public override void ApplyServices(IServiceCollection services)
        => AddEntityFrameworkRelationalDatabase(services);

    public static IServiceCollection AddEntityFrameworkRelationalDatabase(IServiceCollection serviceCollection)
    {
        var builder = new EntityFrameworkRelationalServicesBuilder(serviceCollection);

        // Specific test services are available upstream if we need them
        builder.TryAddCoreServices();

        return serviceCollection;
    }

    private sealed class ExtensionInfo : RelationalExtensionInfo
    {
        public ExtensionInfo(IDbContextOptionsExtension extension)
            : base(extension)
        {
        }

        public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
        {
        }
    }
}
