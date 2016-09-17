using Microsoft.EntityFrameworkCore;
using MySQL.Data.EntityFrameworkCore.Extensions;
using Pomelo.EntityFrameworkCore.MySql.Functional.Models;

namespace Pomelo.EntityFrameworkCore.MySql.Functional
{
    public class AppDb : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<DataTypes> DataTypes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (AppConfig.EfProvider == "oracle")
            {
                // Oracle defines this with a case sensitive "MySQL" in MySQL.Data.EntityFrameworkCore.Extensions
                optionsBuilder.UseMySQL(AppConfig.Config["Data:ConnectionString"] + "ssl mode=none;");
            }
            else
            {
                // Pomelo defines this with a case sensitive "MySql" in Microsoft.EntityFrameworkCore
                optionsBuilder.UseMySql(AppConfig.Config["Data:ConnectionString"]);
            }
        }
    }
}
