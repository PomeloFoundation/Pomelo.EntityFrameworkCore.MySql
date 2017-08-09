using System;
using System.Collections.Generic;

namespace EFCore.MySql.Tests.Query.Models
{
    public class Press
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Tel { get; set; }

        public virtual ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
