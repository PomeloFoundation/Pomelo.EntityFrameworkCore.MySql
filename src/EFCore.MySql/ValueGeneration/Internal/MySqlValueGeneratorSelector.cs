// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Utilities;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Pomelo.EntityFrameworkCore.MySql.ValueGeneration.Internal
{
    public class MySqlValueGeneratorSelector : RelationalValueGeneratorSelector
    {
        private readonly IMySqlOptions _options;

        public MySqlValueGeneratorSelector(
            [NotNull] ValueGeneratorSelectorDependencies dependencies,
            [NotNull] IMySqlOptions options)
            : base(dependencies)
        {
            _options = options;
        }


        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        protected override ValueGenerator FindForType(IProperty property, ITypeBase typeBase, Type clrType)
        {
            var ret = clrType == typeof(Guid)
                ? property.ValueGenerated == ValueGenerated.Never
                  || property.GetDefaultValueSql() != null
                    ? new TemporaryGuidValueGenerator()
                    : new MySqlSequentialGuidValueGenerator(_options)
                : base.FindForType(property, typeBase, clrType);
            return ret;
        }
    }
}
