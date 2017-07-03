// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.Sql;

namespace Microsoft.EntityFrameworkCore.TestUtilities
{
    public class TestQuerySqlGeneratorFactory : QuerySqlGeneratorFactoryBase
    {
        public TestQuerySqlGeneratorFactory(QuerySqlGeneratorDependencies dependencies)
            : base(dependencies)
        {
        }

        public override IQuerySqlGenerator CreateDefault(SelectExpression selectExpression)
            => new TestQuerySqlGenerator(Dependencies, selectExpression);
    }
}
