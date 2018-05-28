// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.TestUtilities;

namespace Microsoft.EntityFrameworkCore.Query
{
    public class NullKeysMySqlTest : NullKeysTestBase<NullKeysMySqlTest.NullKeysMySqlFixture>
    {
        public NullKeysMySqlTest(NullKeysMySqlFixture fixture)
            : base(fixture)
        {
        }

        public class NullKeysMySqlFixture : NullKeysFixtureBase
        {
            protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;
        }
    }
}
