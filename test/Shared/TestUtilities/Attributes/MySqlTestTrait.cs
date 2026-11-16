using System.Collections.Generic;

namespace Microting.EntityFrameworkCore.MySql.Tests.TestUtilities.Attributes
{
    /// <summary>
    /// Helpers for xUnit v3 trait based test filtering.
    /// </summary>
    /// <remarks>
    /// EF Core 11 moved to xUnit v3 and removed its `ITestCondition` infrastructure, which used to allow a custom test
    /// framework discoverer to evaluate conditions and skip tests (and whole test classes).
    /// The replacement used by EF Core and other providers is to add a `category=failing` trait to tests whose condition
    /// is not met, and to let the test runner exclude that category. Unlike xUnit v3's built-in
    /// `Skip`/`SkipUnless`/`SkipWhen` support, traits are also applied to test classes and to overridden base class test
    /// methods, which is what the server version conditions in this repository rely on.
    /// </remarks>
    public static class MySqlTestTrait
    {
        /// <summary>
        /// The trait name that the test runner filters on. Must be kept in sync with the `--filter` arguments used by
        /// the CI workflows.
        /// </summary>
        public const string CategoryTraitName = "category";

        /// <summary>
        /// The trait value that marks a test as excluded.
        /// </summary>
        public const string FailingTraitValue = "failing";

        private static readonly KeyValuePair<string, string>[] _failing = [new KeyValuePair<string, string>(CategoryTraitName, FailingTraitValue)];

        /// <summary>
        /// Returns the `category=failing` trait when <paramref name="isMet"/> is `false`, so the runner filters the test
        /// out, and no traits at all when the condition is met.
        /// </summary>
        public static IReadOnlyCollection<KeyValuePair<string, string>> ForCondition(bool isMet)
            => isMet
                ? []
                : _failing;
    }
}
