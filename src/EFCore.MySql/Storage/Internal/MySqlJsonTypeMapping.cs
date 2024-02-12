// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Design.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MySqlConnector;

namespace Pomelo.EntityFrameworkCore.MySql.Storage.Internal
{
    public class MySqlJsonTypeMapping<T> : MySqlJsonTypeMapping
    {
        public static new MySqlJsonTypeMapping<T> Default { get; } = new("json", null, null, false, true);

        public MySqlJsonTypeMapping(
            [NotNull] string storeType,
            [CanBeNull] ValueConverter valueConverter,
            [CanBeNull] ValueComparer valueComparer,
            bool noBackslashEscapes,
            bool replaceLineBreaksWithCharFunction)
            : base(
                storeType,
                typeof(T),
                valueConverter,
                valueComparer,
                noBackslashEscapes,
                replaceLineBreaksWithCharFunction)
        {
        }

        protected MySqlJsonTypeMapping(
            RelationalTypeMappingParameters parameters,
            MySqlDbType mySqlDbType,
            bool noBackslashEscapes,
            bool replaceLineBreaksWithCharFunction)
            : base(parameters, mySqlDbType, noBackslashEscapes, replaceLineBreaksWithCharFunction)
        {
        }

        protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
            => new MySqlJsonTypeMapping<T>(parameters, MySqlDbType, NoBackslashEscapes, ReplaceLineBreaksWithCharFunction);

        protected override RelationalTypeMapping Clone(bool? noBackslashEscapes = null, bool? replaceLineBreaksWithCharFunction = null)
            => new MySqlJsonTypeMapping<T>(
                Parameters,
                MySqlDbType,
                noBackslashEscapes ?? NoBackslashEscapes,
                replaceLineBreaksWithCharFunction ?? ReplaceLineBreaksWithCharFunction);
    }

    public abstract class MySqlJsonTypeMapping : MySqlStringTypeMapping, IMySqlCSharpRuntimeAnnotationTypeMappingCodeGenerator
    {
        public MySqlJsonTypeMapping(
            [NotNull] string storeType,
            [NotNull] Type clrType,
            [CanBeNull] ValueConverter valueConverter,
            [CanBeNull] ValueComparer valueComparer,
            bool noBackslashEscapes,
            bool replaceLineBreaksWithCharFunction)
            : base(
                new RelationalTypeMappingParameters(
                    new CoreTypeMappingParameters(
                        clrType,
                        valueConverter,
                        valueComparer),
                    storeType,
                    unicode: true),
                MySqlDbType.JSON,
                noBackslashEscapes,
                replaceLineBreaksWithCharFunction,
                false,
                false)
        {
            if (storeType != "json")
            {
                throw new ArgumentException($"The store type '{nameof(storeType)}' must be 'json'.", nameof(storeType));
            }
        }

        protected MySqlJsonTypeMapping(
            RelationalTypeMappingParameters parameters,
            MySqlDbType mySqlDbType,
            bool noBackslashEscapes,
            bool replaceLineBreaksWithCharFunction)
            : base(
                parameters,
                mySqlDbType,
                noBackslashEscapes,
                replaceLineBreaksWithCharFunction,
                isUnquoted: false,
                forceToString: false)
        {
        }

        /// <summary>
        /// Supports compiled models via IMySqlCSharpRuntimeAnnotationTypeMappingCodeGenerator.Create.
        /// </summary>
        protected abstract RelationalTypeMapping Clone(
            bool? noBackslashEscapes = null,
            bool? replaceLineBreaksWithCharFunction = null);

        protected override void ConfigureParameter(DbParameter parameter)
        {
            base.ConfigureParameter(parameter);

            // MySqlConnector does not know how to handle our custom MySqlJsonString type, that could be used when a
            // string parameter is explicitly cast to it.
            if (parameter.Value is MySqlJsonString mySqlJsonString)
            {
                parameter.Value = (string)mySqlJsonString;
            }
        }

        void IMySqlCSharpRuntimeAnnotationTypeMappingCodeGenerator.Create(
            CSharpRuntimeAnnotationCodeGeneratorParameters codeGeneratorParameters,
            CSharpRuntimeAnnotationCodeGeneratorDependencies codeGeneratorDependencies)
        {
            var defaultTypeMapping = Default;
            if (defaultTypeMapping == this)
            {
                return;
            }

            var code = codeGeneratorDependencies.CSharpHelper;

            var cloneParameters = new List<string>();

            if (NoBackslashEscapes != defaultTypeMapping.NoBackslashEscapes)
            {
                cloneParameters.Add($"noBackslashEscapes: {code.Literal(NoBackslashEscapes)}");
            }

            if (ReplaceLineBreaksWithCharFunction != defaultTypeMapping.ReplaceLineBreaksWithCharFunction)
            {
                cloneParameters.Add($"replaceLineBreaksWithCharFunction: {code.Literal(ReplaceLineBreaksWithCharFunction)}");
            }

            if (cloneParameters.Any())
            {
                var mainBuilder = codeGeneratorParameters.MainBuilder;

                mainBuilder.AppendLine(";");

                mainBuilder
                    .AppendLine($"{codeGeneratorParameters.TargetName}.TypeMapping = (({code.Reference(GetType())}){codeGeneratorParameters.TargetName}.TypeMapping).Clone(")
                    .IncrementIndent();

                for (var i = 0; i < cloneParameters.Count; i++)
                {
                    if (i > 0)
                    {
                        mainBuilder.AppendLine(",");
                    }

                    mainBuilder.Append(cloneParameters[i]);
                }

                mainBuilder
                    .Append(")")
                    .DecrementIndent();
            }
        }
    }
}
