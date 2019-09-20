// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.TestUtilities;

namespace Microsoft.EntityFrameworkCore
{
    public class MusicStoreMySqlTest : MusicStoreTestBase<MusicStoreMySqlTest.MusicStoreMySqlFixture>
    {
        public MusicStoreMySqlTest(MusicStoreMySqlFixture fixture)
            : base(fixture)
        {
        }

        public class MusicStoreMySqlFixture : MusicStoreFixtureBase
        {
            protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;
        }
    }
}
