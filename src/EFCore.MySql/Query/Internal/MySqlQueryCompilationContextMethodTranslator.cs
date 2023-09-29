// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace Pomelo.EntityFrameworkCore.MySql.Query.Internal;

public abstract class MySqlQueryCompilationContextMethodTranslator : IMethodCallTranslator
{
    private readonly Func<QueryCompilationContext> _queryCompilationContextResolver;

    protected MySqlQueryCompilationContextMethodTranslator(Func<QueryCompilationContext> queryCompilationContextResolver)
    {
        _queryCompilationContextResolver = queryCompilationContextResolver;
    }

    public virtual SqlExpression Translate(
        SqlExpression instance,
        MethodInfo method,
        IReadOnlyList<SqlExpression> arguments,
        IDiagnosticsLogger<DbLoggerCategory.Query> logger)
        => Translate(instance, method, arguments, _queryCompilationContextResolver() ?? throw new InvalidOperationException());

    public abstract SqlExpression Translate(
        SqlExpression instance,
        MethodInfo method,
        IReadOnlyList<SqlExpression> arguments,
        QueryCompilationContext queryCompilationContext);
}
