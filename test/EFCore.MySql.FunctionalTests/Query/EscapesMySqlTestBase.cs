using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.TestModels.Northwind;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public abstract class EscapesMySqlTestBase<TFixture> : QueryTestBase<TFixture>
        where TFixture : NorthwindQueryFixtureBase<NoopModelCustomizer>, new()
    {
        protected EscapesMySqlTestBase(TFixture fixture)
            : base(fixture)
        {
        }

        protected virtual string Mode => null;

        [ConditionalFact]
        public virtual void Input_query_escapes_parameter()
        {
            ExecuteWithStrategyInTransaction(
                context =>
                {
                    context.Customers.Add(new Customer
                    {
                        CustomerID = "ESCBCKSLINS",
                        CompanyName = @"Back\slash's Insert Operation"
                    });
                    
                    context.SaveChanges();
                },
                context =>
                {
                    var customers = context.Customers.Where(x => x.CustomerID == "ESCBCKSLINS").ToList();
                    Assert.Single(customers);
                    Assert.True(customers[0].CompanyName == @"Back\slash's Insert Operation");
                });
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Where_query_escapes_literal(bool isAsync)
        {
            using (var context = CreateContext())
            {
                var query = context.Set<Customer>().Where(c => c.CompanyName == @"Back\slash's Operation");

                var customers = isAsync
                    ? await query.ToListAsync()
                    : query.ToList();

                Assert.Single(customers);
            }
        }
        
        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Where_query_escapes_parameter(bool isAsync)
        {
            using (var context = CreateContext())
            {
                var companyName = @"Back\slash's Operation";

                var query = context.Set<Customer>().Where(c => c.CompanyName == companyName);

                var customers = isAsync
                    ? await query.ToListAsync()
                    : query.ToList();

                Assert.Single(customers);
            }
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Where_contains_query_escapes(bool isAsync)
        {
            using (var context = CreateContext())
            {
                var companyNames = new[]
                {
                    @"Back\slash's Operation",
                    @"B's Beverages"
                };

                var query = context.Set<Customer>().Where(c => companyNames.Contains(c.CompanyName));

                var customers = isAsync
                    ? await query.ToListAsync()
                    : query.ToList();

                Assert.Equal(2, customers.Count);
            }
        }

        protected NorthwindContext CreateContext() => Fixture.CreateContext();

        protected virtual void ExecuteWithStrategyInTransaction(
            Action<NorthwindContext> testOperation,
            Action<NorthwindContext> nestedTestOperation1 = null,
            Action<NorthwindContext> nestedTestOperation2 = null)
        {
            TestHelpers.ExecuteWithStrategyInTransaction(
                CreateContext,
                UseTransaction,
                testOperation,
                nestedTestOperation1,
                nestedTestOperation2);
        }

        protected virtual void UseTransaction(DatabaseFacade facade, IDbContextTransaction transaction)
            => facade.UseTransaction(transaction.GetDbTransaction());
    }
}
