// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.Sql;

namespace Microsoft.EntityFrameworkCore.TestUtilities
{
    public class TestQuerySqlGenerator : DefaultQuerySqlGenerator
    {
        public TestQuerySqlGenerator(
            QuerySqlGeneratorDependencies dependencies,
            SelectExpression selectExpression)
            : base(dependencies, selectExpression)
        {
        }
    }
}
