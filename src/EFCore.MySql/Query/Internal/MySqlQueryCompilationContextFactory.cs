// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Pomelo.EntityFrameworkCore.MySql.Query.Internal
{
    public class MySqlQueryCompilationContextFactory : IQueryCompilationContextFactory
    {
        private readonly QueryCompilationContextDependencies _dependencies;
        private readonly RelationalQueryCompilationContextDependencies _relationalDependencies;

        public MySqlQueryCompilationContextFactory(
            [NotNull] QueryCompilationContextDependencies dependencies,
            [NotNull] RelationalQueryCompilationContextDependencies relationalDependencies)
        {
            Check.NotNull(dependencies, nameof(dependencies));
            Check.NotNull(relationalDependencies, nameof(relationalDependencies));

            _dependencies = dependencies;
            _relationalDependencies = relationalDependencies;
        }

        public virtual QueryCompilationContext Create(bool async)
            => new MySqlQueryCompilationContext(_dependencies, _relationalDependencies, async);

        public virtual QueryCompilationContext CreatePrecompiled(bool async, IReadOnlySet<string> nonNullableReferenceTypeParameters)
            => new MySqlQueryCompilationContext(
                _dependencies, _relationalDependencies, async, precompiling: true, nonNullableReferenceTypeParameters);
    }
}
