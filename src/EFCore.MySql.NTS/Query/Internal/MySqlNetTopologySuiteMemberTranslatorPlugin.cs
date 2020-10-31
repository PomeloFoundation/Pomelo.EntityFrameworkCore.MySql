// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Query.Internal
{
    public class MySqlNetTopologySuiteMemberTranslatorPlugin : IMemberTranslatorPlugin
    {
        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public MySqlNetTopologySuiteMemberTranslatorPlugin(
            IRelationalTypeMappingSource typeMappingSource,
            ISqlExpressionFactory sqlExpressionFactory,
            IMySqlOptions options)
        {
            var mySqlSqlExpressionFactory = (MySqlSqlExpressionFactory)sqlExpressionFactory;

            Translators = new IMemberTranslator[]
            {
                new MySqlGeometryMemberTranslator(typeMappingSource, mySqlSqlExpressionFactory),
                new MySqlGeometryCollectionMemberTranslator(mySqlSqlExpressionFactory),
                new MySqlLineStringMemberTranslator(typeMappingSource, mySqlSqlExpressionFactory, options),
                new MySqlMultiLineStringMemberTranslator(mySqlSqlExpressionFactory),
                new MySqlPointMemberTranslator(mySqlSqlExpressionFactory),
                new MySqlPolygonMemberTranslator(typeMappingSource, mySqlSqlExpressionFactory)
            };
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual IEnumerable<IMemberTranslator> Translators { get; }
    }
}
