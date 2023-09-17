﻿using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query;

public class TPCInheritanceQueryMySqlFixture : TPCInheritanceQueryFixture
{
    protected override ITestStoreFactory TestStoreFactory
        => MySqlTestStoreFactory.Instance;

    // TODO: Add sequence support for server implementations that have them.
    public override bool UseGeneratedKeys
        => false;

    protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
    {
        base.OnModelCreating(modelBuilder, context);

        // We currently do not support an official way to set a seed and auto_increment value for auto_increment columns, which is needed
        // for TPC if the database implementation does not support sequences (which MariaDB does, but we have not fully implemented yet).
        // We therefore just remove the auto_increment flag from the appropriate entities here, so we do not trigger the related TPC
        // warning by EF Core.
        foreach (var tpcPrimaryKey in modelBuilder.Model.GetEntityTypes()
                     .Where(e => e.GetMappingStrategy() == RelationalAnnotationNames.TpcMappingStrategy)
                     .Select(e => e.FindPrimaryKey()))
        {
            tpcPrimaryKey.Properties.Single().ValueGenerated = ValueGenerated.Never;
        }
    }
}
