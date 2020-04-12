using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Pomelo.EntityFrameworkCore.MySql.Query.ExpressionTranslators.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Query.Internal
{
    public class MySqlMethodCallTranslatorProvider : RelationalMethodCallTranslatorProvider
    {
        public MySqlMethodCallTranslatorProvider(
            [NotNull] RelationalMethodCallTranslatorProviderDependencies dependencies,
            [NotNull] IMySqlOptions options)
            : base(dependencies)
        {
            var sqlExpressionFactory = dependencies.SqlExpressionFactory;

            AddTranslators(new IMethodCallTranslator[]
            {
                new MySqlConvertTranslator(sqlExpressionFactory),
                new MySqlDateTimeMethodTranslator(sqlExpressionFactory),
                new MySqlDateDiffFunctionsTranslator(sqlExpressionFactory),
                new MySqlMathMethodTranslator(sqlExpressionFactory),
                new MySqlNewGuidTranslator(sqlExpressionFactory),
                new MySqlObjectToStringTranslator(sqlExpressionFactory),
                new MySqlStringMethodTranslator(sqlExpressionFactory),
                new MySqlStringComparisonMethodTranslator(sqlExpressionFactory),
                new MySqlRegexIsMatchTranslator(sqlExpressionFactory),
                new MySqlDbFunctionsExtensionsMethodTranslator(sqlExpressionFactory, options),
            });
        }
    }
}
