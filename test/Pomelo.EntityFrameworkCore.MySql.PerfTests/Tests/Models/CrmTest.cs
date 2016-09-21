using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.PerfTests.Models;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.PerfTests.Tests.Models
{
	public class CrmFixture
	{
		public CrmFixture()
		{
			using (var db = new AppDb())
			{
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
		public async Task TestCrmSuperUser()
		{
			using (var db = new AppDb())
			{
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
	}
}
