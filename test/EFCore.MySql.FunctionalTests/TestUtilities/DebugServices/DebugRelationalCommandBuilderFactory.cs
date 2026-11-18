using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;

namespace Microting.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities.DebugServices;

public class DebugRelationalCommandBuilderFactory : RelationalCommandBuilderFactory
{
    public DebugRelationalCommandBuilderFactory([NotNull] RelationalCommandBuilderDependencies dependencies)
        : base(dependencies)
    {
    }

    public override IRelationalCommandBuilder Create()
        => new DebugRelationalCommandBuilder(Dependencies);
}
