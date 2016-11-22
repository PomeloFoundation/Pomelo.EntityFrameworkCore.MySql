using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Scaffolding.Tests
{
    public partial class eftestsContext : DbContext
    {
        // Unable to generate entity type for table 'hello'. Please see the warning messages.
        // Unable to generate entity type for table 'world'. Please see the warning messages.

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            #warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
            optionsBuilder.UseMySql(@"Server=localhost;User Id=root;Password=123456;Database=eftests");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}