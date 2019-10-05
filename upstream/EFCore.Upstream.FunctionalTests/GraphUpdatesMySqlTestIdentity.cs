// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.EntityFrameworkCore
{
    public class GraphUpdatesMySqlTestIdentity : GraphUpdatesMySqlTestBase<GraphUpdatesMySqlTestIdentity.GraphUpdatesWithIdentityMySqlFixture>
    {
        public GraphUpdatesMySqlTestIdentity(GraphUpdatesWithIdentityMySqlFixture fixture)
            : base(fixture)
        {
        }

        public class GraphUpdatesWithIdentityMySqlFixture : GraphUpdatesMySqlFixtureBase
        {
            protected override string StoreName { get; } = "GraphIdentityUpdatesTest";

            protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
            {
                modelBuilder.UseIdentityColumns();

                base.OnModelCreating(modelBuilder, context);
            }
        }
    }
}
