using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.Tests
{
    public class MySqlJsonTest
    {

        public class Blog
        {
            public int Id { get; set; }

            public string Title { set; get; }

            public JsonObject<dynamic> Desc { get; set; }

            public JsonObject<string[]> Authors { set; get; }

            public JsonObject<List<Tag>> Tags { set; get; }
        }

        public class Tag
        {
            public string Title { set; get; }

            public string Remark { set; get; }
        }

        public class MyContext : DbContext
        {
            public DbSet<Blog> Blogs { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder
                    .UseMySql(@"Server=localhost;database=test;uid=root;pwd=123456;");
        }


        [Fact]
        public void SaveJsonTest()
        {
            using (var context = new MyContext())
            {
                Blog blog = new Blog();

                blog.Title = "mysqltest";
                blog.Desc = @"{'name':'Amy','mail':'amy @gmail.com'}";
                List<Tag> tags = new List<Tag>();
                tags.Add(new Tag { Remark = "net", Title = "dev" });
                tags.Add(new Tag { Remark = "core", Title = "rel" });
                blog.Tags = tags;

                blog.Authors = new string[] { "bob", "sunnmy" };

                context.Blogs.Add(blog);
                context.SaveChanges();

                Blog blog1 = context.Blogs.AsNoTracking().Where(x => x.Title == "mysqltest").FirstOrDefault();

                Assert.Equal(blog1.Desc.Json, blog.Desc.Json);
                Assert.Equal(blog1.Tags.Json, blog.Tags.Json);
                Assert.Equal(blog1.Authors.Json, blog.Authors.Json);

            }
        }


        [Fact]
        public void DeleteTest()
        {
            using (var context = new MyContext())
            {
                Blog blog = context.Blogs.Where(x => x.Title == "mysqltest").FirstOrDefault();

                blog.Tags.Object.Clear();
                context.SaveChanges();
 
                Assert.Equal(blog.Tags.Object.Count, 0);
            }
        }
    }
}
