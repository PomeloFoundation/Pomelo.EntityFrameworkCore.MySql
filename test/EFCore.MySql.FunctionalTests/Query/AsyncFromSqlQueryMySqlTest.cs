using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestModels.Northwind;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class AsyncFromSqlQueryMySqlTest : AsyncFromSqlQueryTestBase<NorthwindQueryMySqlFixture<NoopModelCustomizer>>
    {
        public AsyncFromSqlQueryMySqlTest(NorthwindQueryMySqlFixture<NoopModelCustomizer> fixture)
            : base(fixture)
        {
        }
    }
}
