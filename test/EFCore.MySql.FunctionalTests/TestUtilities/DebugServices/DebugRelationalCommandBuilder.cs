using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities.DebugServices;

public class DebugRelationalCommandBuilder : RelationalCommandBuilder
{
    public DebugRelationalCommandBuilder([NotNull] RelationalCommandBuilderDependencies dependencies)
        : base(dependencies)
    {
    }

    public override IRelationalCommand Build()
    {
        var command = base.Build();
        return new DebugRelationalCommand(Dependencies, command.CommandText, command.CommandText, Parameters);
    }
}
