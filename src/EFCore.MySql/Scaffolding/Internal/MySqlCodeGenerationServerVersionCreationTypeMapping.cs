// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal
{
    internal class MySqlCodeGenerationServerVersionCreationTypeMapping : RelationalTypeMapping
    {
        private const string DummyStoreType = "clrOnly";

        public MySqlCodeGenerationServerVersionCreationTypeMapping()
            : base(new RelationalTypeMappingParameters(new CoreTypeMappingParameters(typeof(MySqlCodeGenerationServerVersionCreation)), DummyStoreType))
        {
        }

        protected MySqlCodeGenerationServerVersionCreationTypeMapping(RelationalTypeMappingParameters parameters)
            : base(parameters)
        {
        }

        protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
            => new MySqlCodeGenerationServerVersionCreationTypeMapping(parameters);

        public override string GenerateSqlLiteral(object value)
            => throw new InvalidOperationException("This type mapping exists for code generation only.");

        public override Expression GenerateCodeLiteral(object value)
            => value is MySqlCodeGenerationServerVersionCreation serverVersionCreation
                ? Expression.Call(
                    typeof(ServerVersion).GetMethod(nameof(ServerVersion.Parse), new[] {typeof(string)}),
                    Expression.Constant(serverVersionCreation.ServerVersion.ToString()))
                : null;
    }
}
