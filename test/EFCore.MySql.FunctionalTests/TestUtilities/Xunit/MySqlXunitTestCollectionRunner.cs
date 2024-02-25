using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities.Xunit;

public class MySqlXunitTestCollectionRunner : XunitTestCollectionRunner
{
    public MySqlXunitTestCollectionRunner(
        ITestCollection testCollection,
        IEnumerable<IXunitTestCase> testCases,
        IMessageSink diagnosticMessageSink,
        IMessageBus messageBus,
        ITestCaseOrderer testCaseOrderer,
        ExceptionAggregator aggregator,
        CancellationTokenSource cancellationTokenSource)
        : base(testCollection, testCases, diagnosticMessageSink, messageBus, testCaseOrderer, aggregator, cancellationTokenSource)
    {
    }

    protected override async Task<RunSummary> RunTestClassesAsync()
    {
        var summary = new RunSummary();

        var testsGroupedByClass = TestCases.GroupBy(tc => tc.TestMethod.TestClass, TestClassComparer.Instance);

        // Explicitly order test classes.
        if (TestCaseOrderer is IMySqlTestClassOrderer testClassOrderer)
        {
            var testClassesWithIndex = testClassOrderer
                .OrderTestClasses(testsGroupedByClass.Select(g => g.Key))
                .Select((s, i) => (s, i))
                .ToDictionary(t => t.s, t => t.i);

            testsGroupedByClass = testsGroupedByClass
                .OrderBy(g => testClassesWithIndex.GetValueOrDefault(g.Key, int.MaxValue));
        }

        foreach (var testCasesByClass in testsGroupedByClass)
        {
            summary.Aggregate(await RunTestClassAsync(testCasesByClass.Key, (IReflectionTypeInfo)testCasesByClass.Key.Class, testCasesByClass));
            if (CancellationTokenSource.IsCancellationRequested)
                break;
        }

        return summary;
    }
}
