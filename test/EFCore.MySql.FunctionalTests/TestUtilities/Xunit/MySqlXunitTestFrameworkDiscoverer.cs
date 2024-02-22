using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.TestUtilities.Xunit;
using Pomelo.EntityFrameworkCore.MySql.Tests.TestUtilities.Attributes;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities.Xunit
{
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

            DiscovererTypeCache.Add(typeof(SkippableFactAttribute), typeof(SkippableFactDiscoverer));
            DiscovererTypeCache.Add(typeof(SkippableTheoryAttribute), typeof(SkippableTheoryDiscoverer));
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

        protected override bool FindTestsForMethod(
            ITestMethod testMethod,
            bool includeSourceInformation,
            IMessageBus messageBus,
            ITestFrameworkDiscoveryOptions discoveryOptions)
            => base.FindTestsForMethod(
                testMethod,
                includeSourceInformation,
                new FindTestsForMethodMessageBus(messageBus),
                discoveryOptions);

        private class FindTestsForMethodMessageBus : IMessageBus
        {
            private readonly IMessageBus _messageBus;
            private static readonly HashSet<string> _testCaseDisplayNamesInOrder = MySqlTestCaseOrderer.SpecificTestCaseDisplayNamesInOrder.ToHashSet();

            public FindTestsForMethodMessageBus(IMessageBus messageBus)
                => _messageBus = messageBus;

            public void Dispose()
                => _messageBus.Dispose();

            public bool QueueMessage(IMessageSinkMessage message)
            {
                // Intercept TestCaseDiscoveryMessage messages to filter specific test cases independent of the discoverer, so we can filter
                // all test cases and not just the ones for which we specify our own discoverers.
                if (_testCaseDisplayNamesInOrder.Count > 0 &&
                    message is TestCaseDiscoveryMessage testCaseDiscoveryMessage)
                {
                    var displayName = GetFullyQualifiedDisplayName(testCaseDiscoveryMessage.TestCase);

                    if (!_testCaseDisplayNamesInOrder.Contains(displayName))
                    {
                        return true;
                    }
                }

                return _messageBus.QueueMessage(message);
            }

            private static string GetFullyQualifiedDisplayName(ITestCase testCase)
            {
                var className = testCase.TestMethod.TestClass.Class.Name;
                var displayName = testCase.DisplayName;

                return !displayName.StartsWith(className)
                    ? $"{className}.{displayName}"
                    : displayName;
            }
        }
    }
}
