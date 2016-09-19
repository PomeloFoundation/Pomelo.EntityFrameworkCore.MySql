using System.Collections.Generic;

namespace Pomelo.EntityFrameworkCore.MySql.PerfTests.Models
{
    public class Blog
    {
        public int BlogId { get; set; }
        public string Title { get; set; }

        public List<Post> Posts { get; set; }
    }
}
