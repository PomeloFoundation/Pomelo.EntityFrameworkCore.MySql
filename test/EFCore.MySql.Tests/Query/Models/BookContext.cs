using Microsoft.EntityFrameworkCore;

namespace Pomelo.EntityFrameworkCore.MySql.Tests.Query.Models
{
    public class BookContext : DbContext
    {
        public DbSet<Author> Authors { get; set; }

        public DbSet<Book> Books { get; set; }

        public DbSet<Press> Presses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseMySql("Server=localhost; Uid=root; Pwd=123456; Database=pomelo_query_tests");
        }
    }
}
