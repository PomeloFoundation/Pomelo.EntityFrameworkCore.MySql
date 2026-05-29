// Copyright (c) Microting. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Microting.EntityFrameworkCore.MySql.Query.ExpressionTranslators.Internal;
using Microting.EntityFrameworkCore.MySql.Query.Internal;

namespace Microting.EntityFrameworkCore.MySql.Json.Newtonsoft.Query.Internal
{
    public class MySqlJsonNewtonsoftMemberTranslatorPlugin : IMemberTranslatorPlugin
    {
        public MySqlJsonNewtonsoftMemberTranslatorPlugin(
            IRelationalTypeMappingSource typeMappingSource,
            ISqlExpressionFactory sqlExpressionFactory,
            IMySqlJsonPocoTranslator jsonPocoTranslator)
        {
            var mySqlSqlExpressionFactory = (MySqlSqlExpressionFactory)sqlExpressionFactory;
            var mySqlJsonPocoTranslator = (MySqlJsonPocoTranslator)jsonPocoTranslator;

            Translators = new IMemberTranslator[]
            {
                new MySqlJsonNewtonsoftDomTranslator(
                    mySqlSqlExpressionFactory,
                    typeMappingSource,
                    mySqlJsonPocoTranslator),
                jsonPocoTranslator,
            };
        }

        public virtual IEnumerable<IMemberTranslator> Translators { get; }
    }
}
