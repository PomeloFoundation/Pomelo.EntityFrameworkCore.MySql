// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Query.ExpressionTranslators.Internal
{
    public class MySqlCompositeMethodCallTranslator : RelationalCompositeMethodCallTranslator
    {
        private static readonly IMethodCallTranslator[] _methodCallTranslators =
        {
            new MySqlStringSubstringTranslator(),
            new MySqlMathAbsTranslator(),
            new MySqlMathCeilingTranslator(),
            new MySqlMathFloorTranslator(),
            new MySqlMathPowerTranslator(),
            new MySqlMathRoundTranslator(),
            new MySqlMathTruncateTranslator(),
            new MySqlStringReplaceTranslator(),
            new MySqlStringToLowerTranslator(),
            new MySqlStringToUpperTranslator(),
            new MySqlRegexIsMatchTranslator(),
        };

        public MySqlCompositeMethodCallTranslator([NotNull] ILogger<MySqlCompositeMethodCallTranslator> logger)
            : base(logger)
        {
            // ReSharper disable once DoNotCallOverridableMethodsInConstructor
            AddTranslators(_methodCallTranslators);
        }
    }
}
