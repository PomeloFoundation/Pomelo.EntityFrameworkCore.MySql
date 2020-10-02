// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Data;
using System.Data.Common;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MySqlConnector;

namespace Pomelo.EntityFrameworkCore.MySql.Storage.Internal
{
    // TODO: Use as base class for all type mappings.
    /// <summary>
    /// The base class for mapping MySql-specific types. It configures parameters with the
    /// <see cref="MySqlDbType"/> provider-specific type enum.
    /// </summary>
    public abstract class MySqlTypeMapping : RelationalTypeMapping
    {
        /// <summary>
        /// The database type used by MySql.
        /// </summary>
        public virtual MySqlDbType MySqlDbType { get; }

        // ReSharper disable once PublicConstructorInAbstractClass
        public MySqlTypeMapping(
            [NotNull] string storeType,
            [NotNull] Type clrType,
            MySqlDbType mySqlDbType,
            DbType? dbType = null,
            bool unicode = false,
            int? size = null,
            ValueConverter valueConverter = null,
            ValueComparer valueComparer = null)
            : base(
                new RelationalTypeMappingParameters(
                    new CoreTypeMappingParameters(clrType, valueConverter, valueComparer), storeType, StoreTypePostfix.None, dbType, unicode, size))
            => MySqlDbType = mySqlDbType;

        /// <summary>
        /// Constructs an instance of the <see cref="MySqlTypeMapping"/> class.
        /// </summary>
        /// <param name="parameters">The parameters for this mapping.</param>
        /// <param name="mySqlDbType">The database type of the range subtype.</param>
        protected MySqlTypeMapping(RelationalTypeMappingParameters parameters, MySqlDbType mySqlDbType)
            : base(parameters)
            => MySqlDbType = mySqlDbType;

        protected override void ConfigureParameter(DbParameter parameter)
        {
            if (!(parameter is MySqlParameter mySqlParameter))
            {
                throw new ArgumentException($"MySql-specific type mapping {GetType()} being used with non-MySql parameter type {parameter.GetType().Name}");
            }

            base.ConfigureParameter(parameter);

            mySqlParameter.MySqlDbType = MySqlDbType;
        }
    }
}
