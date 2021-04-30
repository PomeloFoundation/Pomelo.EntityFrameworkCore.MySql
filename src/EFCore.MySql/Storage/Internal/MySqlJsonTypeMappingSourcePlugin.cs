// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Storage.Internal
{
    public abstract class MySqlJsonTypeMappingSourcePlugin
        : IRelationalTypeMappingSourcePlugin
    {
        [NotNull]
        public virtual IMySqlOptions Options { get; }

        protected MySqlJsonTypeMappingSourcePlugin(
            [NotNull] IMySqlOptions options)
        {
            Options = options;
        }

        public virtual RelationalTypeMapping FindMapping(in RelationalTypeMappingInfo mappingInfo)
        {
            var clrType = mappingInfo.ClrType;
            var storeTypeName = mappingInfo.StoreTypeName;

            if (clrType == typeof(MySqlJsonString))
            {
                clrType = typeof(string);
                storeTypeName = "json";
            }

            if (storeTypeName != null)
            {
                clrType ??= typeof(string);
                return storeTypeName.Equals("json", StringComparison.OrdinalIgnoreCase)
                    ? (RelationalTypeMapping)Activator.CreateInstance(
                        MySqlJsonTypeMappingType.MakeGenericType(clrType),
                        storeTypeName,
                        GetValueConverter(clrType),
                        GetValueComparer(clrType),
                        Options)
                    : null;
            }

            return FindDomMapping(mappingInfo);
        }

        protected abstract Type MySqlJsonTypeMappingType { get; }
        protected abstract RelationalTypeMapping FindDomMapping(RelationalTypeMappingInfo mappingInfo);
        protected abstract ValueConverter GetValueConverter(Type clrType);
        protected abstract ValueComparer GetValueComparer(Type clrType);
    }
}
