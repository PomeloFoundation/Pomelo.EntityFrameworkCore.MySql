using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.MySql.Query.ExpressionTranslators.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Query.Internal
{
    public class MySqlMemberTranslatorProvider : RelationalMemberTranslatorProvider
    {
        //public virtual MySqlJsonPocoTranslator JsonPocoTranslator { get; }

        public MySqlMemberTranslatorProvider([NotNull] RelationalMemberTranslatorProviderDependencies dependencies)
            : base(dependencies)
        {
            var sqlExpressionFactory = (MySqlSqlExpressionFactory)dependencies.SqlExpressionFactory;

            AddTranslators(
                new IMemberTranslator[] {
                    new MySqlDateTimeMemberTranslator(sqlExpressionFactory),
                    new MySqlStringMemberTranslator(sqlExpressionFactory),
                });
        }
    }
}
