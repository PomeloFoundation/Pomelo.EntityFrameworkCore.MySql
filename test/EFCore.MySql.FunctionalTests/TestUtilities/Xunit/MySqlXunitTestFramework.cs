using System;
using System.Reflection;
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

        protected override ITestFrameworkExecutor CreateExecutor(AssemblyName assemblyName)
            => new MySqlXunitTestFrameworkExecutor(assemblyName, SourceInformationProvider, DiagnosticMessageSink);
    }
}
