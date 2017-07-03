using System.Collections.Generic;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Models
{
    public class Blog
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public List<BlogPost> Posts { get; set; }
    }

	public class BlogPost
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public string Content { get; set; }

		public int BlogId { get; set; }
		public Blog Blog { get; set; }
	}
}
