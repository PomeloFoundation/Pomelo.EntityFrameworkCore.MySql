using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Models;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Tests.Models
{
	public class DiscriminatorFixture
	{
		public int LowestTeacherId;
		public int HighestTeacherId;

		public DiscriminatorFixture()
		{
			var famalies = new List<PersonFamily>
			{
				new PersonFamily
				{
					LastName = "Garrison",
				},
				new PersonFamily
				{
					LastName = "Cartman",
				},
				new PersonFamily
				{
					LastName = "McCormick",
				},
				new PersonFamily
				{
					LastName = "Broflovski",
				},
				new PersonFamily
				{
					LastName = "Marsh",
				},
			};
			var teachers = new List<PersonTeacher>
			{
				new PersonTeacher {Name = "Ms. Frizzle"},
				new PersonTeacher {Name = "Mr. Garrison", Family = famalies[0]},
				new PersonTeacher {Name = "Mr. Hat", Family = famalies[0]},
			};
			var students = new List<PersonKid>
			{
				// magic school bus
				new PersonKid {Name = "Arnold", Grade = 2, Teacher = teachers[0]},
				new PersonKid {Name = "Phoebe", Grade = 2, Teacher = teachers[0]},
				new PersonKid {Name = "Wanda", Grade = 2, Teacher = teachers[0]},

				// southpark
				new PersonKid {Name = "Eric", Grade = 4, Teacher = teachers[1], Family = famalies[1]},
				new PersonKid {Name = "Kenny", Grade = 4, Teacher = teachers[1], Family = famalies[2]},
				new PersonKid {Name = "Kyle", Grade = 4, Teacher = teachers[1], Family = famalies[3]},
				new PersonKid {Name = "Stan", Grade = 4, Teacher = teachers[1], Family = famalies[4]},
			};
			var parents = new List<PersonParent>
			{
				new PersonParent {Name = "Liane", Occupation = "Nobody knows...", OnPta = true, Family = famalies[1]},
				new PersonParent {Name = "Stuart", Occupation = "Unemployed", OnPta = false, Family = famalies[2]},
				new PersonParent {Name = "Carol", Occupation = "Dishwasher at Olive Garden", OnPta = false, Family = famalies[2]},
				new PersonParent {Name = "Sheila", Occupation = "Frequent protester", OnPta = true, Family = famalies[3]},
				new PersonParent {Name = "Gerald", Occupation = "Lawyer", OnPta = true, Family = famalies[3]},
				new PersonParent {Name = "Randy", Occupation = "Geologist", OnPta = true, Family = famalies[4]},
				new PersonParent {Name = "Sharon", Occupation = "Receptionist at Tom's Rhinoplasty", OnPta = true, Family = famalies[4]},
			};

			using (var scope = new AppDbScope())
			{
				var db = scope.AppDb;
				db.People.AddRange(teachers);
				db.People.AddRange(students);
				db.People.AddRange(parents);
				db.SaveChanges();
				LowestTeacherId = teachers.First().Id;
				HighestTeacherId = teachers.Last().Id;
			}
		}
	}

	public class DiscriminatorTest : IClassFixture<DiscriminatorFixture>
	{
		private readonly DiscriminatorFixture _fixture;

		public DiscriminatorTest(DiscriminatorFixture fixture)
		{
			_fixture = fixture;
		}

		[Fact(Skip="https://github.com/aspnet/EntityFramework/issues/9038")]
		public async Task TestDiscriminator()
		{
			using (var scope = new AppDbScope())
			{
				var db = scope.AppDb;
				var teachers = await db.People.OfType<PersonTeacher>()
					.Where(m => m.Id >= _fixture.LowestTeacherId && m.Id <= _fixture.HighestTeacherId)
					.OrderBy(m => m.Id)
					.Include(m => m.Family)
					.ThenInclude(m => m.Members)
					.Include(m => m.Students)
					.ThenInclude(m => m.Family)
					.ThenInclude(m => m.Members)
					.ToListAsync();

				Assert.Equal(3, teachers.Count);

				// magic school bus
				var frizzle = teachers[0];

				// ms. frizzle has 3 students
				Assert.Equal(3, frizzle.Students.Count);
				var students = new HashSet<string> {"Arnold", "Phoebe", "Wanda"};
				foreach (var student in frizzle.Students)
					Assert.True(students.Contains(student.Name));

				// southpark
				var garrison = teachers[1];
				var hat = teachers[2];

				// mr. garrison has 4 student
				Assert.Equal(4, garrison.Students.Count);
				students = new HashSet<string> {"Eric Cartman", "Kenny McCormick", "Kyle Broflovski", "Stan Marsh"};
				foreach (var student in garrison.Students)
					Assert.True(students.Contains(student.Name + " " + (student.Family?.LastName ?? "")));

				// everyone's parents are on the PTA except for Kenny's
				var pta = new HashSet<string> {"Liane Cartman", "Sheila Broflovski", "Gerald Broflovski", "Randy Marsh", "Sharon Marsh"};
				var ptaSize = 0;
				foreach (var student in garrison.Students)
				{
					foreach (var member in student.Family.Members)
					{
						if (member is PersonParent && (member as PersonParent).OnPta)
						{
							ptaSize++;
							Assert.True(pta.Contains(member.Name + " " + (member.Family?.LastName ?? "")));
						}
					}
				}
				Assert.Equal(pta.Count, ptaSize);

				// mr. hat has no students, but he is in mr. garrison's family
				Assert.Equal(0, hat.Students.Count);
				Assert.Contains(hat, garrison.Family.Members);
			}
		}
	}

}