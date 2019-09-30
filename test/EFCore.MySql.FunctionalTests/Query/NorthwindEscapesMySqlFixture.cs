using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.TestModels.Northwind;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class NorthwindEscapesMySqlFixture<TModelCustomizer> : NorthwindQueryMySqlFixture<TModelCustomizer>
        where TModelCustomizer : IModelCustomizer, new()
    {
        protected override string StoreName => base.StoreName + "Escapes";

        protected override void Seed(NorthwindContext context)
        {
            context.Set<Customer>()
                .Add(new Customer
                {
                    CustomerID = "BCKSL",
                    CompanyName = @"Back\slash's Operation",
                    ContactName = "Back Slasher",
                    ContactTitle = "Back Slasher",
                    Address = "Hintere Str. 58",
                    City = "Stadt Brandenburg",
                    Region = null,
                    PostalCode = "14772",
                    Country = "Germany",
                    Phone = "030-1074521",
                    Fax = "030-2079545"
                });

            base.Seed(context);
        }
    }
}
