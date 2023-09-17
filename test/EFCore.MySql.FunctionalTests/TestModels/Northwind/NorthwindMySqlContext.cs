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
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Employee>(
            b =>
            {
                b.Property(e => e.EmployeeID).HasColumnType("int");
                b.Property(e => e.ReportsTo).HasColumnType("int");
            });

        modelBuilder.Entity<Customer>(
            b =>
            {
                b.Property(e => e.CustomerID).IsFixedLength();
            });

        modelBuilder.Entity<Order>(
            b =>
            {
                b.Property(e => e.CustomerID).IsFixedLength();
                b.Property(e => e.EmployeeID).HasColumnType("int");
                b.Property(o => o.OrderDate).HasColumnType("datetime");
            });

        modelBuilder.Entity<Product>(
            b =>
            {
                b.Property(p => p.UnitsInStock).HasColumnType("smallint");
            });

        modelBuilder.Entity<OrderDetail>(
            b =>
            {
                b.Property(p => p.Quantity).HasColumnType("smallint");
                b.Property(p => p.Discount).HasColumnType("real");
            });
    }
}
