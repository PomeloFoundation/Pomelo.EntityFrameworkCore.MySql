// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Query.ExpressionTranslators.Internal
{
    public class MySqlMathAbsTranslator : MultipleOverloadStaticMethodCallTranslator
    {
        public MySqlMathAbsTranslator()
            : base(typeof(Math), "Abs", "ABS")
        {
        }
    }
}
