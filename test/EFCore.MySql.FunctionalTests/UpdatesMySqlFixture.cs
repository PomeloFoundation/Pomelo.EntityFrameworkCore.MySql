using System;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
{
    public class UpdatesMySqlFixture : UpdatesRelationalFixture
    {
        protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;

        protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
        {
            base.OnModelCreating(modelBuilder, context);

            Models.Issue1300.Setup(modelBuilder, context);
        }

        public static class Models
        {
            public static class Issue1300
            {
                public static void Setup(ModelBuilder modelBuilder, DbContext context)
                {
                    modelBuilder.Entity<Flavor>(
                        entity =>
                        {
                            entity.HasKey(e => new {e.FlavorId, e.DiscoveryDate});
                            entity.Property(e => e.FlavorId)
                                .ValueGeneratedOnAdd();
                            entity.Property(e => e.DiscoveryDate)
                                .ValueGeneratedOnAdd();
                        });
                }

                public class Flavor
                {
                    public int FlavorId { get; set; }
                    public DateTime DiscoveryDate { get; set; }
                }
            }
        }
    }
}
