using System;
using System.Collections.Generic;

namespace Pomelo.EntityFrameworkCore.MySql.Tests.Query.Models
{
    public class Author
    {
        public Guid Id { get; set; }
        
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public virtual ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
