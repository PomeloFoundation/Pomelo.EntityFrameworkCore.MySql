using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Tests;

namespace Pomelo.EntityFrameworkCore.MySql.IntegrationTests.Models
{
    public static class GeneratedContactMeta
    {
        public static void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (!AppConfig.ServerVersion.Supports.Json)
            {
                return;
            }

            modelBuilder.Entity<GeneratedContact>(entity =>
            {
                entity.HasIndex(m => m.Name);
                entity.HasIndex(m => m.Email);

                entity.Property(m => m.Name)
                    .ValueGeneratedOnAddOrUpdate()
                    .HasColumnType(@"VARCHAR(63)")
                    .HasComputedColumnSql(@"JSON_UNQUOTE(JSON_EXTRACT(`Names`, ""$[0]""))");

                entity.Property(m => m.Email)
                    .ValueGeneratedOnAddOrUpdate()
                    .HasColumnType(@"VARCHAR(63)")
                    .HasComputedColumnSql(@"JSON_UNQUOTE(JSON_EXTRACT(`ContactInfo`, ""$.Email""))");

                entity.Property(m => m.Address)
                    .ValueGeneratedOnAddOrUpdate()
                    .HasColumnType(@"VARCHAR(63)")
                    .HasComputedColumnSql(@"CONCAT_WS(', ',	JSON_UNQUOTE(JSON_EXTRACT(`ContactInfo`, ""$.Address"")), JSON_UNQUOTE(JSON_EXTRACT(`ContactInfo`, ""$.City"")), JSON_UNQUOTE(JSON_EXTRACT(`ContactInfo`, ""$.State"")), JSON_UNQUOTE(JSON_EXTRACT(`ContactInfo`, ""$.Zip"")))");
            });
        }
    }

    public class ContactInfo
    {
        public string Email { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
    }

    public class GeneratedContact
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Address { get; set; }

        public List<string> Names { get; set; }

        public ContactInfo ContactInfo { get; set; }
    }

    public class GeneratedTime
    {
        public int Id { get; set; }

        public string Name { get; set; }

        [Column(TypeName = "DATETIME")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedDateTime { get; set; }

        [Column(TypeName = "DATETIME")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedDateTime { get; set; }

        [Column(TypeName = "DATETIME(3)")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedDateTime3 { get; set; }

        [Column(TypeName = "DATETIME(3)")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedDateTime3 { get; set; }

        // DATETIME(6) is the default
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedDateTime6 { get; set; }

        // DATETIME(6) is the default
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedDateTime6 { get; set; }

        [Column(TypeName = "TIMESTAMP")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedTimestamp { get; set; }

        [Column(TypeName = "TIMESTAMP")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedTimetamp { get; set; }

        [Column(TypeName = "TIMESTAMP(3)")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedTimestamp3 { get; set; }

        [Column(TypeName = "TIMESTAMP(3)")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedTimetamp3 { get; set; }

        [Column(TypeName = "TIMESTAMP(6)")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedTimestamp6 { get; set; }

        [Column(TypeName = "TIMESTAMP(6)")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedTimetamp6 { get; set; }
    }

    public interface IGeneratedConcurrencyModel
    {
        int Id { get; set; }

        int Gen { get; set; }
    }

    public class GeneratedConcurrencyCheck : IGeneratedConcurrencyModel
    {
        public int Id { get; set; }

        public int Gen { get; set; }

        [ConcurrencyCheck]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime Updated { get; set; }
    }

    public class GeneratedRowVersion : IGeneratedConcurrencyModel
    {
        public int Id { get; set; }

        public int Gen { get; set; }

        [Timestamp]
        public DateTime RowVersion { get; set; }
    }

}
