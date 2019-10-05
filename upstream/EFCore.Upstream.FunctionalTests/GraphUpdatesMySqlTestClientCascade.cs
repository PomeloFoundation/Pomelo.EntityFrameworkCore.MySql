// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;

namespace Microsoft.EntityFrameworkCore
{
    public class GraphUpdatesMySqlTestClientCascade : GraphUpdatesMySqlTestBase<GraphUpdatesMySqlTestClientCascade.GraphUpdatesWithClientCascadeMySqlFixture>
    {
        public GraphUpdatesMySqlTestClientCascade(GraphUpdatesWithClientCascadeMySqlFixture fixture)
            : base(fixture)
        {
        }

        public class GraphUpdatesWithClientCascadeMySqlFixture : GraphUpdatesMySqlFixtureBase
        {
            protected override string StoreName { get; } = "GraphClientCascadeUpdatesTest";
            public override bool NoStoreCascades => true;

            protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
            {
                base.OnModelCreating(modelBuilder, context);

                foreach (var foreignKey in modelBuilder.Model
                    .GetEntityTypes()
                    .SelectMany(e => e.GetDeclaredForeignKeys())
                    .Where(e => e.DeleteBehavior == DeleteBehavior.Cascade))
                {
                    foreignKey.DeleteBehavior = DeleteBehavior.ClientCascade;
                }
            }
        }
    }
}
