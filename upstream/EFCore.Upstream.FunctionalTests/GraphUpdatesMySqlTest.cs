// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Microsoft.EntityFrameworkCore
{
    public abstract class GraphUpdatesMySqlTest
    {
        public abstract class GraphUpdatesMySqlTestBase<TFixture> : GraphUpdatesTestBase<TFixture>
            where TFixture : GraphUpdatesMySqlTestBase<TFixture>.GraphUpdatesMySqlFixtureBase, new()
        {
            protected GraphUpdatesMySqlTestBase(TFixture fixture)
                : base(fixture)
            {
            }

            protected override void UseTransaction(DatabaseFacade facade, IDbContextTransaction transaction)
                => facade.UseTransaction(transaction.GetDbTransaction());

            public abstract class GraphUpdatesMySqlFixtureBase : GraphUpdatesFixtureBase
            {
                public TestSqlLoggerFactory TestSqlLoggerFactory => (TestSqlLoggerFactory)ServiceProvider.GetRequiredService<ILoggerFactory>();
                protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;
            }
        }

        public class Identity : GraphUpdatesMySqlTestBase<Identity.GraphUpdatesWithIdentityMySqlFixture>
        {
            public Identity(GraphUpdatesWithIdentityMySqlFixture fixture)
                : base(fixture)
            {
            }

            public class GraphUpdatesWithIdentityMySqlFixture : GraphUpdatesMySqlFixtureBase
            {
                protected override string StoreName { get; } = "GraphIdentityUpdatesTest";

                protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
                {
                    modelBuilder.ForMySqlUseIdentityColumns();

                    base.OnModelCreating(modelBuilder, context);
                }
            }
        }

        public class Restrict : GraphUpdatesMySqlTestBase<Restrict.GraphUpdatesWithRestrictMySqlFixture>
        {
            public Restrict(GraphUpdatesWithRestrictMySqlFixture fixture)
                : base(fixture)
            {
            }

            public override DbUpdateException Optional_One_to_one_relationships_are_one_to_one()
            {
                var updateException = base.Optional_One_to_one_relationships_are_one_to_one();

                // Disabled check -- see issue #11031
                //Assert.Contains("IX_OptionalSingle1_RootId", updateException.InnerException.Message);

                return updateException;
            }

            public override DbUpdateException Required_One_to_one_relationships_are_one_to_one()
            {
                var updateException = base.Required_One_to_one_relationships_are_one_to_one();

                // Disabled check -- see issue #11031
                //Assert.Contains("PK_RequiredSingle1", updateException.InnerException.Message);

                return updateException;
            }

            public override DbUpdateException Optional_One_to_one_with_AK_relationships_are_one_to_one()
            {
                var updateException = base.Optional_One_to_one_with_AK_relationships_are_one_to_one();

                // Disabled check -- see issue #11031
                //Assert.Contains("IX_OptionalSingleAk1_RootId", updateException.InnerException.Message);

                return updateException;
            }

            public override DbUpdateException Required_One_to_one_with_AK_relationships_are_one_to_one()
            {
                var updateException = base.Required_One_to_one_with_AK_relationships_are_one_to_one();

                // Disabled check -- see issue #11031
                //Assert.Contains("IX_RequiredSingleAk1_RootId", updateException.InnerException.Message);

                return updateException;
            }

            public override DbUpdateException Save_required_one_to_one_changed_by_reference(ChangeMechanism changeMechanism)
            {
                var updateException = base.Save_required_one_to_one_changed_by_reference(changeMechanism);

                // Disabled check -- see issue #11031
                //Assert.Contains("FK_RequiredSingle2_RequiredSingle1_Id", updateException.InnerException.Message);

                return updateException;
            }

            public override DbUpdateException Sever_required_one_to_one(ChangeMechanism changeMechanism)
            {
                var updateException = base.Sever_required_one_to_one(changeMechanism);

                // Disabled check -- see issue #11031
                //Assert.Contains("FK_RequiredSingle2_RequiredSingle1_Id", updateException.InnerException.Message);

                return updateException;
            }

            public override DbUpdateException Required_many_to_one_dependents_are_cascade_deleted()
            {
                var updateException = base.Required_many_to_one_dependents_are_cascade_deleted();

                // Disabled check -- see issue #11031
                //Assert.Contains("FK_Required2_Required1_ParentId", updateException.InnerException.Message);

                return updateException;
            }

            public override DbUpdateException Optional_many_to_one_dependents_are_orphaned()
            {
                var updateException = base.Optional_many_to_one_dependents_are_orphaned();

                // Disabled check -- see issue #11031
                //Assert.Contains("FK_Optional2_Optional1_ParentId", updateException.InnerException.Message);

                return updateException;
            }

            public override DbUpdateException Optional_one_to_one_are_orphaned()
            {
                var updateException = base.Optional_one_to_one_are_orphaned();

                // Disabled check -- see issue #11031
                //Assert.Contains("FK_OptionalSingle2_OptionalSingle1_BackId", updateException.InnerException.Message);

                return updateException;
            }

            public override DbUpdateException Required_one_to_one_are_cascade_deleted()
            {
                var updateException = base.Required_one_to_one_are_cascade_deleted();

                // Disabled check -- see issue #11031
                //Assert.Contains("FK_RequiredSingle2_RequiredSingle1_Id", updateException.InnerException.Message);

                return updateException;
            }

            public override DbUpdateException Required_non_PK_one_to_one_are_cascade_deleted()
            {
                var updateException = base.Required_non_PK_one_to_one_are_cascade_deleted();

                // Disabled check -- see issue #11031
                //Assert.Contains("FK_RequiredNonPkSingle2_RequiredNonPkSingle1_BackId", updateException.InnerException.Message);

                return updateException;
            }

            public override DbUpdateException Optional_many_to_one_dependents_with_alternate_key_are_orphaned()
            {
                var updateException = base.Optional_many_to_one_dependents_with_alternate_key_are_orphaned();

                // Disabled check -- see issue #11031
                //Assert.Contains("FK_OptionalAk2_OptionalAk1_ParentId", updateException.InnerException.Message);

                return updateException;
            }

            public override DbUpdateException Required_many_to_one_dependents_with_alternate_key_are_cascade_deleted()
            {
                var updateException = base.Required_many_to_one_dependents_with_alternate_key_are_cascade_deleted();

                // Disabled check -- see issue #11031
                //Assert.Contains("FK_RequiredAk2_RequiredAk1_ParentId", updateException.InnerException.Message);

                return updateException;
            }

            public override DbUpdateException Optional_one_to_one_with_alternate_key_are_orphaned()
            {
                var updateException = base.Optional_one_to_one_with_alternate_key_are_orphaned();

                // Disabled check -- see issue #11031
                //Assert.Contains("FK_OptionalSingleAk2_OptionalSingleAk1_BackId", updateException.InnerException.Message);

                return updateException;
            }

            public override DbUpdateException Required_one_to_one_with_alternate_key_are_cascade_deleted()
            {
                var updateException = base.Required_one_to_one_with_alternate_key_are_cascade_deleted();

                // Disabled check -- see issue #11031
                //Assert.Contains("FK_RequiredSingleAk2_RequiredSingleAk1_BackId", updateException.InnerException.Message);

                return updateException;
            }

            public override DbUpdateException Required_non_PK_one_to_one_with_alternate_key_are_cascade_deleted()
            {
                var updateException = base.Required_non_PK_one_to_one_with_alternate_key_are_cascade_deleted();

                // Disabled check -- see issue #11031
                //Assert.Contains("FK_RequiredNonPkSingleAk2_RequiredNonPkSingleAk1_BackId", updateException.InnerException.Message);

                return updateException;
            }

            public override DbUpdateException Required_many_to_one_dependents_are_cascade_deleted_in_store()
            {
                var updateException = base.Required_many_to_one_dependents_are_cascade_deleted_in_store();

                // Disabled check -- see issue #11031
                //Assert.Contains("FK_Required2_Required1_ParentId", updateException.InnerException.Message);

                return updateException;
            }

            public override DbUpdateException Required_one_to_one_are_cascade_deleted_in_store()
            {
                var updateException = base.Required_one_to_one_are_cascade_deleted_in_store();

                // Disabled check -- see issue #11031
                //Assert.Contains("FK_RequiredSingle2_RequiredSingle1_Id", updateException.InnerException.Message);

                return updateException;
            }

            public override DbUpdateException Required_non_PK_one_to_one_are_cascade_deleted_in_store()
            {
                var updateException = base.Required_non_PK_one_to_one_are_cascade_deleted_in_store();

                // Disabled check -- see issue #11031
                //Assert.Contains("FK_RequiredNonPkSingle2_RequiredNonPkSingle1_BackId", updateException.InnerException.Message);

                return updateException;
            }

            public override DbUpdateException Required_many_to_one_dependents_with_alternate_key_are_cascade_deleted_in_store()
            {
                var updateException = base.Required_many_to_one_dependents_with_alternate_key_are_cascade_deleted_in_store();

                // Disabled check -- see issue #11031
                //Assert.Contains("FK_RequiredAk2_RequiredAk1_ParentId", updateException.InnerException.Message);

                return updateException;
            }

            public override DbUpdateException Required_one_to_one_with_alternate_key_are_cascade_deleted_in_store()
            {
                var updateException = base.Required_one_to_one_with_alternate_key_are_cascade_deleted_in_store();

                // Disabled check -- see issue #11031
                //Assert.Contains("FK_RequiredSingleAk2_RequiredSingleAk1_BackId", updateException.InnerException.Message);

                return updateException;
            }

            public override DbUpdateException Required_non_PK_one_to_one_with_alternate_key_are_cascade_deleted_in_store()
            {
                var updateException = base.Required_non_PK_one_to_one_with_alternate_key_are_cascade_deleted_in_store();

                // Disabled check -- see issue #11031
                //Assert.Contains("FK_RequiredNonPkSingleAk2_RequiredNonPkSingleAk1_BackId", updateException.InnerException.Message);

                return updateException;
            }

            public override DbUpdateException Optional_many_to_one_dependents_are_orphaned_in_store()
            {
                var updateException = base.Optional_many_to_one_dependents_are_orphaned_in_store();

                // Disabled check -- see issue #11031
                //Assert.Contains("FK_Optional2_Optional1_ParentId", updateException.InnerException.Message);

                return updateException;
            }

            public override DbUpdateException Optional_one_to_one_are_orphaned_in_store()
            {
                var updateException = base.Optional_one_to_one_are_orphaned_in_store();

                // Disabled check -- see issue #11031
                //Assert.Contains("FK_OptionalSingle2_OptionalSingle1_BackId", updateException.InnerException.Message);

                return updateException;
            }

            public override DbUpdateException Optional_many_to_one_dependents_with_alternate_key_are_orphaned_in_store()
            {
                var updateException = base.Optional_many_to_one_dependents_with_alternate_key_are_orphaned_in_store();

                // Disabled check -- see issue #11031
                //Assert.Contains("FK_OptionalAk2_OptionalAk1_ParentId", updateException.InnerException.Message);

                return updateException;
            }

            public override DbUpdateException Optional_one_to_one_with_alternate_key_are_orphaned_in_store()
            {
                var updateException = base.Optional_one_to_one_with_alternate_key_are_orphaned_in_store();

                // Disabled check -- see issue #11031
                //Assert.Contains("FK_OptionalSingleAk2_OptionalSingleAk1_BackId", updateException.InnerException.Message);

                return updateException;
            }

            public override DbUpdateException Required_many_to_one_dependents_are_cascade_deleted_starting_detached()
            {
                var updateException = base.Required_many_to_one_dependents_are_cascade_deleted_starting_detached();

                // Disabled check -- see issue #11031
                //Assert.Contains("FK_Required2_Required1_ParentId", updateException.InnerException.Message);

                return updateException;
            }

            public override DbUpdateException Optional_many_to_one_dependents_are_orphaned_starting_detached()
            {
                var updateException = base.Optional_many_to_one_dependents_are_orphaned_starting_detached();

                // Disabled check -- see issue #11031
                //Assert.Contains("FK_Optional2_Optional1_ParentId", updateException.InnerException.Message);

                return updateException;
            }

            public override DbUpdateException Optional_one_to_one_are_orphaned_starting_detached()
            {
                var updateException = base.Optional_one_to_one_are_orphaned_starting_detached();

                // Disabled check -- see issue #11031
                //Assert.Contains("FK_OptionalSingle2_OptionalSingle1_BackId", updateException.InnerException.Message);

                return updateException;
            }

            public override DbUpdateException Required_one_to_one_are_cascade_deleted_starting_detached()
            {
                var updateException = base.Required_one_to_one_are_cascade_deleted_starting_detached();

                // Disabled check -- see issue #11031
                //Assert.Contains("FK_RequiredSingle2_RequiredSingle1_Id", updateException.InnerException.Message);

                return updateException;
            }

            public override DbUpdateException Required_non_PK_one_to_one_are_cascade_deleted_starting_detached()
            {
                var updateException = base.Required_non_PK_one_to_one_are_cascade_deleted_starting_detached();

                // Disabled check -- see issue #11031
                //Assert.Contains("FK_RequiredNonPkSingle2_RequiredNonPkSingle1_BackId", updateException.InnerException.Message);

                return updateException;
            }

            public override DbUpdateException Optional_many_to_one_dependents_with_alternate_key_are_orphaned_starting_detached()
            {
                var updateException = base.Optional_many_to_one_dependents_with_alternate_key_are_orphaned_starting_detached();

                // Disabled check -- see issue #11031
                //Assert.Contains("FK_OptionalAk2_OptionalAk1_ParentId", updateException.InnerException.Message);

                return updateException;
            }

            public override DbUpdateException Required_many_to_one_dependents_with_alternate_key_are_cascade_deleted_starting_detached()
            {
                var updateException = base.Required_many_to_one_dependents_with_alternate_key_are_cascade_deleted_starting_detached();

                // Disabled check -- see issue #11031
                //Assert.Contains("FK_RequiredAk2_RequiredAk1_ParentId", updateException.InnerException.Message);

                return updateException;
            }

            public override DbUpdateException Optional_one_to_one_with_alternate_key_are_orphaned_starting_detached()
            {
                var updateException = base.Optional_one_to_one_with_alternate_key_are_orphaned_starting_detached();

                // Disabled check -- see issue #11031
                //Assert.Contains("FK_OptionalSingleAk2_OptionalSingleAk1_BackId", updateException.InnerException.Message);

                return updateException;
            }

            public override DbUpdateException Required_one_to_one_with_alternate_key_are_cascade_deleted_starting_detached()
            {
                var updateException = base.Required_one_to_one_with_alternate_key_are_cascade_deleted_starting_detached();

                // Disabled check -- see issue #11031
                //Assert.Contains("FK_RequiredSingleAk2_RequiredSingleAk1_BackId", updateException.InnerException.Message);

                return updateException;
            }

            public override DbUpdateException Required_non_PK_one_to_one_with_alternate_key_are_cascade_deleted_starting_detached()
            {
                var updateException = base.Required_non_PK_one_to_one_with_alternate_key_are_cascade_deleted_starting_detached();

                // Disabled check -- see issue #11031
                //Assert.Contains("FK_RequiredNonPkSingleAk2_RequiredNonPkSingleAk1_BackId", updateException.InnerException.Message);

                return updateException;
            }

            public override DbUpdateException Required_many_to_one_dependents_are_cascade_detached_when_Added()
            {
                var updateException = base.Required_many_to_one_dependents_are_cascade_detached_when_Added();

                // Disabled check -- see issue #11031
                //Assert.Contains("FK_Required2_Required1_ParentId", updateException.InnerException.Message);

                return updateException;
            }

            public override DbUpdateException Required_one_to_one_are_cascade_detached_when_Added()
            {
                var updateException = base.Required_one_to_one_are_cascade_detached_when_Added();

                // Disabled check -- see issue #11031
                //Assert.Contains("FK_RequiredSingle2_RequiredSingle1_Id", updateException.InnerException.Message);

                return updateException;
            }

            public override DbUpdateException Required_non_PK_one_to_one_are_cascade_detached_when_Added()
            {
                var updateException = base.Required_non_PK_one_to_one_are_cascade_detached_when_Added();

                // Disabled check -- see issue #11031
                //Assert.Contains("FK_RequiredNonPkSingle2_RequiredNonPkSingle1_BackId", updateException.InnerException.Message);

                return updateException;
            }

            public override DbUpdateException Required_many_to_one_dependents_with_alternate_key_are_cascade_detached_when_Added()
            {
                var updateException = base.Required_many_to_one_dependents_with_alternate_key_are_cascade_detached_when_Added();

                // Disabled check -- see issue #11031
                //Assert.Contains("FK_RequiredAk2_RequiredAk1_ParentId", updateException.InnerException.Message);

                return updateException;
            }

            public override DbUpdateException Required_one_to_one_with_alternate_key_are_cascade_detached_when_Added()
            {
                var updateException = base.Required_one_to_one_with_alternate_key_are_cascade_detached_when_Added();

                // Disabled check -- see issue #11031
                //Assert.Contains("FK_RequiredSingleAk2_RequiredSingleAk1_BackId", updateException.InnerException.Message);

                return updateException;
            }

            public override DbUpdateException Required_non_PK_one_to_one_with_alternate_key_are_cascade_detached_when_Added()
            {
                var updateException = base.Required_non_PK_one_to_one_with_alternate_key_are_cascade_detached_when_Added();

                // Disabled check -- see issue #11031
                //Assert.Contains("FK_RequiredNonPkSingleAk2_RequiredNonPkSingleAk1_BackId", updateException.InnerException.Message);

                return updateException;
            }

            public class GraphUpdatesWithRestrictMySqlFixture : GraphUpdatesMySqlFixtureBase
            {
                protected override string StoreName { get; } = "GraphRestrictUpdatesTest";
                public override bool ForceRestrict => true;

                protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
                {
                    base.OnModelCreating(modelBuilder, context);

                    foreach (var foreignKey in modelBuilder.Model
                        .GetEntityTypes()
                        .SelectMany(e => e.GetDeclaredForeignKeys()))
                    {
                        foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
                    }
                }
            }
        }

        [MySqlCondition(MySqlCondition.SupportsSequences)]
        public class Sequence : GraphUpdatesMySqlTestBase<Sequence.GraphUpdatesWithSequenceMySqlFixture>
        {
            public Sequence(GraphUpdatesWithSequenceMySqlFixture fixture)
                : base(fixture)
            {
            }

            public class GraphUpdatesWithSequenceMySqlFixture : GraphUpdatesMySqlFixtureBase
            {
                protected override string StoreName { get; } = "GraphSequenceUpdatesTest";

                protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
                {
                    modelBuilder.ForMySqlUseSequenceHiLo(); // ensure model uses sequences
                    base.OnModelCreating(modelBuilder, context);
                }
            }
        }
    }
}
