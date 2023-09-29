// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore.Query;

namespace Pomelo.EntityFrameworkCore.MySql.Query.ExpressionVisitors.Internal;

public class MySqlQueryTranslationPreprocessorFactory : IQueryTranslationPreprocessorFactory
{
    private readonly QueryTranslationPreprocessorDependencies _dependencies;
    private readonly RelationalQueryTranslationPreprocessorDependencies _relationalDependencies;

    public MySqlQueryTranslationPreprocessorFactory(
        QueryTranslationPreprocessorDependencies dependencies,
        RelationalQueryTranslationPreprocessorDependencies relationalDependencies)
    {
        _dependencies = dependencies;
        _relationalDependencies = relationalDependencies;
    }

    public virtual QueryTranslationPreprocessor Create(QueryCompilationContext queryCompilationContext)
        => new MySqlQueryTranslationPreprocessor(
            _dependencies,
            _relationalDependencies,
            queryCompilationContext);
}
