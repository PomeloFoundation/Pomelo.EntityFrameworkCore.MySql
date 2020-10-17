// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json.Linq;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Pomelo.EntityFrameworkCore.MySql.Json.Newtonsoft.Storage.ValueComparison.Internal;
using Pomelo.EntityFrameworkCore.MySql.Json.Newtonsoft.Storage.ValueConversion.Internal;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Json.Newtonsoft.Storage.Internal
{
    public class MySqlJsonNewtonsoftTypeMappingSourcePlugin : MySqlJsonTypeMappingSourcePlugin
    {
        private static readonly Lazy<MySqlJsonNewtonsoftJTokenValueConverter> _jTokenValueConverter = new Lazy<MySqlJsonNewtonsoftJTokenValueConverter>();
        private static readonly Lazy<MySqlJsonNewtonsoftStringValueConverter> _jsonStringValueConverter = new Lazy<MySqlJsonNewtonsoftStringValueConverter>();

        public MySqlJsonNewtonsoftTypeMappingSourcePlugin(
            [NotNull] IMySqlOptions options)
            : base(options)
        {
        }

        protected override Type MySqlJsonTypeMappingType => typeof(MySqlJsonNewtonsoftTypeMapping<>);

        protected override RelationalTypeMapping FindDomMapping(RelationalTypeMappingInfo mappingInfo)
        {
            var clrType = mappingInfo.ClrType;

            if (typeof(JToken).IsAssignableFrom(clrType))
            {
                return (RelationalTypeMapping)Activator.CreateInstance(
                    MySqlJsonTypeMappingType.MakeGenericType(clrType),
                    "json",
                    GetValueConverter(clrType),
                    GetValueComparer(clrType),
                    Options);
            }

            return null;
        }

        protected override ValueConverter GetValueConverter(Type clrType)
        {
            if (typeof(JToken).IsAssignableFrom(clrType))
            {
                return _jTokenValueConverter.Value;
            }

            if (clrType == typeof(string))
            {
                return _jsonStringValueConverter.Value;
            }

            return (ValueConverter)Activator.CreateInstance(
                typeof(MySqlJsonNewtonsoftPocoValueConverter<>).MakeGenericType(clrType));
        }

        protected override ValueComparer GetValueComparer(Type clrType)
            => MySqlJsonNewtonsoftValueComparer.Create(clrType, Options.JsonChangeTrackingOptions);
    }
}
