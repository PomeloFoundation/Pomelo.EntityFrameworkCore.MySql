// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Query.ExpressionTranslators.Internal
{
    public class MySqlCompositeMemberTranslator : RelationalCompositeMemberTranslator
    {
        public MySqlCompositeMemberTranslator()
        {
            var MySqlTranslators = new List<IMemberTranslator>
            {
                new MySqlStringLengthTranslator(),
                new MySqlDateTimeNowTranslator()
            };

            AddTranslators(MySqlTranslators);
        }
    }
}
