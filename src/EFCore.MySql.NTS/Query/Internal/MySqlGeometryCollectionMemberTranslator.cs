// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using NetTopologySuite.Geometries;

namespace Pomelo.EntityFrameworkCore.MySql.Query.Internal
{
    public class MySqlGeometryCollectionMemberTranslator : IMemberTranslator
    {
        private static readonly MemberInfo _count = typeof(GeometryCollection).GetRuntimeProperty(nameof(GeometryCollection.Count));
        private readonly ISqlExpressionFactory _sqlExpressionFactory;

        public MySqlGeometryCollectionMemberTranslator(ISqlExpressionFactory sqlExpressionFactory)
        {
            _sqlExpressionFactory = sqlExpressionFactory;
        }

        public virtual SqlExpression Translate(SqlExpression instance, MemberInfo member, Type returnType)
        {
            if (Equals(member, _count))
            {
                return _sqlExpressionFactory.Function(
                    "ST_NumGeometries",
                    new [] {instance},
                    returnType);
            }

            return null;
        }
    }
}
