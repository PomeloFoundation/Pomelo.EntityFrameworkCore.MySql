using System;
using System.Data.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Models;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
{
	public class AppDb : IdentityDbContext<AppIdentityUser>
	{
		// blog
		public DbSet<Blog> Blogs { get; set; }

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
		public DbSet<GeneratedConcurrencyToken> GeneratedConcurrencyToken { get; set; }

		// people
		public DbSet<Person> People { get; set; }
		public DbSet<PersonTeacher> PeopleTeachers { get; set; }
		public DbSet<PersonKid> PeopleKids { get; set; }
		public DbSet<PersonParent> PeopleParents { get; set; }
		public DbSet<PersonFamily> PeopleFamilies { get; set; }
		
		public AppDb(DbContextOptions options) : base(options)
		{
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

			// Add our models fluent APIs
			CrmMeta.OnModelCreating(modelBuilder);
			GeneratedContactMeta.OnModelCreating(modelBuilder);
			PeopleMeta.OnModelCreating(modelBuilder);

		    if (AppConfig.EfSchema != null)
		    {
                // Generates all models in a different schema
		        modelBuilder.HasDefaultSchema(AppConfig.EfSchema);
		    }

		}
	}
}
