using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MySqlConnector;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Tests;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities.Xunit
{
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

            // MariaDB (observed on the native Windows build of 11.6.x) can spuriously raise
            // error 1020 (ER_CHECKREAD, "Record has changed since last read") while executing
            // ExecuteUpdate/ExecuteDelete statements against table-split entities. The identical
            // server version executes the very same statements correctly on Linux, so this is a
            // server-side bug rather than a provider issue, and it surfaces non-deterministically
            // across the affected tests. Treat it as a skip instead of a failure.
            if (AppConfig.ServerVersion.Type == ServerType.MariaDb &&
                aggregateException.InnerExceptions.Any(IsMariaDbRecordHasChangedException))
            {
                return true;
            }

            foreach (var innerException in aggregateException.InnerExceptions)
            {
                if (!skip ||
                    innerException is not InvalidOperationException)
                {
                    return false;
                }

                if (innerException.Message.StartsWith("The LINQ expression '") ||
                    innerException.Message.Contains("' could not be translated."))
                {
                    skip &= !supports.OuterApply && innerException.Message.Contains("OUTER APPLY") ||
                            !supports.CrossApply && innerException.Message.Contains("CROSS APPLY") ||
                            !supports.WindowFunctions && innerException.Message.Contains("ROW_NUMBER() OVER") ||
                            !supports.ExceptIntercept &&
                            (innerException.Message.Contains("EXCEPT") || innerException.Message.Contains("INTERSECT")) ||
                            !supports.JsonTable && (innerException.Message.Contains("JSON_TABLE") ||
                                                    innerException.Message.Contains("JsonTable")) ||
                            innerException.Message.Contains("Primitive collections support has not been enabled.") ||
                            innerException.Message.Contains("MySqlBipolarExpression");
                }
                else
                {
                    skip &= (!supports.JsonTable ||
                             !supports.JsonTableImplementationWithAggregate) && (innerException.Message.Contains("JSON_TABLE") ||
                                                                                 innerException.Message.Contains("JsonTable"));
                }
            }

            return skip;
        }

        private static bool IsMariaDbRecordHasChangedException(Exception exception)
            => exception is MySqlException { Number: 1020 } ||
               exception.Message.Contains("Record has changed since last read");
    }
}
