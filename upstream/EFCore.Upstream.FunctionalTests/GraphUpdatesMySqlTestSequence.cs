// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.TestUtilities;

namespace Microsoft.EntityFrameworkCore
{
    [MySqlCondition(MySqlCondition.SupportsSequences)]
    public class GraphUpdatesMySqlTestSequence : GraphUpdatesMySqlTestBase<
        GraphUpdatesMySqlTestSequence.GraphUpdatesWithSequenceMySqlFixture>
    {
        public GraphUpdatesMySqlTestSequence(GraphUpdatesWithSequenceMySqlFixture fixture)
            : base(fixture)
        {
        }

        public class GraphUpdatesWithSequenceMySqlFixture : GraphUpdatesMySqlFixtureBase
        {
            protected override string StoreName { get; } = "GraphSequenceUpdatesTest";

            protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
            {
                modelBuilder.UseHiLo(); // ensure model uses sequences
                base.OnModelCreating(modelBuilder, context);
            }
        }
    }
}
