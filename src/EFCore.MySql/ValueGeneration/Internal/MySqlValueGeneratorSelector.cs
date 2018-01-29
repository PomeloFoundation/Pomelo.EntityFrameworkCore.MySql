using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Utilities;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.ValueGeneration.Internal
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

        public override ValueGenerator Create(IProperty property, IEntityType entityType)
        {
            Check.NotNull(property, nameof(property));
            Check.NotNull(entityType, nameof(entityType));

            var ret = property.ClrType.UnwrapNullableType() == typeof(Guid)
                ? property.ValueGenerated == ValueGenerated.Never
                  || property.MySql().DefaultValueSql != null
                    ? (ValueGenerator)new TemporaryGuidValueGenerator()
                    : new MySqlSequentialGuidValueGenerator(_options)
                : base.Create(property, entityType);
            return ret;
        }
    }
}
