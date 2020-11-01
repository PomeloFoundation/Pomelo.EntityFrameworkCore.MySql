using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using Pomelo.EntityFrameworkCore.MySql.Query.ExpressionTranslators.Internal;
using Pomelo.EntityFrameworkCore.MySql.Query.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Json.Newtonsoft.Query.Internal
{
    public class MySqlJsonNewtonsoftPocoTranslator : MySqlJsonPocoTranslator
    {
        public MySqlJsonNewtonsoftPocoTranslator(
            [NotNull] IRelationalTypeMappingSource typeMappingSource,
            [NotNull] ISqlExpressionFactory sqlExpressionFactory)
        : base(typeMappingSource, (MySqlSqlExpressionFactory)sqlExpressionFactory)
        {
        }

        public override string GetJsonPropertyName(MemberInfo member)
            => member.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName;
    }
}
