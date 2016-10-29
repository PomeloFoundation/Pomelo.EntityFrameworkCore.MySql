using System.Threading.Tasks;
using Pomelo.EntityFrameworkCore.MySql.PerfTests.Models;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.PerfTests.Tests.Connection
{
	public class ConnectionTest
	{

		[Fact]
		public async Task AffectedRowsFalse()
		{
			var title = "test";
			var blog = new Blog{ Title = title };
			using (var db = new AppDb())
			{
				db.Blogs.Add(blog);
				await db.SaveChangesAsync();
			}
			Assert.True(blog.Id > 0);

			// this will throw a DbUpdateConcurrencyException if UseAffectedRows=true
			var sameBlog = new Blog { Id = blog.Id, Title = title };
			using (var db = new AppDb())
			{
				db.Blogs.Update(sameBlog);
				await db.SaveChangesAsync();
			}
			Assert.Equal(blog.Id, sameBlog.Id);
		}

	}
}