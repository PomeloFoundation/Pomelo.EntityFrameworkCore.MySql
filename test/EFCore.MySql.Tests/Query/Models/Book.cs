using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pomelo.EntityFrameworkCore.MySql.Tests.Query.Models
{
    public class Book
    {
        [Key]
        [MaxLength(64)]
        public string ISBN { get; set; }

        [MaxLength(64)]
        public string Title { get; set; }

        [ForeignKey("Author")]
        public Guid AuthorId { get; set; }

        public virtual Author Author { get; set; }

        [ForeignKey("Press")]
        public Guid PressId { get; set; }

        public virtual Press Press { get; set; }

        public int SaleCount { get; set; }

        public float SinglePrice { get; set; }
    }
}
