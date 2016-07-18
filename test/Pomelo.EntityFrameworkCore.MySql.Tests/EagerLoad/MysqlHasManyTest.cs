using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.Tests.EagerLoad
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
        {
            optionsBuilder
                  .UseMySql(@"Server=127.0.0.1;database=hasmanytest;uid=root;pwd=Password12!;");
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<User>(e =>
            {
                e.HasMany(x => x.Blogs);
            });
        }
    }


    public class MysqlHasManyTest
    {

        public MysqlHasManyTest()
        {
            using (BlogContext db = new BlogContext())
            {
                db.Database.EnsureDeleted();

                db.Database.EnsureCreated();
            }
        }

        [Fact]
        public void InitData()
        {
            using (BlogContext db = new BlogContext())
            {

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
        public void IncludeTest()
        {
            using (BlogContext db = new BlogContext())
            {

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

            using (BlogContext db = new BlogContext())
            {
                Blog blog = db.Blogs.Where(x => x.Id == 1).Include(x => x.User).FirstOrDefault();

                Assert.Equal(blog.User.UserName, "nele");
            }
        }

        [Fact]
        public void HasManyTest()
        {

            using (BlogContext db = new BlogContext())
            {

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

            using (BlogContext db = new BlogContext())
            {
                User user = db.Users.Where(x => x.Id == 1).Include(x => x.Blogs).FirstOrDefault();

                Assert.Equal(user.Blogs.Count, 4);
            }
        }


    }
}
