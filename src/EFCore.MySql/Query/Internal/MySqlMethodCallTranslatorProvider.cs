using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Pomelo.EntityFrameworkCore.MySql.Query.ExpressionTranslators.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Query.Internal
{
    public class MySqlMethodCallTranslatorProvider : RelationalMethodCallTranslatorProvider
    {
        public MySqlMethodCallTranslatorProvider(
            [NotNull] RelationalMethodCallTranslatorProviderDependencies dependencies,
            [NotNull] IRelationalTypeMappingSource typeMappingSource,
            [NotNull] IMySqlOptions options)
            : base(dependencies)
        {
            var sqlExpressionFactory = (MySqlSqlExpressionFactory)dependencies.SqlExpressionFactory;
            var jsonTranslator = new MySqlJsonPocoTranslator(typeMappingSource, sqlExpressionFactory);

            AddTranslators(new IMethodCallTranslator[]
            {
                new MySqlArrayTranslator(sqlExpressionFactory, jsonTranslator),
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
                new MySqlJsonDomTranslator(sqlExpressionFactory, typeMappingSource),
                new MySqlJsonDbFunctionsTranslator(sqlExpressionFactory, typeMappingSource)
            });
        }
    }
}
