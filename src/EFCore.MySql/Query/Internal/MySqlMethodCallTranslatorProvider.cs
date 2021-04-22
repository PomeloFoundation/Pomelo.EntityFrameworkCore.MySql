// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Pomelo.EntityFrameworkCore.MySql.Query.ExpressionTranslators.Internal;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Query.Internal
{
    public class MySqlMethodCallTranslatorProvider : RelationalMethodCallTranslatorProvider
    {
        public MySqlMethodCallTranslatorProvider(
            [NotNull] RelationalMethodCallTranslatorProviderDependencies dependencies,
            [NotNull] IMySqlOptions options)
            : base(dependencies)
        {
            var sqlExpressionFactory = (MySqlSqlExpressionFactory)dependencies.SqlExpressionFactory;
            var relationalTypeMappingSource = (MySqlTypeMappingSource)dependencies.RelationalTypeMappingSource;

            AddTranslators(new IMethodCallTranslator[]
            {
                new MySqlByteArrayMethodTranslator(sqlExpressionFactory),
                new MySqlConvertTranslator(sqlExpressionFactory),
                new MySqlDateTimeMethodTranslator(sqlExpressionFactory),
                new MySqlDateDiffFunctionsTranslator(sqlExpressionFactory),
                new MySqlDbFunctionsExtensionsMethodTranslator(sqlExpressionFactory),
                new MySqlJsonDbFunctionsTranslator(sqlExpressionFactory),
                new MySqlMathMethodTranslator(sqlExpressionFactory),
                new MySqlNewGuidTranslator(sqlExpressionFactory),
                new MySqlObjectToStringTranslator(sqlExpressionFactory),
                new MySqlRegexIsMatchTranslator(sqlExpressionFactory),
                new MySqlStringComparisonMethodTranslator(sqlExpressionFactory, options),
                new MySqlStringMethodTranslator(sqlExpressionFactory, relationalTypeMappingSource, options),
            });
        }
    }
}
