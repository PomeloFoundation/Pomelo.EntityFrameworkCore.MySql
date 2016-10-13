using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Pomelo.EntityFrameworkCore.MySql.PerfTests.Models
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
		public int Id { get; set; }

		public string Name { get; set; }

		public string Email { get; set; }

		public string Address { get; set; }

		public JsonObject<List<string>> Names { get; set; }

		public JsonObject<Dictionary<string, string>> ContactInfo { get; set; }
	}

}
