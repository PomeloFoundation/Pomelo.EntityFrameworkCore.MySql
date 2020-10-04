using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pomelo.EntityFrameworkCore.MySql.IntegrationTests.Models;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using Pomelo.EntityFrameworkCore.MySql.IntegrationTests.Tests.Attributes;
using Pomelo.EntityFrameworkCore.MySql.Storage;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.IntegrationTests.Tests.Models
{
	public class GeneratedTypesTest
	{
        [SupportedServerVersionFact(ServerVersion.JsonSupportKey, Skip = "Version of MySQL/MariaDB does not support JSON")]
		public async Task TestGeneratedContact()
		{
			const string email = "bob@example.com";
			const string address = "123 Entity Framework Ln";
			const string city = "Redmond";
			const string state = "WA";
			const string zip = "99999";
			var addressFormatted = string.Join(", ", address, city, state, zip);

		    using (var scope = new AppDbScope())
			{
				var db = scope.AppDb;
			    void TestContact(GeneratedContact contact)
			    {
			        var csb = new MySqlConnectionStringBuilder(db.Database.GetDbConnection().ConnectionString);
			        var guidHexStr = csb.OldGuids
			            ? BitConverter.ToString(contact.Id.ToByteArray().Take(8).ToArray()).Replace("-", "")
			            : contact.Id.ToString().Replace("-", "").Substring(0, 16);
			        var guidTicks = Convert.ToInt64("0x" + guidHexStr, 16);
			        var guidDateTime = new DateTime(guidTicks);

			        Assert.InRange(guidDateTime - DateTime.UtcNow, TimeSpan.FromSeconds(-5), TimeSpan.FromSeconds(5));
			        Assert.Equal(email, contact.Email);
			        Assert.Equal(addressFormatted, contact.Address);
			    }

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
				TestContact(gen);

				// test the entity after fresh retreival from the database
				var genDb = await db.GeneratedContacts.FirstOrDefaultAsync(m => m.Id == gen.Id);
				TestContact(genDb);
			}
		}

		[Fact]
		public async Task TestGeneratedTime()
		{
			var gt = new GeneratedTime {Name = "test"};

			using (var scope = new AppDbScope())
			{
				var db = scope.AppDb;
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

		[Fact]
		public async Task TestGeneratedConcurrencyToken()
		{
			var gct = new GeneratedConcurrencyCheck { Gen = 1 };
			using (var scope = new AppDbScope())
			{
				var db = scope.AppDb;
				db.GeneratedConcurrencyCheck.Add(gct);
				await db.SaveChangesAsync();

				using (var scope2 = new AppDbScope())
				{
					var db2 = scope2.AppDb;
					var gct2 = await db2.GeneratedConcurrencyCheck.FindAsync(gct.Id);
					gct2.Gen++;
					await db2.SaveChangesAsync();
				}

				gct.Gen++;
				await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => db.SaveChangesAsync());
			}
		}

	    [Fact]
	    public async Task TestGeneratedRowVersion()
	    {
	        var gct = new GeneratedRowVersion { Gen = 1 };
	        using (var scope = new AppDbScope())
	        {
	            var db = scope.AppDb;
	            db.GeneratedRowVersion.Add(gct);
	            await db.SaveChangesAsync();

	            using (var scope2 = new AppDbScope())
	            {
	                var db2 = scope2.AppDb;
	                var gct2 = await db2.GeneratedRowVersion.FindAsync(gct.Id);
	                gct2.Gen++;
	                await db2.SaveChangesAsync();
	            }

	            gct.Gen++;
	            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => db.SaveChangesAsync());
	        }
	    }
	}
}
