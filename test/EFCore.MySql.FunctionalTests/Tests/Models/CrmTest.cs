using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Models;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Tests.Models
{
	public class CrmFixture
	{
		public CrmFixture()
		{
			using (var scope = new AppDbScope())
			{
				var db = scope.AppDb;
				var superAdmin = new CrmAdmin
				{
					Username = "test",
					Password = "test"
				};
				var menus = new List<CrmMenu>
				{
					new CrmMenu {Name = "Dashboard"},
					new CrmMenu {Name = "Posts"},
					new CrmMenu {Name = "Media"},
					new CrmMenu {Name = "Comments"},
					new CrmMenu {Name = "Themes"}
				};
				var roles = new List<CrmRole>
				{
					new CrmRole {Name = "Viewer"},
					new CrmRole {Name = "Author"},
					new CrmRole {Name = "Editor"},
					new CrmRole {Name = "Admin"},
				};
				db.CrmMenus.AddRange(menus);
				db.CrmRoles.AddRange(roles);

				superAdmin.AdminMenus = new List<CrmAdminMenu>();
				foreach (var menu in menus)
				{
					superAdmin.AdminMenus.Add(new CrmAdminMenu{ Menu = menu });
				}

				superAdmin.AdminRoles = new List<CrmAdminRole>();
				foreach (var role in roles)
				{
					superAdmin.AdminRoles.Add(new CrmAdminRole{ Role = role });
				}
				db.CrmAdmins.Add(superAdmin);

				db.SaveChanges();
			}
		}
	}

	public class CrmTest : IClassFixture<CrmFixture>
	{
		[Fact]
		public async Task TestCrmSuperUserEagerLoading()
		{
			using (var scope = new AppDbScope())
			{
				var db = scope.AppDb;
				var superUser = await db.CrmAdmins
					.OrderByDescending(m => m.Id)
					.Include(m => m.AdminMenus)
						.ThenInclude(m => m.Menu)
					.Include(m => m.AdminRoles)
						.ThenInclude(m => m.Role)
					.FirstOrDefaultAsync();

				Assert.NotNull(superUser);
				Assert.Equal("test", superUser.Username);
				Assert.Equal("test", superUser.Password);
				Assert.Equal(5, superUser.AdminMenus.Count);
				Assert.NotNull(superUser.AdminMenus[0].Menu);
				Assert.Equal(4, superUser.AdminRoles.Count);
				Assert.NotNull(superUser.AdminRoles[0].Role);
			}
		}

		[Fact]
		public async Task TestCrmSuperUserExplicitLoading()
		{
			using (var scope = new AppDbScope())
			{
				var db = scope.AppDb;
				// load just the superUser
				var superUser = await db.CrmAdmins.OrderByDescending(m => m.Id).FirstOrDefaultAsync();

				// explicit load AdminMenus
				await db.Entry(superUser).Collection(m => m.AdminMenus).LoadAsync();

				// explicit load each Menu
// this code is creating a race condition and hanging somewhere
// probably an upstream bug in EntityFramework, possibly in MySqlConnector
// needs further investigation.  this is a poor way to use Explicit Loading anyways though
//				var tasks = new List<Task>();
//				foreach (var adminMenu in superUser.AdminMenus)
//					tasks.Add(db.Entry(adminMenu).Reference(m => m.Menu).LoadAsync());
//				await Task.WhenAll(tasks);
				foreach (var adminMenu in superUser.AdminMenus)
					await db.Entry(adminMenu).Reference(m => m.Menu).LoadAsync();

				// explicit load AdminRoles, eagerly loading Roles at the same time
				await db.Entry(superUser).Collection(m => m.AdminRoles).Query().Include(m => m.Role).ToListAsync();

				Assert.NotNull(superUser);
				Assert.Equal("test", superUser.Username);
				Assert.Equal("test", superUser.Password);
				Assert.Equal(5, superUser.AdminMenus.Count);
				Assert.NotNull(superUser.AdminMenus[0].Menu);
				Assert.Equal(4, superUser.AdminRoles.Count);
				Assert.NotNull(superUser.AdminRoles[0].Role);

				// queries can do more than just load!  here's an example to get counts without loading entities
				Assert.Equal(5, await db.Entry(superUser).Collection(m => m.AdminMenus).Query().CountAsync());
				Assert.Equal(4, await db.Entry(superUser).Collection(m => m.AdminRoles).Query().CountAsync());
			}
		}
	}
}
