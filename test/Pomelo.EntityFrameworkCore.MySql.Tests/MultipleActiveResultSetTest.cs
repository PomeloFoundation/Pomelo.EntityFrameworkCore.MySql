using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.Tests
{

    public class Blog
    {
        [Key]
        public int Id { set; get; }

        public string Title { set; get; }

        public string Description { set; get; }

        [ForeignKey("User")]
        public int UserId { set; get; }

        public virtual User User { set; get; }
    }

    public class User
    {
        [Key]
        public int Id { set; get; }

        public string UserName { set; get; }

        public virtual ICollection<Blog> Blogs { set; get; }
    }

    public class BlogContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }

        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder
                .UseMySql(@"Server=localhost;database=blogs;uid=root;pwd=123456;");
    }


    public class MultipleActiveResultSetTest
    {
        [Fact]
        public void InitData()
        {
            using (BlogContext db = new BlogContext())
            {
                db.Database.EnsureCreated();

                User user = new User { UserName = "nele" };
                db.Users.Add(user);
                db.SaveChanges();
            }

            using (BlogContext db = new BlogContext())
            {
                List<Blog> blogs = new List<Blog>();

                blogs.Add(new Blog { Title = "test1", UserId = 1, Description = "demo1" });
                blogs.Add(new Blog { Title = "test2", UserId = 1, Description = "demo2" });
                blogs.Add(new Blog { Title = "test3", UserId = 1, Description = "demo3" });
                blogs.Add(new Blog { Title = "test4", UserId = 1, Description = "demo4" });

                db.Blogs.AddRange(blogs);
                db.SaveChanges();
            }
        }

        [Fact]
        public void MultipleActiveResultSetDataTest()
        {
            using (BlogContext db = new BlogContext())
            {
                Blog blog = db.Blogs.Where(x => x.Id == 1).Include(x => x.User).FirstOrDefault();

                Assert.Equal(blog.User.UserName, "nele");
            }
        }

        [Fact]
        public void MultipleActiveResultSetDataTest2()
        {
            using (BlogContext db = new BlogContext())
            {
                User user = db.Users.Where(x => x.Id == 1).Include(x => x.Blogs).FirstOrDefault();

                Assert.Equal(user.Blogs.Count, 4);
            }
        }
    }
}
