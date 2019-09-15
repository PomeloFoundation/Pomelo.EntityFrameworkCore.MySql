using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
namespace Pomelo.EntityFrameworkCore.MySql.Query.Internal
{
    public class MySqlDateTimeMemberTranslator : IMemberTranslator
    {
        private static readonly Dictionary<string, (string Part, int Divisor)> _datePartMapping
            = new Dictionary<string, (string, int)>
            {
                { nameof(DateTime.Year), ("year", 1) },
                { nameof(DateTime.Month), ("month", 1) },
                { nameof(DateTime.Day), ("day", 1) },
                { nameof(DateTime.Hour), ("hour", 1) },
                { nameof(DateTime.Minute), ("minute", 1) },
                { nameof(DateTime.Second), ("second", 1) },
                { nameof(DateTime.Millisecond), ("microsecond", 1000) },
            };
        private readonly MySqlSqlExpressionFactory _sqlExpressionFactory;

        public MySqlDateTimeMemberTranslator(ISqlExpressionFactory sqlExpressionFactory)
        {
            _sqlExpressionFactory = (MySqlSqlExpressionFactory)sqlExpressionFactory;
        }

        public virtual SqlExpression Translate(SqlExpression instance, MemberInfo member, Type returnType)
        {
            var declaringType = member.DeclaringType;

            if (declaringType == typeof(DateTime)
                || declaringType == typeof(DateTimeOffset))
            {
                var memberName = member.Name;

                if (_datePartMapping.TryGetValue(memberName, out var datePart))
                {
                    var extract = _sqlExpressionFactory.Function(
                        "EXTRACT",
                        new[]
                        {
                            _sqlExpressionFactory.ComplexFunctionArgument(
                                new [] {
                                    _sqlExpressionFactory.Fragment($"{datePart.Part} FROM"),
                                    instance
                                },
                                typeof(string))
                        },
                        returnType);

                    if (datePart.Divisor != 1)
                    {
                        return _sqlExpressionFactory.Divide(
                            extract,
                            _sqlExpressionFactory.Constant(datePart.Divisor));
                    }

                    return extract;
                }

                switch (memberName)
                {
                    case nameof(DateTime.DayOfYear):
                        return _sqlExpressionFactory.Function(
                        "DAYOFYEAR",
                        new[] { instance },
                        returnType,
                        instance.TypeMapping);

                    case nameof(DateTime.Date):
                        return _sqlExpressionFactory.Function(
                            "CONVERT",
                            new[]{
                                instance,
                                _sqlExpressionFactory.Fragment("date")
                            },
                            returnType,
                            instance.TypeMapping);

                    case nameof(DateTime.TimeOfDay):
                        return _sqlExpressionFactory.Convert(instance, returnType);

                    case nameof(DateTime.Now) when declaringType == typeof(DateTime):
                        return _sqlExpressionFactory.Function("CURRENT_TIMESTAMP", returnType);

                    case nameof(DateTime.UtcNow) when declaringType == typeof(DateTime):
                        return _sqlExpressionFactory.Function("UTC_TIMESTAMP", returnType);

                    case nameof(DateTime.Today) when declaringType == typeof(DateTime):
                        return _sqlExpressionFactory.Function("CURDATE", returnType);
                }
            }

            return null;
        }
    }
}
