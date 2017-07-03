// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;

namespace Microsoft.EntityFrameworkCore.TestUtilities
{
    public class TestRelationalCompositeMemberTranslator : RelationalCompositeMemberTranslator
    {
        public TestRelationalCompositeMemberTranslator(RelationalCompositeMemberTranslatorDependencies dependencies)
            : base(dependencies)
        {
        }
    }
}
