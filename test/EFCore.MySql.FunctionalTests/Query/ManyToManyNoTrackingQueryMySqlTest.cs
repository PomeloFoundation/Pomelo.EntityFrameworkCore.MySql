﻿using Microsoft.EntityFrameworkCore.Query;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class ManyToManyNoTrackingQueryMySqlTest
        : ManyToManyNoTrackingQueryRelationalTestBase<ManyToManyQueryMySqlFixture>
    {
        public ManyToManyNoTrackingQueryMySqlTest(ManyToManyQueryMySqlFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        // TODO:
        // protected override bool CanExecuteQueryString => true;
    }
}
