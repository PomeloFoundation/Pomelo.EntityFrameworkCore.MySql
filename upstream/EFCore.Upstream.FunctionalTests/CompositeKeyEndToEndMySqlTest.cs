// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.TestUtilities;

namespace Microsoft.EntityFrameworkCore
{
    public class CompositeKeyEndToEndMySqlTest : CompositeKeyEndToEndTestBase<CompositeKeyEndToEndMySqlTest.CompositeKeyEndToEndMySqlFixture>
    {
        public CompositeKeyEndToEndMySqlTest(CompositeKeyEndToEndMySqlFixture fixture)
            : base(fixture)
        {
        }

        public class CompositeKeyEndToEndMySqlFixture : CompositeKeyEndToEndFixtureBase
        {
            protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;
        }
    }
}
