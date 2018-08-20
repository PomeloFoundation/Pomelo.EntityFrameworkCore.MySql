using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestModels.Northwind;
using Microsoft.EntityFrameworkCore.TestUtilities;
using MySql.Data.MySqlClient;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public abstract class EscapesMySqlTestBase<TFixture> : IClassFixture<TFixture>, IDisposable where TFixture : NorthwindQueryMySqlFixture<NoopModelCustomizer>
    {
        private readonly NorthwindContext _context;
        protected EscapesMySqlTestBase(TFixture fixture)
        {
            Fixture = fixture;
            _context = CreateContext();
        }

        protected TFixture Fixture;

        protected NorthwindContext CreateContext() => Fixture.CreateContext();

        protected void PerfomInTransaction(Action<NorthwindContext> action)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                action.Invoke(_context);
            }
        }

        public virtual void Input_query_escapes_parameter()
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                var companyName = @"Back\slash's Operation";
                _context.Customers.Add(new Customer
                {
                    CustomerID = "ESCAPETEST",
                    CompanyName = companyName
                });
                _context.SaveChanges();
                var count = _context.Customers.Count(x => x.CompanyName == companyName);
                Assert.Equal(1, count);
            }
        }

        public virtual void Where_query_escapes_literal()
        {
            _context.Customers.Count(x => x.CompanyName == "B's Beverages");
        }

        public virtual void Where_query_escapes_parameter(string companyName)
        {
            _context.Customers.Count(x => x.CompanyName == companyName);
        }

        public virtual void Where_contains_query_escapes(params string[] companyNames)
        {
            _context.Customers.Count(x => companyNames.Contains(x.CompanyName));
        }
        
        protected void SetSqlMode(string modes)
        {
            _context.Database.ExecuteSqlCommand("SET SESSION sql_mode = @p0", new MySqlParameter("@p0", modes) );
        }

        protected void AssertBaseline(params string[] expected)
        {
            Fixture.TestSqlLoggerFactory.AssertBaseline(expected);
        }

        public void Dispose() => _context.Dispose();

    }
}
