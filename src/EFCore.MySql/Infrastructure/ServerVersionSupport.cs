// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Pomelo.EntityFrameworkCore.MySql.Infrastructure
{
    public class ServerVersionSupport
    {
        public virtual ServerVersion ServerVersion { get; }

        public ServerVersionSupport([NotNull] ServerVersion serverVersion)
        {
            ServerVersion = serverVersion ?? throw new ArgumentNullException(nameof(serverVersion));
        }

        public override string ToString()
            => throw new NotImplementedException(); // TODO: Remove or implement!

        public virtual bool Version(string versionString)
            => Version(ServerVersion.Parse(versionString));

        public virtual bool Version(ServerVersion serverVersion)
            => ServerVersion.Type == serverVersion.Type &&
               ServerVersion.TypeIdentifier == serverVersion.TypeIdentifier &&
               ServerVersion.Version >= serverVersion.Version;

        public virtual bool PropertyOrVersion(string propertyNameOrServerVersion)
        {
            if (ServerVersion.TryParse(propertyNameOrServerVersion, out var serverVersion))
            {
                return ServerVersion.Type == serverVersion.Type &&
                       ServerVersion.TypeIdentifier == serverVersion.TypeIdentifier &&
                       ServerVersion.Version >= serverVersion.Version;
            }

            var property = typeof(ServerVersionSupport).GetRuntimeProperty(propertyNameOrServerVersion);
            if (property != null &&
                property.PropertyType == typeof(bool))
            {
                return (bool)property.GetValue(this);
            }

            throw new ArgumentException("The parameter is neither a valid server version nor a valid property of 'ServerVersionSupport'.", nameof(propertyNameOrServerVersion));
        }

        public virtual bool DateTimeCurrentTimestamp => false;
        public virtual bool DateTime6 => false;
        public virtual bool LargerKeyLength => false;
        public virtual bool RenameIndex => false;
        public virtual bool RenameColumn => false;
        public virtual bool WindowFunctions => false;
        public virtual bool FloatCast => false; // The implemented support drops some decimal places and rounds.
        public virtual bool DoubleCast => false;
        public virtual bool OuterApply => false;
        public virtual bool CrossApply => false;
        public virtual bool OuterReferenceInMultiLevelSubquery => false;
        public virtual bool Json => false;
        public virtual bool GeneratedColumns => false;
        public virtual bool NullableGeneratedColumns => false;
        public virtual bool ParenthesisEnclosedGeneratedColumnExpressions => false;
        public virtual bool DefaultCharSetUtf8Mb4 => false;
        public virtual bool DefaultExpression => false;
        public virtual bool AlternativeDefaultExpression => false;
        public virtual bool SpatialIndexes => false;
        public virtual bool SpatialReferenceSystemRestrictedColumns => false;
        public virtual bool SpatialFunctionAdditions => false;
        public virtual bool SpatialSupportFunctionAdditions => false;
        public virtual bool SpatialSetSridFunction => false;
        public virtual bool SpatialDistanceFunctionImplementsAndoyer => false;
        public virtual bool SpatialDistanceSphereFunction => false;
        public virtual bool SpatialGeographic => false;
        public virtual bool ExceptIntercept => false;
        public virtual bool ExceptInterceptPrecedence => false;
        public virtual bool JsonDataTypeEmulation => false;
        public virtual bool ImplicitBoolCheckUsesIndex => false;
        public virtual bool Sequences => false;
        public virtual bool MySqlBug96947Workaround => false;
        public virtual bool MySqlBug104294Workaround => false;
        public virtual bool FullTextParser => false;
        public virtual bool InformationSchemaCheckConstraintsTable => false;
        public virtual bool IdentifyJsonColumsByCheckConstraints => false;
        public virtual bool MySqlBugLimit0Offset0ExistsWorkaround => false;
    }
}
