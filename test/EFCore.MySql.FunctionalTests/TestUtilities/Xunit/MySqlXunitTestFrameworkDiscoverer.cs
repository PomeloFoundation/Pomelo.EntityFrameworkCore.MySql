using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.TestUtilities.Xunit;
using Pomelo.EntityFrameworkCore.MySql.Tests.TestUtilities.Attributes;
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
}
