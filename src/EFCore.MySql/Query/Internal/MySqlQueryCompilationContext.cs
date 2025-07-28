// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query;

namespace Pomelo.EntityFrameworkCore.MySql.Query.Internal
{
    public class MySqlQueryCompilationContext : RelationalQueryCompilationContext
    {
        public MySqlQueryCompilationContext(
            [NotNull] QueryCompilationContextDependencies dependencies,
            [NotNull] RelationalQueryCompilationContextDependencies relationalDependencies,
            bool async)
            : base(dependencies, relationalDependencies, async)
        {
        }

        public MySqlQueryCompilationContext(
            [NotNull] QueryCompilationContextDependencies dependencies,
            [NotNull] RelationalQueryCompilationContextDependencies relationalDependencies,
            bool async,
            bool precompiling,
            IReadOnlySet<string> nonNullableReferenceTypeParameters)
            : base(dependencies, relationalDependencies, async, precompiling, nonNullableReferenceTypeParameters)
        {
        }

        public override bool IsBuffering
            => base.IsBuffering ||
               QuerySplittingBehavior == Microsoft.EntityFrameworkCore.QuerySplittingBehavior.SplitQuery;

        /// <inheritdoc />
        public override bool SupportsPrecompiledQuery => false;
    }
}
