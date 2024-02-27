﻿using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities.Xunit
{
    public class MySqlTestCollectionOrderer : ITestCollectionOrderer
    {
        public IEnumerable<ITestCollection> OrderTestCollections(IEnumerable<ITestCollection> testCollections)
            => testCollections
                .OrderBy(c => c.DisplayName, StringComparer.OrdinalIgnoreCase)
                .ThenBy(c => c.DisplayName, StringComparer.Ordinal)
                .ThenBy(c => c.UniqueID);
    }
}
