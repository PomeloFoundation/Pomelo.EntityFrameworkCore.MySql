using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestModels.Northwind;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestModels.Northwind;

public class NorthwindMySqlContext : NorthwindRelationalContext
{
    public NorthwindMySqlContext(DbContextOptions options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>().ToTable("Customers");
        modelBuilder.Entity<Employee>().ToTable("Employees");
        modelBuilder.Entity<Product>().ToTable("Products");
        modelBuilder.Entity<Order>().ToTable("Orders");
        modelBuilder.Entity<OrderDetail>().ToTable("OrderDetails");

        modelBuilder.Entity<CustomerOrderHistory>().HasKey(coh => coh.ProductName);
        modelBuilder.Entity<MostExpensiveProduct>().HasKey(mep => mep.TenMostExpensiveProducts);

        modelBuilder.Entity<CustomerQuery>().ToSqlQuery(
            "SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region` FROM `Customers` AS `c`");

        modelBuilder.Entity<OrderQuery>().ToSqlQuery(@"select * from `Orders`");
        modelBuilder.Entity<ProductView>().ToView("Alphabetical list of products");
    }
}
