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

		[Fact]
		public async Task TestGeneratedTime()
		{
			var gt = new GeneratedTime {Name = "test"};

			using (var db = new AppDb())
			{
				db.GeneratedTime.Add(gt);
				await db.SaveChangesAsync();

				Assert.Equal(gt.CreatedDateTime, gt.UpdatedDateTime);
				Assert.Equal(gt.CreatedDateTime3, gt.UpdatedDateTime3);
				Assert.Equal(gt.CreatedDateTime6, gt.UpdatedDateTime6);
				Assert.Equal(gt.CreatedTimestamp, gt.UpdatedTimetamp);
				Assert.Equal(gt.CreatedTimestamp3, gt.UpdatedTimetamp3);
				Assert.Equal(gt.CreatedTimestamp6, gt.UpdatedTimetamp6);

				Assert.InRange(gt.CreatedDateTime3 - gt.CreatedDateTime, TimeSpan.Zero, TimeSpan.FromMilliseconds(999));
				Assert.InRange(gt.CreatedDateTime6 - gt.CreatedDateTime3, TimeSpan.Zero, TimeSpan.FromMilliseconds(0.999));

				Assert.InRange(gt.CreatedTimestamp3 - gt.CreatedTimestamp, TimeSpan.Zero, TimeSpan.FromMilliseconds(999));
				Assert.InRange(gt.CreatedTimestamp6 - gt.CreatedTimestamp3, TimeSpan.Zero, TimeSpan.FromMilliseconds(0.999));

				await Task.Delay(TimeSpan.FromSeconds(1));
				gt.Name = "test2";
				await db.SaveChangesAsync();

				Assert.NotEqual(gt.CreatedDateTime, gt.UpdatedDateTime);
				Assert.NotEqual(gt.CreatedDateTime3, gt.UpdatedDateTime3);
				Assert.NotEqual(gt.CreatedDateTime6, gt.UpdatedDateTime6);
				Assert.NotEqual(gt.CreatedTimestamp, gt.UpdatedTimetamp);
				Assert.NotEqual(gt.CreatedTimestamp3, gt.UpdatedTimetamp3);
				Assert.NotEqual(gt.CreatedTimestamp6, gt.UpdatedTimetamp6);
			}
		}
	}
}
