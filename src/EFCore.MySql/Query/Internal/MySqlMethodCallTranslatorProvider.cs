using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query;

namespace Pomelo.EntityFrameworkCore.MySql.Query.Internal
{
    public class MySqlMethodCallTranslatorProvider : RelationalMethodCallTranslatorProvider
    {
        public MySqlMethodCallTranslatorProvider([NotNull] RelationalMethodCallTranslatorProviderDependencies dependencies)
            : base(dependencies)
        {
            var sqlExpressionFactory = dependencies.SqlExpressionFactory;

            AddTranslators(new IMethodCallTranslator[]
            {
                new MySqlContainsOptimizedTranslator(sqlExpressionFactory),
                new MySqlConvertTranslator(sqlExpressionFactory),
                new MySqlDateTimeMethodTranslator(sqlExpressionFactory),
                new MySqlDateDiffFunctionsTranslator(sqlExpressionFactory),
                new MySqlMathTranslator(sqlExpressionFactory),
                new MySqlNewGuidTranslator(sqlExpressionFactory),
                new MySqlObjectToStringTranslator(sqlExpressionFactory),
                new MySqlStringMethodTranslator(sqlExpressionFactory),
            });
        }
    }
}
