using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.Migrations
{
    public sealed class MySqlMigrationTest : TestBase<MySqlMigrationTest.Context>
    {
        public static class Models
        {
            public static class IceCreamBrandModel
            {
                public static ModelBuilder GetModel(ModelBuilder modelBuilder)
                {
                    modelBuilder.Entity<IceCream>();
                    modelBuilder.Entity<IceCreamBrand>(
                        entity =>
                        {
                            entity.HasAlternateKey(b => b.InternalName);

                            entity.Property(e => e.InternalName)
                                .HasMaxLength(255)
                                .IsRequired();
                        });

                    return modelBuilder;
                }

                public class IceCream
                {
                    public int IceCreamId { get; set; }
                    public string Name { get; set; }
                    public int IceCreamBrandId { get; set; }

                    public IceCreamBrand Brand { get; set; }
                }

                public class IceCreamBrand
                {
                    public int IceCreamBrandId { get; set; }
                    public string Name { get; set; }
                    public string InternalName { get; set; }

                    public ICollection<IceCream> IceCreams { get; set; } = new HashSet<IceCream>();
                }
            }
        }

        public class Context : ContextBase
        {
        }
    }
}
