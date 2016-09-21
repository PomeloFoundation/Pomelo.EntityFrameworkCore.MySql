using Microsoft.EntityFrameworkCore;
using MySQL.Data.EntityFrameworkCore.Extensions;
using Pomelo.EntityFrameworkCore.MySql.PerfTests.Models;

namespace Pomelo.EntityFrameworkCore.MySql.PerfTests
{
	public class AppDb : DbContext
	{
		// blog
		public DbSet<Blog> Blogs { get; set; }

		// crm
		public DbSet<CrmAdmin> CrmAdmins { get; set; }
		public DbSet<CrmRole> CrmRoles { get; set; }
		public DbSet<CrmMenu> CrmMenus { get; set; }

		// data types
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

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			CrmMeta.OnModelCreating(modelBuilder);
		}

	}
}
