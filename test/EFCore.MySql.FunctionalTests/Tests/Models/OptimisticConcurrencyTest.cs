using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Models;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Tests.Models
{
    public class OptimisticConcurrencyTests
    {
        private static readonly MySqlConnection Connection = new MySqlConnection(AppConfig.Config["Data:ConnectionString"]);

        [Fact]
        public async Task DbUpdateConcurrencyExceptionThrownWhenConflictDetected()
        {
            using (var scope = new AppDbScope(Connection))
            {
                var category = Guid.NewGuid().ToString();
                await scope.AppDb.Database.ExecuteSqlCommandAsync("INSERT INTO Invoices (Category, Id, Name, Description) VALUES ({0}, {1}, {2}, {3})", category, 1, "name", "description");

                var invoice = await scope.AppDb.Invoices.SingleAsync(x => x.Category == category && x.Id == 1);

                await scope.AppDb.Database.ExecuteSqlCommandAsync("UPDATE Invoices SET Name = {0} WHERE Category = {1} AND Id = {2}", "changed", invoice.Category, invoice.Id);

                invoice.Description = "new description";
                invoice.Items.AddRange(new[]
                {
                    new InvoiceItem
                    {
                        Category = invoice.Category,
                        InvoiceId = invoice.Id,
                        Description = "item1"
                    },
                    new InvoiceItem
                    {
                        Category = invoice.Category,
                        InvoiceId = invoice.Id,
                        Description = "item2"
                    },
                    new InvoiceItem
                    {
                        Category = invoice.Category,
                        InvoiceId = invoice.Id,
                        Description = "item3"
                    }
                });
                await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => scope.AppDb.SaveChangesAsync());
            }
        }

        [Fact]
        public async Task DbUpdateConcurrencyExceptionNotThrownWhenNoConflictDetected()
        {
            var invoice = new Invoice { Category = Guid.NewGuid().ToString(), Id = 1, Name = "name" };
            using (var scope = new AppDbScope(Connection))
            {
                scope.AppDb.Invoices.Add(invoice);
                await scope.AppDb.SaveChangesAsync();

                invoice.Name = "new name";
                await scope.AppDb.SaveChangesAsync();
            }
        }
    }
}
