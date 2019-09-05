using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query;

namespace Pomelo.EntityFrameworkCore.MySql.Query.Internal
{
    public class MySqlMemberTranslatorProvider : RelationalMemberTranslatorProvider
    {
        public MySqlMemberTranslatorProvider([NotNull] RelationalMemberTranslatorProviderDependencies dependencies)
            : base(dependencies)
        {
            var sqlExpressionFactory = dependencies.SqlExpressionFactory;

            AddTranslators(
                new IMemberTranslator[] {
                    new MySqlDateTimeMemberTranslator(sqlExpressionFactory),
                    new MySqlStringMemberTranslator(sqlExpressionFactory)
                });
        }
    }
}
