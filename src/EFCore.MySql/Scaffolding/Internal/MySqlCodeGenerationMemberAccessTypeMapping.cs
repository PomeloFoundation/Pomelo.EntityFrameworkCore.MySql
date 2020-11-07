// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Storage;

namespace Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal
{
    internal class MySqlCodeGenerationMemberAccessTypeMapping : RelationalTypeMapping
    {
        private const string DummyStoreType = "clrOnly";

        public MySqlCodeGenerationMemberAccessTypeMapping()
            : base(new RelationalTypeMappingParameters(new CoreTypeMappingParameters(typeof(MySqlCodeGenerationMemberAccess)), DummyStoreType))
        {
        }

        protected MySqlCodeGenerationMemberAccessTypeMapping(RelationalTypeMappingParameters parameters)
            : base(parameters)
        {
        }

        protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
            => new MySqlCodeGenerationMemberAccessTypeMapping(parameters);

        public override string GenerateSqlLiteral(object value)
            => throw new InvalidOperationException("This type mapping exists for code generation only.");

        public override Expression GenerateCodeLiteral(object value)
            => value is MySqlCodeGenerationMemberAccess memberAccess
                ? Expression.MakeMemberAccess(null, memberAccess.MemberInfo)
                : null;
    }
}
