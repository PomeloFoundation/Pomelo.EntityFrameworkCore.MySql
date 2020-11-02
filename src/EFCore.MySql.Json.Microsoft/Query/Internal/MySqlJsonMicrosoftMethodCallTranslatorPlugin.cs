// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Pomelo.EntityFrameworkCore.MySql.Query.ExpressionTranslators.Internal;
using Pomelo.EntityFrameworkCore.MySql.Query.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Json.Microsoft.Query.Internal
{
    public class MySqlJsonMicrosoftMethodCallTranslatorPlugin : IMethodCallTranslatorPlugin
    {
        public MySqlJsonMicrosoftMethodCallTranslatorPlugin(
            IRelationalTypeMappingSource typeMappingSource,
            ISqlExpressionFactory sqlExpressionFactory,
            IMySqlJsonPocoTranslator jsonPocoTranslator)
        {
            var mySqlSqlExpressionFactory = (MySqlSqlExpressionFactory)sqlExpressionFactory;
            var mySqlJsonPocoTranslator = (MySqlJsonPocoTranslator)jsonPocoTranslator;

            Translators = new IMethodCallTranslator[]
            {
                new MySqlJsonMicrosoftDomTranslator(
                    mySqlSqlExpressionFactory,
                    typeMappingSource,
                    mySqlJsonPocoTranslator),
            };
        }

        public virtual IEnumerable<IMethodCallTranslator> Translators { get; }
    }
}
