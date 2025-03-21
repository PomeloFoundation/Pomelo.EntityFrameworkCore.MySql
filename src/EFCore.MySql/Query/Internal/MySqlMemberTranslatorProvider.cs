﻿// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Query.Internal
{
    public class MySqlMemberTranslatorProvider : RelationalMemberTranslatorProvider
    {
        public MySqlMemberTranslatorProvider([NotNull] RelationalMemberTranslatorProviderDependencies dependencies, IMySqlOptions mySqlOptions)
            : base(dependencies)
        {
            var sqlExpressionFactory = (MySqlSqlExpressionFactory)dependencies.SqlExpressionFactory;

            AddTranslators(
                new IMemberTranslator[] {
                    new MySqlDateTimeMemberTranslator(sqlExpressionFactory, mySqlOptions),
                    new MySqlStringMemberTranslator(sqlExpressionFactory),
                    new MySqlTimeSpanMemberTranslator(sqlExpressionFactory),
                });
        }
    }
}
