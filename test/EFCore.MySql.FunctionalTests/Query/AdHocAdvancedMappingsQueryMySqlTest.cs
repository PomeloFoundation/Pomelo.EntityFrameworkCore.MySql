using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query;

public class AdHocAdvancedMappingsQueryMySqlTest : AdHocAdvancedMappingsQueryRelationalTestBase
{
    [SkippableTheory]
    public override async Task Query_generates_correct_datetime2_parameter_definition(int? fractionalSeconds, string postfix)
    {
        Skip.If(fractionalSeconds > 6, "MySQL has a max. DateTime precision of 6.");

        await base.Query_generates_correct_datetime2_parameter_definition(fractionalSeconds, postfix);
    }

    [SkippableTheory]
    public override async Task Query_generates_correct_datetimeoffset_parameter_definition(int? fractionalSeconds, string postfix)
    {
        Skip.If(fractionalSeconds > 6, "MySQL has a max. DateTimeOffset precision of 6.");

        await base.Query_generates_correct_datetimeoffset_parameter_definition(fractionalSeconds, postfix);
    }

    [SkippableTheory]
    public override async Task Query_generates_correct_timespan_parameter_definition(int? fractionalSeconds, string postfix)
    {
        Skip.If(fractionalSeconds > 6, "MySQL has a max. TimeSpan precision of 6.");

        await base.Query_generates_correct_timespan_parameter_definition(fractionalSeconds, postfix);
    }

    protected override ITestStoreFactory TestStoreFactory
        => MySqlTestStoreFactory.Instance;
}
