using Xunit;

//
// Optional: Control the test execution order.
//           This can be helpful for diffing etc.
//

#if FIXED_TEST_ORDER || SPECIFIC_TEST_ORDER

[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly, DisableTestParallelization = true, MaxParallelThreads = 1)]
[assembly: TestCaseOrderer("Microting.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities.Xunit.MySqlTestCaseOrderer", "Microting.EntityFrameworkCore.MySql.FunctionalTests")]
[assembly: TestCollectionOrderer("Microting.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities.Xunit.MySqlTestCollectionOrderer", "Microting.EntityFrameworkCore.MySql.FunctionalTests")]

#endif

// Our custom MySqlXunitTestFrameworkDiscoverer class allows filtering whole classes like SupportedServerVersionConditionAttribute, instead
// of just the test cases. This is necessary, if a fixture is database server version dependent.
[assembly: TestFramework("Microting.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities.Xunit.MySqlXunitTestFramework", "Microting.EntityFrameworkCore.MySql.FunctionalTests")]
