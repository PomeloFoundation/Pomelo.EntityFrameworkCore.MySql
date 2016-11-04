using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Pomelo.EntityFrameworkCore.MySql.PerfTests.Models
{
	public static class PeopleMeta
	{
		public static void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<PersonKid>(entity =>
			{
				entity.HasOne(m => m.Teacher)
					.WithMany(m => m.Students)
					.HasForeignKey(m => m.TeacherId)
					.HasPrincipalKey(m => m.Id)
					.OnDelete(DeleteBehavior.Restrict);
			});
		}
	}

	public abstract class Person
	{
		public int Id { get; set; }

		public string Name { get; set; }

		public int? TeacherId { get; set; }

		public PersonFamily Family { get; set; }
	}

	public class PersonKid : Person
	{
		public int Grade { get; set; }

		public PersonTeacher Teacher { get; set; }
	}

	public class PersonTeacher : Person
	{
		public ICollection<PersonKid> Students { get; set; }
	}

	public class PersonParent : Person
	{
		public string Occupation { get; set; }

		public bool OnPta { get; set; }
	}

	public class PersonFamily
	{
		public int Id { get; set; }

		public string LastName { get; set; }

		public ICollection<Person> Members { get; set; }
	}


}