using System.Collections.Generic;
using System.Reflection;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities.Xunit;

public class MySqlXunitTestFrameworkExecutor : XunitTestFrameworkExecutor
{
    public MySqlXunitTestFrameworkExecutor(
        AssemblyName assemblyName,
        ISourceInformationProvider sourceInformationProvider,
        IMessageSink diagnosticMessageSink)
        : base(assemblyName, sourceInformationProvider, diagnosticMessageSink)
    {
    }

    protected override async void RunTestCases(
        IEnumerable<IXunitTestCase> testCases,
        IMessageSink executionMessageSink,
        ITestFrameworkExecutionOptions executionOptions)
    {
        using var assemblyRunner = new MySqlXunitTestAssemblyRunner(
            TestAssembly,
            testCases,
            DiagnosticMessageSink,
            executionMessageSink,
            executionOptions);
        await assemblyRunner.RunAsync();
    }
}
