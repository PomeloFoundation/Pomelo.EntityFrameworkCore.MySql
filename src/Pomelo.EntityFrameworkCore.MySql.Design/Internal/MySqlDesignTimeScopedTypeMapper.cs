// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Data;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Scaffolding.Internal
{
    public class MySqlDesignTimeScopedTypeMapper : MySqlScopedTypeMapper
    {
        private static readonly RelationalTypeMapping TinyIntBoolean = new RelationalTypeMapping("tinyint(1)", typeof(bool), DbType.Boolean);

        public MySqlDesignTimeScopedTypeMapper(
            MySqlTypeMapper typeMapper)
            : base(typeMapper, null)
        {
        }

        protected override RelationalTypeMapping MaybeConvertMapping(RelationalTypeMapping mapping)
        {
            // TreatTinyIntAsBool
            if (ConnectionSettings.TreatTinyAsBoolean && (mapping.StoreType == "tinyint(1)" || mapping.StoreType == "tinyint(1) unsigned"))
                return TinyIntBoolean;

            return base.MaybeConvertMapping(mapping);
        }
    }
}
