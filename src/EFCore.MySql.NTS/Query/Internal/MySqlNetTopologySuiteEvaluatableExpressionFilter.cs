using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Pomelo.EntityFrameworkCore.MySql.Query.Internal
{
    public class MySqlNetTopologySuiteEvaluatableExpressionFilter : IMySqlEvaluatableExpressionFilter
    {
        public virtual bool? IsEvaluatableExpression(Expression expression, IModel model)
        {
            if (expression is MethodCallExpression methodCallExpression &&
                methodCallExpression.Method.DeclaringType == typeof(MySqlSpatialDbFunctionsExtensions))
            {
                return false;
            }

            return null;
        }
    }
}
