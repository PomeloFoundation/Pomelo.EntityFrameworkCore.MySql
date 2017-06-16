using System;
using System.Data.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using Pomelo.EntityFrameworkCore.MySql.PerfTests.Models;
#if ORACLE
using MySQL.Data.EntityFrameworkCore.Extensions;
#elif SAPIENT
using MySQL.Data.Entity.Extensions;
#endif

namespace Pomelo.EntityFrameworkCore.MySql.PerfTests
{
	public class AppDb : IdentityDbContext<AppIdentityUser>//, IDesignTimeDbContextFactory<AppDb>
	{
		// blog
		public DbSet<Blog> Blogs { get; set; }

#if POMELO
		// crm
		public DbSet<CrmAdmin> CrmAdmins { get; set; }
		public DbSet<CrmRole> CrmRoles { get; set; }
		public DbSet<CrmMenu> CrmMenus { get; set; }

		// data types
		public DbSet<DataTypesSimple> DataTypesSimple { get; set; }
		public DbSet<DataTypesVariable> DataTypesVariable { get; set; }

		// generated data types
		public DbSet<GeneratedContact> GeneratedContacts { get; set; }
		public DbSet<GeneratedTime> GeneratedTime { get; set; }

		// people
		public DbSet<Person> People { get; set; }
		public DbSet<PersonTeacher> PeopleTeachers { get; set; }
		public DbSet<PersonKid> PeopleKids { get; set; }
		public DbSet<PersonParent> PeopleParents { get; set; }
		public DbSet<PersonFamily> PeopleFamilies { get; set; }
#endif

		private readonly bool _configured;
	    private readonly DbConnection _connection;

		public AppDb()
		{
			_configured = false;
		}

		public AppDb(DbContextOptions options) : base(options)
		{
			_configured = true;
		}

	    public AppDb(DbConnection connection)
	    {
	        _configured = false;
	        _connection = connection;
	    }

		// AppDb IDesignTimeDbContextFactory<AppDb>.CreateDbContext(string[] args)
		// {
		// 	var optionsBuilder = new DbContextOptionsBuilder<AppDb>()
		// 		.UseMySql(AppConfig.Config["Data:ConnectionString"]);
		// 	new MySqlDbContextOptionsBuilder(optionsBuilder).MaxBatchSize(AppConfig.EfBatchSize);
		// 	return new AppDb(optionsBuilder.Options);
		// }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
		    if (_configured)
		        return;

#if POMELO
		    if (_connection != null)
                optionsBuilder.UseMySql(_connection, options => options.MaxBatchSize(AppConfig.EfBatchSize));
		    else
				optionsBuilder.UseMySql(AppConfig.Config["Data:ConnectionString"], options => options.MaxBatchSize(AppConfig.EfBatchSize));
#else
            if (_connection != null)
                optionsBuilder.UseMySQL(_connection, options => options.MaxBatchSize(AppConfig.EfBatchSize));
		    else
		        optionsBuilder.UseMySQL(AppConfig.Config["Data:ConnectionString"], options => options.MaxBatchSize(AppConfig.EfBatchSize));
#endif

		    optionsBuilder.UseLoggerFactory(new LoggerFactory().AddConsole(AppConfig.Config.GetSection("Logging")));
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// Shorten key length for Identity
			modelBuilder.Entity<AppIdentityUser>(entity => {
				entity.Property(m => m.Email).HasMaxLength(127);
				entity.Property(m => m.NormalizedEmail).HasMaxLength(127);
				entity.Property(m => m.NormalizedUserName).HasMaxLength(127);
				entity.Property(m => m.UserName).HasMaxLength(127);
			});
			modelBuilder.Entity<IdentityRole>(entity => {
				entity.Property(m => m.Name).HasMaxLength(127);
				entity.Property(m => m.NormalizedName).HasMaxLength(127);
			});
			modelBuilder.Entity<IdentityUserLogin<string>>(entity =>
			{
				entity.Property(m => m.LoginProvider).HasMaxLength(127);
				entity.Property(m => m.ProviderKey).HasMaxLength(127);
			});
			modelBuilder.Entity<IdentityUserRole<string>>(entity =>
			{
				entity.Property(m => m.UserId).HasMaxLength(127);
				entity.Property(m => m.RoleId).HasMaxLength(127);
			});
			modelBuilder.Entity<IdentityUserToken<string>>(entity =>
			{
				entity.Property(m => m.UserId).HasMaxLength(127);
				entity.Property(m => m.LoginProvider).HasMaxLength(127);
				entity.Property(m => m.Name).HasMaxLength(127);

			});

#if POMELO
			// Add our models fluent APIs
			CrmMeta.OnModelCreating(modelBuilder);
			GeneratedContactMeta.OnModelCreating(modelBuilder);
			PeopleMeta.OnModelCreating(modelBuilder);
#endif

		    if (AppConfig.EfSchema != null)
		    {
                // Generates all models in a different schema
		        modelBuilder.HasDefaultSchema(AppConfig.EfSchema);
		    }

		}
	}
}
