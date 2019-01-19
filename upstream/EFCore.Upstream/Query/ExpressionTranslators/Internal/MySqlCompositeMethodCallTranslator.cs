// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;

namespace Pomelo.EntityFrameworkCore.MySql.Query.ExpressionTranslators.Internal
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
            new MySqlDateDiffTranslator(),
            new MySqlEndsWithOptimizedTranslator(),
            new MySqlFullTextSearchMethodCallTranslator(),
            new MySqlMathTranslator(),
            new MySqlNewGuidTranslator(),
            new MySqlObjectToStringTranslator(),
            new MySqlStartsWithOptimizedTranslator(),
            new MySqlStringIsNullOrWhiteSpaceTranslator(),
            new MySqlStringReplaceTranslator(),
            new MySqlStringSubstringTranslator(),
            new MySqlStringToLowerTranslator(),
            new MySqlStringToUpperTranslator(),
            new MySqlStringTrimEndTranslator(),
            new MySqlStringTrimStartTranslator(),
            new MySqlStringTrimTranslator(),
            new MySqlStringIndexOfTranslator(),
            new MySqlStringConcatMethodCallTranslator()
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
