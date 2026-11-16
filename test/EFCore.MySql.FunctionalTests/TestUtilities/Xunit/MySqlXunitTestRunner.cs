using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microting.EntityFrameworkCore.MySql.Tests;
using Xunit;
using Xunit.v3;

namespace Microting.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities.Xunit
{
    /// <summary>
    /// Replaces the default xUnit.net v3 test runner, to report tests as 'Skipped' instead of 'Failed', if they failed because they use an
    /// expression that is not supported by the underlying database server version (or by an explicitly disabled provider feature).
    /// </summary>
    /// <remarks>
    /// Under xUnit.net v2, this used to be implemented by replacing the whole test framework (discoverer, executor and all runners). In
    /// xUnit.net v3, <see cref="XunitTestRunner.Instance"/> is the single extensibility point used by
    /// <c>XunitTestCaseRunnerBaseContext.RunTest()</c>, so swapping that instance is all that is needed.
    /// </remarks>
    public class MySqlXunitTestRunner : XunitTestRunner
    {
#pragma warning disable CA2255 // The 'ModuleInitializer' attribute should not be used in libraries
        [ModuleInitializer]
        internal static void Install()
            => Instance = new MySqlXunitTestRunner();
#pragma warning restore CA2255

        protected override ValueTask<(bool Continue, TestResultState ResultState)> OnTestFailed(
            XunitTestRunnerContext ctxt,
            Exception exception,
            decimal executionTime,
            string output,
            string[] warnings)
            => SkipFailedTest(exception)
                ? OnTestSkipped(ctxt, exception.Message, executionTime, output, warnings)
                : base.OnTestFailed(ctxt, exception, executionTime, output, warnings);

        /// <summary>
        /// Mark failed tests as 'Skipped', if they failed because they use an expression, that is not supported by the underlying database
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
    }
}
