// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using NetTopologySuite.Geometries;

namespace Pomelo.EntityFrameworkCore.MySql.Query.Internal
{
    public class MySqlLineStringMethodTranslator : IMethodCallTranslator
    {
        private static readonly MethodInfo _getPointN = typeof(LineString).GetRuntimeMethod(
            nameof(LineString.GetPointN), new[] { typeof(int) });

        private readonly IRelationalTypeMappingSource _typeMappingSource;
        private readonly ISqlExpressionFactory _sqlExpressionFactory;

        public MySqlLineStringMethodTranslator(
            IRelationalTypeMappingSource typeMappingSource,
            ISqlExpressionFactory sqlExpressionFactory)
        {
            _typeMappingSource = typeMappingSource;
            _sqlExpressionFactory = sqlExpressionFactory;
        }

        public virtual SqlExpression Translate(SqlExpression instance, MethodInfo method, IReadOnlyList<SqlExpression> arguments)
        {
            if (Equals(method, _getPointN))
            {
                return _sqlExpressionFactory.Function(
                    "ST_PointN",
                    new[]
                    {
                        instance,
                        _sqlExpressionFactory.Add(
                            arguments[0],
                            _sqlExpressionFactory.Constant(1))
                    },
                    method.ReturnType,
                    _typeMappingSource.FindMapping(method.ReturnType, instance.TypeMapping.StoreType));
            }

            return null;
        }
    }
}
