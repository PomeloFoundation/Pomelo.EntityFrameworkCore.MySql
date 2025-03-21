﻿using System;
using Microsoft.EntityFrameworkCore.BulkUpdates;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestModels.Northwind;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.BulkUpdates;

public class NorthwindBulkUpdatesMySqlFixture<TModelCustomizer> : NorthwindBulkUpdatesRelationalFixture<TModelCustomizer>
    where TModelCustomizer : ITestModelCustomizer, new()
{
    protected override ITestStoreFactory TestStoreFactory
        => MySqlNorthwindTestStoreFactory.Instance;

    protected override Type ContextType
        => typeof(NorthwindMySqlContext);
}
