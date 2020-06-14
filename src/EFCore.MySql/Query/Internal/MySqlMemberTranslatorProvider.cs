using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Pomelo.EntityFrameworkCore.MySql.Query.ExpressionTranslators.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Query.Internal
{
    public class MySqlMemberTranslatorProvider : RelationalMemberTranslatorProvider
    {
        public virtual MySqlJsonPocoTranslator JsonPocoTranslator { get; }

        public MySqlMemberTranslatorProvider([NotNull] RelationalMemberTranslatorProviderDependencies dependencies,
            [NotNull] IRelationalTypeMappingSource typeMappingSource)
            : base(dependencies)
        {
            var sqlExpressionFactory = (MySqlSqlExpressionFactory)dependencies.SqlExpressionFactory;
            JsonPocoTranslator = new MySqlJsonPocoTranslator(typeMappingSource, sqlExpressionFactory);

            AddTranslators(
                new IMemberTranslator[] {
                    new MySqlDateTimeMemberTranslator(sqlExpressionFactory),
                    new MySqlStringMemberTranslator(sqlExpressionFactory),
                    new MySqlJsonDomTranslator(sqlExpressionFactory, typeMappingSource),
                    JsonPocoTranslator,
                });
        }
    }
}
