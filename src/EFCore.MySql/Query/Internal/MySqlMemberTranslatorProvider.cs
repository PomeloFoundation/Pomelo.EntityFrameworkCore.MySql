﻿// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;

namespace Pomelo.EntityFrameworkCore.MySql.Query.Internal
{
    public class MySqlMemberTranslatorProvider : RelationalMemberTranslatorProvider
    {
        public MySqlMemberTranslatorProvider(
            [NotNull] RelationalMemberTranslatorProviderDependencies dependencies,
            IRelationalTypeMappingSource typeMappingSource)
            : base(dependencies)
        {
            var sqlExpressionFactory = (MySqlSqlExpressionFactory)dependencies.SqlExpressionFactory;

            AddTranslators(
                new IMemberTranslator[] {
                    new MySqlDateTimeMemberTranslator(sqlExpressionFactory),
                    new MySqlStringMemberTranslator(sqlExpressionFactory),
                    new MySqlTimeSpanMemberTranslator(sqlExpressionFactory, typeMappingSource),
                });
        }
    }
}
