// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;

//ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.TestUtilities
{
    public class TestRelationalCompositeMethodCallTranslator : RelationalCompositeMethodCallTranslator
    {
        public TestRelationalCompositeMethodCallTranslator(RelationalCompositeMethodCallTranslatorDependencies dependencies)
            : base(dependencies)
        {
        }
    }
}
