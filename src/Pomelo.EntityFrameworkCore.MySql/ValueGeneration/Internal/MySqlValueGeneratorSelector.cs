using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Microsoft.EntityFrameworkCore.Utilities;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.ValueGeneration.Internal
{
    public class MySqlValueGeneratorSelector : RelationalValueGeneratorSelector
    {
        private readonly MySqlScopedTypeMapper _mySqlTypeMapper;

        public MySqlValueGeneratorSelector(
            [NotNull] IValueGeneratorCache cache,
            [NotNull] IRelationalAnnotationProvider relationalExtensions,
            [NotNull] IRelationalTypeMapper typeMapper)
            : base(cache, relationalExtensions)
        {
            _mySqlTypeMapper = typeMapper as MySqlScopedTypeMapper;
        }

        public override ValueGenerator Create(IProperty property, IEntityType entityType)
        {
            Check.NotNull(property, nameof(property));
            Check.NotNull(entityType, nameof(entityType));

            var ret = property.ClrType.UnwrapNullableType() == typeof(Guid)
                ? property.ValueGenerated == ValueGenerated.Never
                  || property.MySql().DefaultValueSql != null
                    ? (ValueGenerator)new TemporaryGuidValueGenerator()
                    : new MySqlSequentialGuidValueGenerator(_mySqlTypeMapper)
                : base.Create(property, entityType);
            return ret;
        }
    }
}
