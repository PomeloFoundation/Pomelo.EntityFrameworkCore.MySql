using Xunit;

//
// Optional: Control the test execution order.
//           This can be helpful for diffing etc.
//

#if FIXED_TEST_ORDER

[assembly: CollectionBehavior(DisableTestParallelization = true)]
[assembly: TestCollectionOrderer("Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities.Xunit.MySqlTestCollectionOrderer", "Pomelo.EntityFrameworkCore.MySql.FunctionalTests")]
[assembly: TestCaseOrderer("Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities.Xunit.MySqlTestCaseOrderer", "Pomelo.EntityFrameworkCore.MySql.FunctionalTests")]

#endif
