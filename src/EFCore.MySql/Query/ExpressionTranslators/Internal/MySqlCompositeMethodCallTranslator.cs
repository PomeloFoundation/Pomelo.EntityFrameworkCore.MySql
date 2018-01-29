// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using JetBrains.Annotations;

//ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Query.ExpressionTranslators.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class MySqlCompositeMethodCallTranslator : RelationalCompositeMethodCallTranslator
    {
        private static readonly IMethodCallTranslator[] _methodCallTranslators =
        {
            new MySqlContainsOptimizedTranslator(),
            new MySqlConvertTranslator(),
            new MySqlDateAddTranslator(),
            new MySqlEndsWithOptimizedTranslator(),
            new MySqlMathTranslator(),
            new MySqlNewGuidTranslator(),
            new MySqlObjectToStringTranslator(),
            new MySqlRegexIsMatchTranslator(),
            new MySqlStartsWithOptimizedTranslator(),
            new MySqlStringIsNullOrWhiteSpaceTranslator(),
            new MySqlStringReplaceTranslator(),
            new MySqlStringSubstringTranslator(),
            new MySqlStringToLowerTranslator(),
            new MySqlStringToUpperTranslator(),
            new MySqlStringTrimEndTranslator(),
            new MySqlStringTrimStartTranslator(),
            new MySqlStringTrimTranslator()
        };

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public MySqlCompositeMethodCallTranslator(
            [NotNull] RelationalCompositeMethodCallTranslatorDependencies dependencies)
            : base(dependencies)
        {
            // ReSharper disable once DoNotCallOverridableMethodsInConstructor
            AddTranslators(_methodCallTranslators);
        }
    }
}
