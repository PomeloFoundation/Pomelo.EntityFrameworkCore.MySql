using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace Pomelo.EntityFrameworkCore.MySql.Query.Internal
{
    public class MySqlConvertTranslator : IMethodCallTranslator
    {
        // https://www.w3schools.com/sql/func_mysql_convert.asp
        private static readonly Dictionary<string, string> _typeMapping = new Dictionary<string, string>
        {

            [nameof(Convert.ToByte)] = "signed",
            [nameof(Convert.ToDecimal)] = "decimal",
            [nameof(Convert.ToDouble)] = "decimal",
            [nameof(Convert.ToInt16)] = "signed",
            [nameof(Convert.ToInt32)] = "signed",
            [nameof(Convert.ToInt64)] = "signed",
            [nameof(Convert.ToString)] = "char",

            [nameof(Convert.ToUInt16)] = "unsigned",
            [nameof(Convert.ToUInt32)] = "unsigned",
            [nameof(Convert.ToUInt64)] = "unsigned",
            [nameof(Convert.ToDateTime)] = "DATETIME"
        };

        private static readonly List<Type> _supportedTypes = new List<Type>
        {
            typeof(byte),
            typeof(DateTime),
            typeof(decimal),
            typeof(double),
            typeof(float),
            typeof(int),
            typeof(long),
            typeof(short),
            typeof(string),

            typeof(uint),
            typeof(ulong),
            typeof(ushort),
        };

        private static readonly IEnumerable<MethodInfo> _supportedMethods
            = _typeMapping.Keys
                .SelectMany(
                    t => typeof(Convert).GetTypeInfo().GetDeclaredMethods(t)
                        .Where(
                            m => m.GetParameters().Length == 1
                                 && _supportedTypes.Contains(m.GetParameters().First().ParameterType)));
        private readonly ISqlExpressionFactory _sqlExpressionFactory;

        public MySqlConvertTranslator(ISqlExpressionFactory sqlExpressionFactory)
        {
            _sqlExpressionFactory = sqlExpressionFactory;
        }

        public virtual SqlExpression Translate(SqlExpression instance, MethodInfo method, IReadOnlyList<SqlExpression> arguments)
        {
            return _supportedMethods.Contains(method)
                ? _sqlExpressionFactory.Function(
                    "CONVERT",
                    new[]
                    {
                        arguments[0],
                        _sqlExpressionFactory.Fragment(_typeMapping[method.Name])
                    },
                    method.ReturnType)
                : null;
        }
    }
}
