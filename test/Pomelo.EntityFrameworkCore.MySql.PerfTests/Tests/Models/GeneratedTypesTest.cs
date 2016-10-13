using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.PerfTests.Models;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.PerfTests.Tests.Models
{
	public class GeneratedTypesTest
	{
		[SkipAppVeyorFact]
		public async Task TestGeneratedContact()
		{
			const string email = "bob@example.com";
			const string address = "123 Entity Framework Ln";
			const string city = "Redmond";
			const string state = "WA";
			const string zip = "99999";
			var addressFormatted = string.Join(", ", address, city, state, zip);

			Action<GeneratedContact> testContact = contact =>
			{
				Assert.Equal(email, contact.Email);
				Assert.Equal(addressFormatted, contact.Address);
			};

			using (var db = new AppDb())
			{
				var gen = new GeneratedContact
				{
					Names = new JsonObject<List<string>>(new List<string> {"Bob", "Bobby"}),
					ContactInfo = new JsonObject<Dictionary<string, string>>(new Dictionary<string, string>
					{
						{"Email", email},
						{"Address", address},
						{"City", city},
						{"State", state},
						{"Zip", zip},
					}),
				};

				// test the entity after saving to the db
				db.GeneratedContacts.Add(gen);
				await db.SaveChangesAsync();
				testContact(gen);

				// test the entity after fresh retreival from the database
				var genDb = await db.GeneratedContacts.FirstOrDefaultAsync(m => m.Id == gen.Id);
				testContact(genDb);
			}
		}
	}
}
