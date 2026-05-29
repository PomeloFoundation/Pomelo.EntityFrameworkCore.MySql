using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;

namespace Microting.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities.DebugServices;

public class DebugRelationalCommandBuilder : RelationalCommandBuilder
{
    public DebugRelationalCommandBuilder([NotNull] RelationalCommandBuilderDependencies dependencies)
        : base(dependencies)
    {
    }

    public override IRelationalCommand Build()
    {
        var relationalCommand = base.Build();
        return new DebugRelationalCommand(Dependencies, relationalCommand.CommandText, relationalCommand.LogCommandText, Parameters);
    }
}
