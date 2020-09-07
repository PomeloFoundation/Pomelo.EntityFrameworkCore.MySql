using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Storage.Internal
{
    public abstract class MySqlJsonTypeMappingSourcePlugin
        : IRelationalTypeMappingSourcePlugin
    {
        [NotNull]
        public IMySqlOptions Options { get; }

        protected MySqlJsonTypeMappingSourcePlugin(
            [NotNull] IMySqlOptions options)
        {
            Options = options;
        }

        public virtual RelationalTypeMapping FindMapping(in RelationalTypeMappingInfo mappingInfo)
        {
            var clrType = mappingInfo.ClrType;
            var storeTypeName = mappingInfo.StoreTypeName;

            if (storeTypeName != null)
            {
                clrType ??= typeof(string);
                return storeTypeName.Equals("json", StringComparison.OrdinalIgnoreCase)
                    ? (RelationalTypeMapping)Activator.CreateInstance(
                        MySqlJsonTypeMappingType.MakeGenericType(clrType),
                        storeTypeName,
                        GetValueConverter(clrType),
                        Options)
                    : null;
            }

            return FindDomMapping(mappingInfo);
        }

        protected abstract Type MySqlJsonTypeMappingType { get; }
        protected abstract RelationalTypeMapping FindDomMapping(RelationalTypeMappingInfo mappingInfo);
        protected abstract ValueConverter GetValueConverter(Type clrType);
    }
}
