using System.Linq;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.TestModels.Northwind;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class NorthwindQueryMySqlFixture<TModelCustomizer> : NorthwindQueryRelationalFixture<TModelCustomizer>
        where TModelCustomizer : IModelCustomizer, new()
    {
        // TODO: Consider using an existing database/initialize from script
        protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;

        protected override bool ShouldLogCategory(string logCategory)
            => logCategory == DbLoggerCategory.Query.Name || logCategory == DbLoggerCategory.Database.Command.Name;

        public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
            => base.AddOptions(builder)
                .EnableDetailedErrors();

        protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
        {
            base.OnModelCreating(modelBuilder, context);

            var northwindContext = (NorthwindRelationalContext)context;
            modelBuilder
                .Entity<OrderQuery>()
                .HasNoKey()
                .ToQuery(() => northwindContext.Orders
                    .FromSqlRaw("select * from `Orders`")
                    .Select(
                        o => new OrderQuery
                        {
                            CustomerID = o.CustomerID
                        }));

            modelBuilder
                .Entity<CustomerView>()
                .HasNoKey()
                .ToQuery(() => northwindContext.CustomerQueries
                    .FromSqlInterpolated($"SELECT `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region` FROM `Customers` AS `c`"));
        }

        protected override void Seed(NorthwindContext context)
        {
            base.Seed(context);

            context.Database.ExecuteSqlRaw(@"DROP PROCEDURE IF EXISTS `Ten Most Expensive Products`;
CREATE PROCEDURE `Ten Most Expensive Products` ()
BEGIN
  SELECT `ProductName` AS `TenMostExpensiveProducts`, `UnitPrice`
  FROM `Products`
  ORDER BY `UnitPrice` DESC
  LIMIT 10;
END;");
            context.Database.ExecuteSqlRaw(@"DROP PROCEDURE IF EXISTS `CustOrderHist`;
CREATE PROCEDURE `CustOrderHist` (IN CustomerID VARCHAR(768))
BEGIN
  SELECT `ProductName`, SUM(`Quantity`) AS `Total`
  FROM `Products` `p`, `Order Details` `od`, `Orders` `o`, `Customers` `c`
  WHERE `c`.`CustomerId` = `CustomerId`
  AND `c`.`CustomerId` = `o`.`CustomerId`
  AND `o`.`OrderId` = `od`.`OrderId`
  AND `od`.`ProductId` = `p`.`ProductId`
  GROUP BY `ProductName`;
END;");
            context.Database.ExecuteSqlRaw(@"drop view if exists `Alphabetical list of products`;
create view `Alphabetical list of products` AS
SELECT `p`.`ProductId`, `p`.`ProductName`, 'Food' as `CategoryName`
FROM `Products` `p`
WHERE `p`.`Discontinued` = 0");
        }
    }
}
