// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Pomelo.EntityFrameworkCore.MySql.Query.ExpressionTranslators.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Query.Internal
{
    public class MySqlNetTopologySuiteMethodCallTranslatorPlugin : IMethodCallTranslatorPlugin
    {
        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public MySqlNetTopologySuiteMethodCallTranslatorPlugin(
            IRelationalTypeMappingSource typeMappingSource,
            ISqlExpressionFactory sqlExpressionFactory,
            IMySqlOptions options)
        {
            Translators = new IMethodCallTranslator[]
            {
                new MySqlGeometryMethodTranslator(typeMappingSource, sqlExpressionFactory, options),
                new MySqlGeometryCollectionMethodTranslator(typeMappingSource, sqlExpressionFactory),
                new MySqlLineStringMethodTranslator(typeMappingSource, sqlExpressionFactory),
                new MySqlPolygonMethodTranslator(typeMappingSource, sqlExpressionFactory),
                new MySqlSpatialDbFunctionsExtensionsMethodTranslator(typeMappingSource, sqlExpressionFactory, options)
            };
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual IEnumerable<IMethodCallTranslator> Translators { get; }
    }
}
