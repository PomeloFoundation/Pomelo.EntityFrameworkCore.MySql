using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Models
{

	public static class CrmMeta
	{
		public static void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<CrmAdmin>(entity =>
			{
				entity.Property(m => m.Username)
					.HasMaxLength(10);

				entity.Property(m => m.Password)
					.HasMaxLength(20);
			});

			modelBuilder.Entity<CrmAdminRole>(entity =>
			{
				entity.HasKey(am => new {am.AdminId, am.RoleId});

				entity.HasOne(am => am.Admin)
					.WithMany(a => a.AdminRoles)
					.HasForeignKey(am => am.AdminId);

				entity.HasOne(am => am.Role)
					.WithMany(m => m.AdminRoles)
					.HasForeignKey(am => am.RoleId);
			});

			modelBuilder.Entity<CrmAdminMenu>(entity =>
			{
				entity.HasKey(am => new {am.AdminId, am.MenuId});

				entity.HasOne(am => am.Admin)
					.WithMany(a => a.AdminMenus)
					.HasForeignKey(am => am.AdminId);

				entity.HasOne(am => am.Menu)
					.WithMany(m => m.AdminMenus)
					.HasForeignKey(am => am.MenuId);
			});

		}
	}

	public class CrmAdmin
	{
		public int Id { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public List<CrmAdminRole> AdminRoles { get; set; }
		public List<CrmAdminMenu> AdminMenus { get; set; }
	}

	public class CrmRole
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public List<CrmAdminRole> AdminRoles { get; set; }
	}

	public class CrmMenu
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public List<CrmAdminMenu> AdminMenus { get; set; }
	}

	public class CrmAdminRole
	{
		public int AdminId { get; set; }
		public int RoleId { get; set; }
		public CrmAdmin Admin { get; set; }
		public CrmRole Role { get; set; }
	}

	public class CrmAdminMenu
	{
		public int AdminId { get; set; }
		public int MenuId { get; set; }
		public CrmAdmin Admin { get; set; }
		public CrmMenu Menu { get; set; }
	}
}
