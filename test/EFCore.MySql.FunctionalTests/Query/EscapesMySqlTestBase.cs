using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.TestModels.Northwind;
using Microsoft.EntityFrameworkCore.TestUtilities;
using MySql.Data.MySqlClient;
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
                    Assert.Equal(1, customers.Count);
                    Assert.True(customers[0].CompanyName == @"Back\slash's Insert Operation");
                });
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Where_query_escapes_literal(bool isAsync)
        {
            using (var context = CreateContext())
            {
                /*
                var customerParameterExpression = Expression.Parameter(typeof(Customer), "c");
                var predicateExpression = Expression.Lambda<Func<Customer, bool>>(
                    Expression.Equal(
                        Expression.Property(
                            customerParameterExpression,
                            nameof(Customer.CompanyName)),
                        Expression.Constant(@"Back\slash's Insert Operation")),
                    customerParameterExpression);
                
                var query = context.Set<Customer>().Where(predicateExpression);
                */

                var query = context.Set<Customer>().Where(c => c.CompanyName == @"Back\slash's Operation");

                var customers = isAsync
                    ? await query.ToListAsync()
                    : query.ToList();

                Assert.Equal(1, customers.Count);
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

                Assert.Equal(1, customers.Count);
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
        
        protected void AddSqlMode(DbContext context, string mode)
        {
            context.Database.ExecuteSqlRaw("SET SESSION sql_mode = CONCAT(@@sql_mode, ',', @p0)", new MySqlParameter("@p0", mode));
        }

        protected NorthwindContext CreateContext() => CreateContext(Mode);

        protected NorthwindContext CreateContext(string mode)
        {
            var context = Fixture.CreateContext();

            if (mode != null)
            {
                AddSqlMode(context, mode);
            }

            return context;
        }

        protected virtual void ExecuteWithStrategyInTransaction(
            Action<NorthwindContext> testOperation,
            Action<NorthwindContext> nestedTestOperation1 = null,
            Action<NorthwindContext> nestedTestOperation2 = null)
        {
            // Defer setting sql_mode to the point, where we enlisted in the current transaction.
            void SetModeAndCallOperation(NorthwindContext context, Action<NorthwindContext> operation)
            {
                if (Mode != null)
                {
                    AddSqlMode(context, Mode);
                }

                operation?.Invoke(context);
            }

            TestHelpers.ExecuteWithStrategyInTransaction(
                () => CreateContext(null),
                UseTransaction,
                c => SetModeAndCallOperation(c, testOperation),
                c => SetModeAndCallOperation(c, nestedTestOperation1),
                c => SetModeAndCallOperation(c, nestedTestOperation2));
        }

        protected virtual void UseTransaction(DatabaseFacade facade, IDbContextTransaction transaction)
            => facade.UseTransaction(transaction.GetDbTransaction());
    }
}
