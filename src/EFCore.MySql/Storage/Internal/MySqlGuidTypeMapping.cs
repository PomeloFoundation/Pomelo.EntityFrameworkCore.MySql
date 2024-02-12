// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Design.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Json;
using MySqlConnector;

namespace Pomelo.EntityFrameworkCore.MySql.Storage.Internal
{
    public class MySqlGuidTypeMapping : GuidTypeMapping, IJsonSpecificTypeMapping, IMySqlCSharpRuntimeAnnotationTypeMappingCodeGenerator
    {
        public static new MySqlGuidTypeMapping Default { get; } = new(MySqlGuidFormat.Char36);

        public virtual MySqlGuidFormat GuidFormat { get; }

        public MySqlGuidTypeMapping(MySqlGuidFormat guidFormat)
            : this(new RelationalTypeMappingParameters(
                    new CoreTypeMappingParameters(
                        typeof(Guid),
                        jsonValueReaderWriter: JsonGuidReaderWriter.Instance),
                    GetStoreType(guidFormat),
                    StoreTypePostfix.Size,
                    System.Data.DbType.Guid,
                    false,
                    GetSize(guidFormat),
                    true),
                guidFormat)
        {
        }

        protected MySqlGuidTypeMapping(RelationalTypeMappingParameters parameters, MySqlGuidFormat guidFormat)
            : base(parameters)
        {
            GuidFormat = guidFormat;
        }

        protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
            => new MySqlGuidTypeMapping(parameters, GuidFormat);

        public virtual RelationalTypeMapping Clone(MySqlGuidFormat guidFormat)
            => new MySqlGuidTypeMapping(Parameters, guidFormat);

        public virtual bool IsCharBasedStoreType
            => GetStoreType(GuidFormat) == "char";

        protected override string GenerateNonNullSqlLiteral(object value)
        {
            switch (GuidFormat)
            {
                case MySqlGuidFormat.Char36:
                    return $"'{value:D}'";

                case MySqlGuidFormat.Char32:
                    return $"'{value:N}'";

                case MySqlGuidFormat.Binary16:
                case MySqlGuidFormat.TimeSwapBinary16:
                case MySqlGuidFormat.LittleEndianBinary16:
                    return "0x" + Convert.ToHexString(GetBytesFromGuid(GuidFormat, (Guid)value));

                case MySqlGuidFormat.None:
                case MySqlGuidFormat.Default:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static string GetStoreType(MySqlGuidFormat guidFormat)
        {
            switch (guidFormat)
            {
                case MySqlGuidFormat.Char36:
                case MySqlGuidFormat.Char32:
                    return "char";

                case MySqlGuidFormat.Binary16:
                case MySqlGuidFormat.TimeSwapBinary16:
                case MySqlGuidFormat.LittleEndianBinary16:
                    return "binary";

                case MySqlGuidFormat.None:
                case MySqlGuidFormat.Default:
                default:
                    throw new InvalidOperationException();
            }
        }

        private static int GetSize(MySqlGuidFormat guidFormat)
        {
            switch (guidFormat)
            {
                case MySqlGuidFormat.Char36:
                    return 36;

                case MySqlGuidFormat.Char32:
                    return 32;

                case MySqlGuidFormat.Binary16:
                case MySqlGuidFormat.TimeSwapBinary16:
                case MySqlGuidFormat.LittleEndianBinary16:
                    return 16;

                case MySqlGuidFormat.None:
                case MySqlGuidFormat.Default:
                default:
                    throw new InvalidOperationException();
            }
        }

        public static bool IsValidGuidFormat(MySqlGuidFormat guidFormat)
            => guidFormat != MySqlGuidFormat.None &&
               guidFormat != MySqlGuidFormat.Default;

        protected static byte[] GetBytesFromGuid(MySqlGuidFormat guidFormat, Guid guid)
        {
            var bytes = guid.ToByteArray();

            if (guidFormat == MySqlGuidFormat.Binary16)
            {
                return new[] { bytes[3], bytes[2], bytes[1], bytes[0], bytes[5], bytes[4], bytes[7], bytes[6], bytes[8], bytes[9], bytes[10], bytes[11], bytes[12], bytes[13], bytes[14], bytes[15] };
            }

            if (guidFormat == MySqlGuidFormat.TimeSwapBinary16)
            {
                return new[] { bytes[7], bytes[6], bytes[5], bytes[4], bytes[3], bytes[2], bytes[1], bytes[0], bytes[8], bytes[9], bytes[10], bytes[11], bytes[12], bytes[13], bytes[14], bytes[15] };
            }

            return bytes;
        }

        /// <summary>
        /// For JSON values, we will always use the 36 character string representation.
        /// </summary>
        public virtual RelationalTypeMapping CloneAsJsonCompatible()
            => new MySqlGuidTypeMapping(MySqlGuidFormat.Char36);

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

            if (GuidFormat != defaultTypeMapping.GuidFormat)
            {
                cloneParameters.Add($"guidFormat: {code.Literal(GuidFormat, true)}");
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
