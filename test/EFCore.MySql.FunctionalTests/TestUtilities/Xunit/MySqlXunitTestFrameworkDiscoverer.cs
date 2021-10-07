using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.TestUtilities.Xunit;
using Pomelo.EntityFrameworkCore.MySql.Tests;
using Pomelo.EntityFrameworkCore.MySql.Tests.TestUtilities.Attributes;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities.Xunit
{
    public class MySqlXunitTestFramework : XunitTestFramework
    {
        public MySqlXunitTestFramework(IMessageSink messageSink) : base(messageSink)
        {
        }

        protected override ITestFrameworkDiscoverer CreateDiscoverer(IAssemblyInfo assemblyInfo)
            => new MySqlXunitTestFrameworkDiscoverer(assemblyInfo, SourceInformationProvider, DiagnosticMessageSink);

        // protected override ITestFrameworkExecutor CreateExecutor(AssemblyName assemblyName)
        //     => new MySqlXunitTestFrameworkExecutor(assemblyName, SourceInformationProvider, DiagnosticMessageSink);
    }

    public class MySqlXunitTestFrameworkDiscoverer : XunitTestFrameworkDiscoverer
    {
        public MySqlXunitTestFrameworkDiscoverer(
            IAssemblyInfo assemblyInfo,
            ISourceInformationProvider sourceProvider,
            IMessageSink diagnosticMessageSink,
            IXunitTestCollectionFactory collectionFactory = null)
            : base(
                assemblyInfo,
                sourceProvider,
                diagnosticMessageSink,
                collectionFactory)
        {
            // Prime the cache with our own discoverers, so they get used over the original ones from EF Core.
            DiscovererTypeCache.Add(typeof(ConditionalFactAttribute), typeof(MySqlConditionalFactDiscoverer));
            DiscovererTypeCache.Add(typeof(ConditionalTheoryAttribute), typeof(MySqlConditionalTheoryDiscoverer));
        }

        protected override bool IsValidTestClass(ITypeInfo type)
            => base.IsValidTestClass(type) &&
               IsTestConditionMet<SupportedServerVersionConditionAttribute>(type) &&
               IsTestConditionMet<SupportedServerVersionLessThanConditionAttribute>(type);

        protected virtual bool IsTestConditionMet<TType>(ITypeInfo type) where TType : ITestCondition
            => GetTestConditions<TType>(type).Aggregate(true, (current, next) => current && next.IsMetAsync().Result);

        protected virtual IEnumerable<ITestCondition> GetTestConditions<TType>(ITypeInfo type) where TType : ITestCondition
            => type.GetCustomAttributes(typeof(TType))
                .Select(attribute => (TType)Activator.CreateInstance(typeof(TType), attribute.GetConstructorArguments().ToArray()))
                .Cast<ITestCondition>();
    }

    public class MySqlConditionalFactDiscoverer : ConditionalFactDiscoverer
    {
        public MySqlConditionalFactDiscoverer(IMessageSink messageSink)
            : base(messageSink)
        {
        }

        protected override IXunitTestCase CreateTestCase(
            ITestFrameworkDiscoveryOptions discoveryOptions,
            ITestMethod testMethod,
            IAttributeInfo factAttribute)
            => new MySqlConditionalFactTestCase(
                DiagnosticMessageSink,
                discoveryOptions.MethodDisplayOrDefault(),
                discoveryOptions.MethodDisplayOptionsOrDefault(),
                testMethod);
    }

    public class MySqlConditionalTheoryDiscoverer : ConditionalTheoryDiscoverer
    {
        public MySqlConditionalTheoryDiscoverer(IMessageSink messageSink)
            : base(messageSink)
        {
        }

        protected override IEnumerable<IXunitTestCase> CreateTestCasesForTheory(
            ITestFrameworkDiscoveryOptions discoveryOptions,
            ITestMethod testMethod,
            IAttributeInfo theoryAttribute)
        {
            yield return new MySqlConditionalTheoryTestCase(
                DiagnosticMessageSink,
                discoveryOptions.MethodDisplayOrDefault(),
                discoveryOptions.MethodDisplayOptionsOrDefault(),
                testMethod);
        }

        protected override IEnumerable<IXunitTestCase> CreateTestCasesForDataRow(
            ITestFrameworkDiscoveryOptions discoveryOptions,
            ITestMethod testMethod,
            IAttributeInfo theoryAttribute,
            object[] dataRow)
        {
            yield return new MySqlConditionalFactTestCase(
                DiagnosticMessageSink,
                discoveryOptions.MethodDisplayOrDefault(),
                discoveryOptions.MethodDisplayOptionsOrDefault(),
                testMethod,
                dataRow);
        }
    }

    /// <remarks>
    /// We cannot inherit from ConditionalFactTestCase, because it's sealed.
    /// </remarks>
    public sealed class MySqlConditionalFactTestCase : XunitTestCase
    {
        [Obsolete("Called by the de-serializer; should only be called by deriving classes for de-serialization purposes")]
        public MySqlConditionalFactTestCase()
        {
        }

        public MySqlConditionalFactTestCase(
            IMessageSink diagnosticMessageSink,
            TestMethodDisplay defaultMethodDisplay,
            TestMethodDisplayOptions defaultMethodDisplayOptions,
            ITestMethod testMethod,
            object[] testMethodArguments = null)
            : base(diagnosticMessageSink, defaultMethodDisplay, defaultMethodDisplayOptions, testMethod, testMethodArguments)
        {
        }

        public override async Task<RunSummary> RunAsync(
            IMessageSink diagnosticMessageSink,
            IMessageBus messageBus,
            object[] constructorArguments,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource)
            => await XunitTestCaseExtensions.TrySkipAsync(this, messageBus)
                ? new RunSummary { Total = 1, Skipped = 1 }
                : await new MySqlXunitTestCaseRunner(
                    this,
                    DisplayName,
                    SkipReason,
                    constructorArguments,
                    TestMethodArguments,
                    messageBus,
                    aggregator,
                    cancellationTokenSource).RunAsync();
    }

    /// <remarks>
    /// We cannot inherit from ConditionalTheoryTestCase, because it's sealed.
    /// </remarks>
    public sealed class MySqlConditionalTheoryTestCase : XunitTheoryTestCase
    {
        [Obsolete("Called by the de-serializer; should only be called by deriving classes for de-serialization purposes")]
        public MySqlConditionalTheoryTestCase()
        {
        }

        public MySqlConditionalTheoryTestCase(
            IMessageSink diagnosticMessageSink,
            TestMethodDisplay defaultMethodDisplay,
            TestMethodDisplayOptions defaultMethodDisplayOptions,
            ITestMethod testMethod)
            : base(diagnosticMessageSink, defaultMethodDisplay, defaultMethodDisplayOptions, testMethod)
        {
        }

        public override async Task<RunSummary> RunAsync(
            IMessageSink diagnosticMessageSink,
            IMessageBus messageBus,
            object[] constructorArguments,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource)
            => await XunitTestCaseExtensions.TrySkipAsync(this, messageBus)
                ? new RunSummary { Total = 1, Skipped = 1 }
                : await new MySqlXunitTheoryTestCaseRunner(
                        this,
                        DisplayName,
                        SkipReason,
                        constructorArguments,
                        diagnosticMessageSink,
                        messageBus,
                        aggregator,
                        cancellationTokenSource)
                    .RunAsync();
    }

    public class MySqlXunitTestCaseRunner : XunitTestCaseRunner
    {
        public MySqlXunitTestCaseRunner(
            IXunitTestCase testCase,
            string displayName,
            string skipReason,
            object[] constructorArguments,
            object[] testMethodArguments,
            IMessageBus messageBus,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource)
            : base(
                testCase,
                displayName,
                skipReason,
                constructorArguments,
                testMethodArguments,
                messageBus,
                aggregator,
                cancellationTokenSource)
        {
        }

        protected override XunitTestRunner CreateTestRunner(
            ITest test,
            IMessageBus messageBus,
            Type testClass,
            object[] constructorArguments,
            MethodInfo testMethod,
            object[] testMethodArguments,
            string skipReason,
            IReadOnlyList<BeforeAfterTestAttribute> beforeAfterAttributes,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource)
            => new MySqlXunitTestRunner(
                test,
                messageBus,
                testClass,
                constructorArguments,
                testMethod,
                testMethodArguments,
                skipReason,
                beforeAfterAttributes,
                new ExceptionAggregator(aggregator),
                cancellationTokenSource);

        /// <remarks>
        /// `TestRunner&lt;TTestCase&gt;.RunAsync()` is not virtual, so we need to override this method here to call our own
        /// `MySqlXunitTestRunner.RunAsync()` implementation.
        /// </remarks>>
        protected override Task<RunSummary> RunTestAsync()
            => ((MySqlXunitTestRunner)CreateTestRunner(
                    CreateTest(TestCase, DisplayName),
                    MessageBus,
                    TestClass,
                    ConstructorArguments,
                    TestMethod,
                    TestMethodArguments,
                    SkipReason,
                    BeforeAfterAttributes,
                    Aggregator,
                    CancellationTokenSource))
                .RunAsync();
    }

    public class MySqlXunitTheoryTestCaseRunner : XunitTheoryTestCaseRunner
    {
        public MySqlXunitTheoryTestCaseRunner(
            IXunitTestCase testCase,
            string displayName,
            string skipReason,
            object[] constructorArguments,
            IMessageSink diagnosticMessageSink,
            IMessageBus messageBus,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource)
            : base(
                testCase,
                displayName,
                skipReason,
                constructorArguments,
                diagnosticMessageSink,
                messageBus,
                aggregator,
                cancellationTokenSource)
        {
        }

        protected override XunitTestRunner CreateTestRunner(
            ITest test,
            IMessageBus messageBus,
            Type testClass,
            object[] constructorArguments,
            MethodInfo testMethod,
            object[] testMethodArguments,
            string skipReason,
            IReadOnlyList<BeforeAfterTestAttribute> beforeAfterAttributes,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource)
            => new MySqlXunitTestRunner(
                test,
                messageBus,
                testClass,
                constructorArguments,
                testMethod,
                testMethodArguments,
                skipReason,
                beforeAfterAttributes,
                new ExceptionAggregator(aggregator),
                cancellationTokenSource);
    }

    public class MySqlXunitTestRunner : XunitTestRunner
    {
        public MySqlXunitTestRunner(
            ITest test,
            IMessageBus messageBus,
            Type testClass,
            object[] constructorArguments,
            MethodInfo testMethod,
            object[] testMethodArguments,
            string skipReason,
            IReadOnlyList<BeforeAfterTestAttribute> beforeAfterAttributes,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource)
            : base(
                test,
                messageBus,
                testClass,
                constructorArguments,
                testMethod,
                testMethodArguments,
                skipReason,
                beforeAfterAttributes,
                aggregator,
                cancellationTokenSource)
        {
        }

        public new async Task<RunSummary> RunAsync()
        {
            var runSummary = new RunSummary { Total = 1 };
            var output = string.Empty;

            if (!MessageBus.QueueMessage(new TestStarting(Test)))
            {
                CancellationTokenSource.Cancel();
            }
            else
            {
                AfterTestStarting();

                if (!string.IsNullOrEmpty(SkipReason))
                {
                    ++runSummary.Skipped;

                    if (!MessageBus.QueueMessage(
                        new TestSkipped(Test, SkipReason)))
                    {
                        CancellationTokenSource.Cancel();
                    }
                }
                else
                {
                    var aggregator = new ExceptionAggregator(Aggregator);
                    if (!aggregator.HasExceptions)
                    {
                        var tuple = await aggregator.RunAsync(() => InvokeTestAsync(aggregator));
                        if (tuple != null)
                        {
                            runSummary.Time = tuple.Item1;
                            output = tuple.Item2;
                        }
                    }

                    TestResultMessage testResultMessage;

                    var exception = aggregator.ToException();
                    if (exception == null)
                    {
                        testResultMessage = new TestPassed(Test, runSummary.Time, output);
                    }
#region Customized
                    /// This is what we are after. Mark failed tests as 'Skipped', if their failure is expected.
                    else if (SkipFailedTest(exception))
                    {
                        testResultMessage = new TestSkipped(Test, exception.Message);
                        ++runSummary.Skipped;
                    }
#endregion Customized
                    else
                    {
                        testResultMessage = new TestFailed(Test, runSummary.Time, output, exception);
                        ++runSummary.Failed;
                    }

                    if (!CancellationTokenSource.IsCancellationRequested &&
                        !MessageBus.QueueMessage(testResultMessage))
                    {
                        CancellationTokenSource.Cancel();
                    }
                }

                Aggregator.Clear();

                BeforeTestFinished();

                if (Aggregator.HasExceptions && !MessageBus.QueueMessage(
                    new TestCleanupFailure(Test, Aggregator.ToException())))
                {
                    CancellationTokenSource.Cancel();
                }

                if (!MessageBus.QueueMessage(new TestFinished(Test, runSummary.Time, output)))
                {
                    CancellationTokenSource.Cancel();
                }
            }

            return runSummary;
        }

        /// <summary>
        /// Mark failed tests as 'Skipped', it they failed because they use an expression, that is not supported by the underlying database
        /// server version.
        /// </summary>
        protected virtual bool SkipFailedTest(Exception exception)
        {
            var skip = true;
            var supports = AppConfig.ServerVersion.Supports;
            var aggregateException = exception as AggregateException ??
                                     new AggregateException(exception);

            foreach (var innerException in aggregateException.InnerExceptions)
            {
                if (!skip ||
                    innerException is not InvalidOperationException ||
                    !innerException.Message.StartsWith("The LINQ expression '") ||
                    !innerException.Message.Contains("' could not be translated."))
                {
                    return false;
                }

                skip &= !supports.OuterApply && innerException.Message.Contains("OUTER APPLY") ||
                        !supports.CrossApply && innerException.Message.Contains("CROSS APPLY") ||
                        !supports.WindowFunctions && innerException.Message.Contains("ROW_NUMBER() OVER");
            }

            return skip;
        }
    }

    #region Unused

    // public class MySqlXunitTestFrameworkExecutor : XunitTestFrameworkExecutor
    // {
    //     public MySqlXunitTestFrameworkExecutor(
    //         AssemblyName assemblyName,
    //         ISourceInformationProvider sourceInformationProvider,
    //         IMessageSink diagnosticMessageSink)
    //         : base(
    //             assemblyName,
    //             sourceInformationProvider,
    //             diagnosticMessageSink)
    //     {
    //     }
    //
    //     /// <inheritdoc />
    //     /// <footer><a href="https://www.google.com/search?q=Xunit.Sdk.XunitTestFrameworkExecutor.RunTestCases">`XunitTestFrameworkExecutor.RunTestCases` on google.com</a></footer>
    //     protected override async void RunTestCases(
    //         IEnumerable<IXunitTestCase> testCases,
    //         IMessageSink executionMessageSink,
    //         ITestFrameworkExecutionOptions executionOptions)
    //     {
    //         using var assemblyRunner = new MySqlXunitTestAssemblyRunner(
    //             TestAssembly,
    //             testCases,
    //             DiagnosticMessageSink,
    //             executionMessageSink,
    //             executionOptions);
    //
    //         var runSummary = await assemblyRunner.RunAsync();
    //     }
    // }
    //
    // public class MySqlXunitTestAssemblyRunner : XunitTestAssemblyRunner
    // {
    //     public MySqlXunitTestAssemblyRunner(
    //         ITestAssembly testAssembly,
    //         IEnumerable<IXunitTestCase> testCases,
    //         IMessageSink diagnosticMessageSink,
    //         IMessageSink executionMessageSink,
    //         ITestFrameworkExecutionOptions executionOptions)
    //         : base(
    //             testAssembly,
    //             testCases,
    //             diagnosticMessageSink,
    //             executionMessageSink,
    //             executionOptions)
    //     {
    //     }
    //
    //     protected override Task<RunSummary> RunTestCollectionAsync(
    //         IMessageBus messageBus,
    //         ITestCollection testCollection,
    //         IEnumerable<IXunitTestCase> testCases,
    //         CancellationTokenSource cancellationTokenSource)
    //         => new MySqlXunitTestCollectionRunner(
    //             testCollection,
    //             testCases,
    //             DiagnosticMessageSink,
    //             messageBus,
    //             TestCaseOrderer,
    //             new ExceptionAggregator(Aggregator),
    //             cancellationTokenSource).RunAsync();
    // }
    //
    // public class MySqlXunitTestCollectionRunner : XunitTestCollectionRunner
    // {
    //     public MySqlXunitTestCollectionRunner(
    //         ITestCollection testCollection,
    //         IEnumerable<IXunitTestCase> testCases,
    //         IMessageSink diagnosticMessageSink,
    //         IMessageBus messageBus,
    //         ITestCaseOrderer testCaseOrderer,
    //         ExceptionAggregator aggregator,
    //         CancellationTokenSource cancellationTokenSource)
    //         : base(
    //             testCollection,
    //             testCases,
    //             diagnosticMessageSink,
    //             messageBus,
    //             testCaseOrderer,
    //             aggregator,
    //             cancellationTokenSource)
    //     {
    //     }
    //
    //     protected override Task<RunSummary> RunTestClassAsync(
    //         ITestClass testClass, IReflectionTypeInfo @class, IEnumerable<IXunitTestCase> testCases)
    //         => new MySqlXunitTestClassRunner(
    //                 testClass,
    //                 @class,
    //                 testCases,
    //                 DiagnosticMessageSink,
    //                 MessageBus,
    //                 TestCaseOrderer,
    //                 new ExceptionAggregator(Aggregator),
    //                 CancellationTokenSource,
    //                 CollectionFixtureMappings)
    //             .RunAsync();
    // }
    //
    // public class MySqlXunitTestClassRunner : XunitTestClassRunner
    // {
    //     public MySqlXunitTestClassRunner(
    //         ITestClass testClass,
    //         IReflectionTypeInfo @class,
    //         IEnumerable<IXunitTestCase> testCases,
    //         IMessageSink diagnosticMessageSink,
    //         IMessageBus messageBus,
    //         ITestCaseOrderer testCaseOrderer,
    //         ExceptionAggregator aggregator,
    //         CancellationTokenSource cancellationTokenSource,
    //         IDictionary<Type, object> collectionFixtureMappings)
    //         : base(
    //             testClass,
    //             @class,
    //             testCases,
    //             diagnosticMessageSink,
    //             messageBus,
    //             testCaseOrderer,
    //             aggregator,
    //             cancellationTokenSource,
    //             collectionFixtureMappings)
    //     {
    //     }
    //
    //     protected override Task<RunSummary> RunTestMethodAsync(
    //         ITestMethod testMethod, IReflectionMethodInfo method, IEnumerable<IXunitTestCase> testCases, object[] constructorArguments)
    //         => new MySqlXunitTestMethodRunner(
    //                 testMethod,
    //                 Class,
    //                 method,
    //                 testCases,
    //                 DiagnosticMessageSink,
    //                 MessageBus,
    //                 new ExceptionAggregator(Aggregator),
    //                 CancellationTokenSource,
    //                 constructorArguments)
    //             .RunAsync();
    // }
    //
    // public class MySqlXunitTestMethodRunner : XunitTestMethodRunner
    // {
    //     private readonly object[] constructorArguments;
    //     private readonly IMessageSink diagnosticMessageSink;
    //
    //     public MySqlXunitTestMethodRunner(
    //         ITestMethod testMethod,
    //         IReflectionTypeInfo @class,
    //         IReflectionMethodInfo method,
    //         IEnumerable<IXunitTestCase> testCases,
    //         IMessageSink diagnosticMessageSink,
    //         IMessageBus messageBus,
    //         ExceptionAggregator aggregator,
    //         CancellationTokenSource cancellationTokenSource,
    //         object[] constructorArguments)
    //         : base(
    //             testMethod,
    //             @class,
    //             method,
    //             testCases,
    //             diagnosticMessageSink,
    //             messageBus,
    //             aggregator,
    //             cancellationTokenSource,
    //             constructorArguments)
    //     {
    //         this.constructorArguments = constructorArguments;
    //         this.diagnosticMessageSink = diagnosticMessageSink;
    //     }
    //
    //     protected override Task<RunSummary> RunTestCaseAsync(IXunitTestCase testCase)
    //     {
    //         return testCase.RunAsync(
    //             diagnosticMessageSink,
    //             MessageBus,
    //             constructorArguments,
    //             new ExceptionAggregator(Aggregator),
    //             CancellationTokenSource);
    //     }
    // }

    #endregion Unused
}
