// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using JetBrains.Annotations;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    /// Represents a <see cref="ServerVersion"/> for MySQL database servers.
    /// For MariaDB database servers, use <see cref="MariaDbServerVersion"/> instead.
    /// </summary>
    public class MySqlServerVersion : ServerVersion
    {
        public static readonly string MySqlTypeIdentifier = nameof(ServerType.MySql).ToLowerInvariant();
        public static readonly ServerVersion LatestSupportedServerVersion = new MySqlServerVersion(new Version(8, 0, 21));

        public override ServerVersionSupport Supports { get; }

        public MySqlServerVersion(Version version)
            : base(version, ServerType.MySql)
        {
            Supports = new MySqlServerVersionSupport(this);
        }

        public MySqlServerVersion(string versionString)
            : this(Parse(versionString, ServerType.MySql))
        {
        }

        public MySqlServerVersion(ServerVersion serverVersion)
            : base(serverVersion.Version, serverVersion.Type, serverVersion.TypeIdentifier)
        {
            if (Type != ServerType.MySql ||
                !string.Equals(TypeIdentifier, MySqlTypeIdentifier))
            {
                throw new ArgumentException($"{nameof(MySqlServerVersion)} is not compatible with the supplied server type.");
            }

            Supports = new MySqlServerVersionSupport(this);
        }

        public class MySqlServerVersionSupport : ServerVersionSupport
        {
            internal MySqlServerVersionSupport([NotNull] ServerVersion serverVersion)
                : base(serverVersion)
            {
            }

            public override bool DateTimeCurrentTimestamp => ServerVersion.Version >= new Version(5, 6, 5);
            public override bool DateTime6 => ServerVersion.Version >= new Version(5, 6, 4);
            public override bool LargerKeyLength => ServerVersion.Version >= new Version(5, 7, 7);
            public override bool RenameIndex => ServerVersion.Version >= new Version(5, 7, 0);
            public override bool RenameColumn => ServerVersion.Version >= new Version(8, 0, 0);
            public override bool WindowFunctions => ServerVersion.Version >= new Version(8, 0, 0);
            public override bool FloatCast => false; // The implemented support drops some decimal places and rounds.
            public override bool DoubleCast => ServerVersion.Version >= new Version(8, 0, 17);
            public override bool OuterApply => ServerVersion.Version >= new Version(8, 0, 14);
            public override bool CrossApply => ServerVersion.Version >= new Version(8, 0, 14);
            public override bool OuterReferenceInMultiLevelSubquery => ServerVersion.Version >= new Version(8, 0, 14);
            public override bool Json => ServerVersion.Version >= new Version(5, 7, 8);
            public override bool GeneratedColumns => ServerVersion.Version >= new Version(5, 7, 6);
            public override bool NullableGeneratedColumns => ServerVersion.Version >= new Version(5, 7, 0);
            public override bool ParenthesisEnclosedGeneratedColumnExpressions => GeneratedColumns;
            public override bool DefaultCharSetUtf8Mb4 => ServerVersion.Version >= new Version(8, 0, 0);
            public override bool DefaultExpression => ServerVersion.Version >= new Version(8, 0, 13);
            public override bool AlternativeDefaultExpression => false;
            public override bool SpatialIndexes => ServerVersion.Version >= new Version(5, 7, 5);
            public override bool SpatialReferenceSystemRestrictedColumns => ServerVersion.Version >= new Version(8, 0, 3);
            public override bool SpatialFunctionAdditions => false;
            public override bool SpatialSupportFunctionAdditions => ServerVersion.Version >= new Version(5, 7, 6);
            public override bool SpatialSetSridFunction => ServerVersion.Version >= new Version(8, 0, 0);
            public override bool SpatialDistanceFunctionImplementsAndoyer => ServerVersion.Version >= new Version(8, 0, 0);
            public override bool SpatialDistanceSphereFunction => ServerVersion.Version >= new Version(8, 0, 0);
            public override bool SpatialGeographic => ServerVersion.Version >= new Version(8, 0, 0);
            public override bool ExceptIntercept => false;
            public override bool ExceptInterceptPrecedence => false;
            public override bool JsonDataTypeEmulation => false;
            public override bool ImplicitBoolCheckUsesIndex => ServerVersion.Version >= new Version(8, 0, 0); // Exact version has not been verified yet
            public override bool MySqlBug96947Workaround => ServerVersion.Version >= new Version(5, 7, 0) &&
                                                            ServerVersion.Version < new Version(8, 0, 25); // Exact version has not been verified yet, but it is 5.7.x and could very well be 5.7.0
            public override bool MySqlBug104294Workaround => ServerVersion.Version >= new Version(8, 0, 0); // Exact version has not been determined yet
            public override bool FullTextParser => ServerVersion.Version >= new Version(5, 7, 3);
            public override bool InformationSchemaCheckConstraintsTable => ServerVersion.Version >= new Version(8, 0, 16); // MySQL is missing the explicit TABLE_NAME column that MariaDB supports, so always join the TABLE_CONSTRAINTS table when accessing CHECK_CONSTRAINTS for any database server that supports CHECK_CONSTRAINTS.
            public override bool MySqlBugLimit0Offset0ExistsWorkaround => true;
        }
    }
}
