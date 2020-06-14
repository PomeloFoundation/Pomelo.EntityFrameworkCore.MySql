using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Pomelo.EntityFrameworkCore.MySql.Query.Internal;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Query.ExpressionTranslators.Internal
{
    public class MySqlArrayTranslator : IMethodCallTranslator, IMemberTranslator
    {
        private static readonly MethodInfo _enumerableAnyWithoutPredicate = typeof(Enumerable).GetTypeInfo()
            .GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Single(mi => mi.Name == nameof(Enumerable.Any) && mi.GetParameters().Length == 1);

        [NotNull] private readonly MySqlSqlExpressionFactory _sqlExpressionFactory;
        private readonly MySqlJsonPocoTranslator _jsonPocoTranslator;

        public MySqlArrayTranslator(
            [NotNull] MySqlSqlExpressionFactory sqlExpressionFactory,
            [NotNull] MySqlJsonPocoTranslator jsonPocoTranslator)
        {
            _sqlExpressionFactory = sqlExpressionFactory;
            _jsonPocoTranslator = jsonPocoTranslator;
        }

        [CanBeNull]
        public virtual SqlExpression Translate(SqlExpression instance, MethodInfo method, IReadOnlyList<SqlExpression> arguments)
        {
            if (instance != null && instance.Type.IsGenericList() && method.Name == "get_Item" && arguments.Count == 1)
            {
                // Try translating indexing inside json column
                return _jsonPocoTranslator.TranslateMemberAccess(instance, arguments[0], method.ReturnType);
            }

            // Predicate-less Any - translate to a simple length check.
            if (method.IsClosedFormOf(_enumerableAnyWithoutPredicate) &&
                arguments.Count == 1 &&
                arguments[0].Type.TryGetElementType(out _) &&
                arguments[0].TypeMapping is MySqlJsonTypeMapping)
            {
                return _sqlExpressionFactory.GreaterThan(
                    _jsonPocoTranslator.TranslateArrayLength(arguments[0]),
                    _sqlExpressionFactory.Constant(0));
            }

            return null;
        }

        public virtual SqlExpression Translate(SqlExpression instance, MemberInfo member, Type returnType)
        {
            if (instance?.Type.IsGenericList() == true &&
                member.Name == nameof(List<object>.Count) &&
                instance.TypeMapping is null)
            {
                return _jsonPocoTranslator.TranslateArrayLength(instance);
            }

            return null;
        }
    }
}
