// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using JetBrains.Annotations;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    /// Represents a <see cref="ServerVersion"/> for MariaDB database servers.
    /// For MySQL database servers, use <see cref="MySqlServerVersion"/> instead.
    /// </summary>
    public class MariaDbServerVersion : ServerVersion
    {
        public static readonly string MariaDbTypeIdentifier = nameof(ServerType.MariaDb).ToLowerInvariant();
        public static readonly ServerVersion LatestSupportedServerVersion = new MariaDbServerVersion(new Version(10, 5, 5));

        public override ServerVersionSupport Supports { get; }

        public MariaDbServerVersion(Version version)
            : base(version, ServerType.MariaDb)
        {
            Supports = new MariaDbServerVersionSupport(this);
        }

        public MariaDbServerVersion(string versionString)
            : this(Parse(versionString, ServerType.MariaDb))
        {
        }

        public MariaDbServerVersion(ServerVersion serverVersion)
            : base(serverVersion.Version, serverVersion.Type, serverVersion.TypeIdentifier)
        {
            if (Type != ServerType.MariaDb ||
                !string.Equals(TypeIdentifier, MariaDbTypeIdentifier))
            {
                throw new ArgumentException($"{nameof(MariaDbServerVersion)} is not compatible with the supplied server type.");
            }

            Supports = new MariaDbServerVersionSupport(this);
        }

        public class MariaDbServerVersionSupport : ServerVersionSupport
        {
            internal MariaDbServerVersionSupport([NotNull] ServerVersion serverVersion)
                : base(serverVersion)
            {
            }

            public override bool DateTimeCurrentTimestamp => ServerVersion.Version >= new Version(10, 0, 1);
            public override bool DateTime6 => ServerVersion.Version >= new Version(10, 1, 2);
            public override bool LargerKeyLength => ServerVersion.Version >= new Version(10, 2, 2);
            public override bool RenameIndex => false;
            public override bool RenameColumn => ServerVersion.Version >= new Version(10, 5, 2);
            public override bool WindowFunctions => ServerVersion.Version >= new Version(10, 2, 0);
            public override bool FloatCast => false; // The implemented support drops some decimal places and rounds.
            public override bool DoubleCast => ServerVersion.Version >= new Version(10, 4, 0);
            public override bool OuterApply => false; // MDEV-19078, MDEV-6373
            public override bool CrossApply => false; // MDEV-19078, MDEV-6373
            public override bool OuterReferenceInMultiLevelSubquery => false;
            public override bool Json => ServerVersion.Version >= new Version(10, 2, 4);
            public override bool GeneratedColumns => ServerVersion.Version >= new Version(10, 2, 0);
            public override bool NullableGeneratedColumns => false;
            public override bool ParenthesisEnclosedGeneratedColumnExpressions => false;
            public override bool DefaultCharSetUtf8Mb4 => false;
            public override bool DefaultExpression => false;
            public override bool AlternativeDefaultExpression => ServerVersion.Version >= new Version(10, 2, 7);
            public override bool SpatialIndexes => ServerVersion.Version >= new Version(10, 2, 2);
            public override bool SpatialReferenceSystemRestrictedColumns => false;
            public override bool SpatialFunctionAdditions => ServerVersion.Version >= new Version(10, 1, 2);
            public override bool SpatialSupportFunctionAdditions => false;
            public override bool SpatialSetSridFunction => false;
            public override bool SpatialDistanceFunctionImplementsAndoyer => false;
            public override bool SpatialGeographic => false;
            public override bool ExceptIntercept => ServerVersion.Version >= new Version(10, 3, 0);
            public override bool ExceptInterceptPrecedence => ServerVersion.Version >= new Version(10, 4, 0);
            public override bool JsonDataTypeEmulation => ServerVersion.Version >= new Version(10, 2, 4); // JSON_COMPACT was added in 10.2.4, though most other functions where added in 10.2.3
            public override bool ImplicitBoolCheckUsesIndex => ServerVersion.Version >= new Version(10, 0, 0); // Exact version has not been verified yet
            public override bool Sequences => ServerVersion.Version >= new Version(10, 3, 0);
            public override bool InformationSchemaCheckConstraintsTable => ServerVersion.Version >= new Version(10, 3, 10) ||
                                                                           ServerVersion.Version.Major == 10 && ServerVersion.Version.Minor == 2 && ServerVersion.Version.Build >= 22;  // MySQL is missing the explicit TABLE_NAME column that MariaDB supports, so always join the TABLE_CONSTRAINTS table when accessing CHECK_CONSTRAINTS for any database server that supports CHECK_CONSTRAINTS.
            public override bool IdentifyJsonColumsByCheckConstraints => true;
        }
    }
}
