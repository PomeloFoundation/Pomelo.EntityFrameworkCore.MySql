using System;
using System.Data;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;

namespace Pomelo.EntityFrameworkCore.MySql.Storage.Internal
{
    /// <summary>
    ///     <para>
    ///         Represents the mapping between a .NET <see cref="Guid" /> type and a database type.
    ///     </para>
    ///     <para>
    ///         This type is typically used by database providers (and other extensions). It is generally
    ///         not used in application code.
    ///     </para>
    /// </summary>
    public class MySqlGuidTypeMapping : GuidTypeMapping
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Microsoft.EntityFrameworkCore.Storage.GuidTypeMapping" /> class.
        /// </summary>
        /// <param name="storeType"> The name of the database type. </param>
        /// <param name="dbType"> The <see cref="DbType" /> to be used. </param>
        public MySqlGuidTypeMapping(
            [NotNull] string storeType,
            DbType? dbType = null,
            bool unicode = false,
            int? size = 0)
            : this(new RelationalTypeMappingParameters(
                new CoreTypeMappingParameters(typeof(Guid)),
                storeType,
                size != null
                    ? StoreTypePostfix.Size
                    : StoreTypePostfix.None,
                dbType,
                unicode,
                size,
                true))
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Microsoft.EntityFrameworkCore.Storage.GuidTypeMapping" /> class.
        /// </summary>
        /// <param name="parameters"> Parameter object for <see cref="RelationalTypeMapping" />. </param>
        protected MySqlGuidTypeMapping(RelationalTypeMappingParameters parameters)
            : base(parameters)
        {
        }

        /// <summary>
        ///     Creates a copy of this mapping.
        /// </summary>
        /// <param name="parameters"> The parameters for this mapping. </param>
        /// <returns> The newly created mapping. </returns>
        protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
            => new MySqlGuidTypeMapping(parameters);
    }
}
