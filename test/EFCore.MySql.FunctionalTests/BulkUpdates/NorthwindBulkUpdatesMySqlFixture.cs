using System;
using Microsoft.EntityFrameworkCore.BulkUpdates;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microting.EntityFrameworkCore.MySql.FunctionalTests.TestModels.Northwind;
using Microting.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;

namespace Microting.EntityFrameworkCore.MySql.FunctionalTests.BulkUpdates;

public class NorthwindBulkUpdatesMySqlFixture<TModelCustomizer> : NorthwindBulkUpdatesRelationalFixture<TModelCustomizer>
    where TModelCustomizer : ITestModelCustomizer, new()
{
    protected override ITestStoreFactory TestStoreFactory
        => MySqlNorthwindTestStoreFactory.Instance;

    protected override Type ContextType
        => typeof(NorthwindMySqlContext);
}
