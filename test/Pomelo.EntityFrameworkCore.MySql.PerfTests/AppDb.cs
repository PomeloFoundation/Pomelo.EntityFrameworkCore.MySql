using System.Data.Common;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pomelo.EntityFrameworkCore.MySql.PerfTests.Models;

namespace Pomelo.EntityFrameworkCore.MySql.PerfTests
{
	public class AppDb : IdentityDbContext<AppIdentityUser>
	{
		// blog
		public DbSet<Blog> Blogs { get; set; }

		// crm
		public DbSet<CrmAdmin> CrmAdmins { get; set; }
		public DbSet<CrmRole> CrmRoles { get; set; }
		public DbSet<CrmMenu> CrmMenus { get; set; }

		// data types
		public DbSet<DataTypesSimple> DataTypesSimple { get; set; }
		public DbSet<DataTypesVariable> DataTypesVariable { get; set; }

		// generated data types
		public DbSet<GeneratedContact> GeneratedContacts { get; set; }
		public DbSet<GeneratedTime> GeneratedTime { get; set; }

		// people
		public DbSet<Person> People { get; set; }
		public DbSet<PersonTeacher> PeopleTeachers { get; set; }
		public DbSet<PersonKid> PeopleKids { get; set; }
		public DbSet<PersonParent> PeopleParents { get; set; }
		public DbSet<PersonFamily> PeopleFamilies { get; set; }

		private readonly bool _configured;
	    private readonly DbConnection _connection;

		public AppDb()
		{
			_configured = false;
		}

		public AppDb(DbContextOptions options) : base(options)
		{
			_configured = true;
		}

	    public AppDb(DbConnection connection)
	    {
	        _configured = false;
	        _connection = connection;
	    }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
		    if (_configured)
		        return;

		    if (_connection != null)
                optionsBuilder.UseMySql(_connection, options => options.MaxBatchSize(AppConfig.EfBatchSize));
		    else
		        optionsBuilder.UseMySql(AppConfig.Config["Data:ConnectionString"], options => options.MaxBatchSize(AppConfig.EfBatchSize));

		    optionsBuilder.UseLoggerFactory(new LoggerFactory().AddConsole(AppConfig.Config.GetSection("Logging")));
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// Add our models fluent APIs
			CrmMeta.OnModelCreating(modelBuilder);
			GeneratedContactMeta.OnModelCreating(modelBuilder);
			PeopleMeta.OnModelCreating(modelBuilder);
		}
	}
}
