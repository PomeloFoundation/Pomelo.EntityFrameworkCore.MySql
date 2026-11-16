// #define FIXED_TEST_ORDER

using Xunit;

//
// Optional: Control the test execution order.
//           This can be helpful for diffing etc.
//
// The custom test case/collection orderers that used to be referenced here were built on the xUnit v2 extensibility
// model, which no longer exists in xUnit v3, so they were removed. Disabling test parallelization still gives a stable
// (though not customizable) order.
//

#if FIXED_TEST_ORDER

[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly, DisableTestParallelization = true, MaxParallelThreads = 1)]

#endif
