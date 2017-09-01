using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Models
{
	public static class GeneratedContactMeta
	{
		public static void OnModelCreating(ModelBuilder modelBuilder)
		{
			// AppVeyor's MySQL version does not support these functions properly
			if (AppConfig.AppVeyor)
				return;

			modelBuilder.Entity<GeneratedContact>(entity =>
			{
				entity.HasIndex(m => m.Name);
				entity.HasIndex(m => m.Email);

				entity.Property(m => m.Name)
					.ValueGeneratedOnAddOrUpdate()
					.HasColumnType(@"VARCHAR(63) GENERATED ALWAYS AS (
						`Names` ->> ""$[0]""
					) VIRTUAL");

				entity.Property(m => m.Email)
					.ValueGeneratedOnAddOrUpdate()
					.HasColumnType(@"VARCHAR(63) GENERATED ALWAYS AS (
						`ContactInfo` ->> ""$.Email""
					) VIRTUAL");

				entity.Property(m => m.Address)
					.ValueGeneratedOnAddOrUpdate()
					.HasColumnType(@"VARCHAR(63) GENERATED ALWAYS AS (
						CONCAT_WS(', ',
							`ContactInfo` ->> ""$.Address"",
                            `ContactInfo` ->> ""$.City"",
                            `ContactInfo` ->> ""$.State"",
                            `ContactInfo` ->> ""$.Zip""
						)) STORED");
			});
		}
	}

	public class GeneratedContact
	{
	    public Guid Id { get; set; }

		public string Name { get; set; }

		public string Email { get; set; }

		public string Address { get; set; }

		public JsonObject<List<string>> Names { get; set; }

		public JsonObject<Dictionary<string, string>> ContactInfo { get; set; }
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

	public class GeneratedConcurrencyToken
	{
		public int Id { get; set; }

		public int Gen { get; set; }

		[Timestamp]
		public DateTime Updated { get; set; }
	}

}
