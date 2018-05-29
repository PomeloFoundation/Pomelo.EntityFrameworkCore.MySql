using System.Collections.Generic;
using System.Threading.Tasks;
using Pomelo.EntityFrameworkCore.MySql.IntegrationTests.Models;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.IntegrationTests.Tests.Models
{
    public class BatchTest
    {
        [Fact]
        public async Task BatchInsertWorksWithMixedNoServerData()
        {
            using (var scope = new AppDbScope())
            {
                var db = scope.AppDb;

                var newCatgory = new Category()
                {
                    Name = "test"
                };

                var products = new List<Product>();

                for (var i = 0; i < 10; ++i)
                {
                    var product = new Product()
                    {
                        Name = $"Product {i}",
                    };

                    product.ProductCategories.Add(new ProductCategory()
                    {
                        Category = newCatgory
                    });

                    if (i % 2 == 1)
                    {
                        product.ParentProduct = products[i - 1];
                    }

                    products.Add(product);
                }

                db.Products.AddRange(products);

                var resultCount = await db.SaveChangesAsync();

                Assert.Equal(21, resultCount);
            }
        }
    }
}
