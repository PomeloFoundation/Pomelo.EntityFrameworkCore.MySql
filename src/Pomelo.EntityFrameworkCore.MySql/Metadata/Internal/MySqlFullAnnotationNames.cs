// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{
    public class MySqlFullAnnotationNames : RelationalFullAnnotationNames
    {
        protected MySqlFullAnnotationNames(string prefix)
            : base(prefix)
        {
            Serial = prefix + MySqlAnnotationNames.Serial;
            DefaultSequenceName = prefix + MySqlAnnotationNames.DefaultSequenceName;
            DefaultSequenceSchema = prefix + MySqlAnnotationNames.DefaultSequenceSchema;
            SequenceName = prefix + MySqlAnnotationNames.SequenceName;
            SequenceSchema = prefix + MySqlAnnotationNames.SequenceSchema;
            IndexMethod = prefix + MySqlAnnotationNames.IndexMethod;
            MySqlExtensionPrefix = prefix + MySqlAnnotationNames.MySqlExtensionPrefix;
            DatabaseTemplate = prefix + MySqlAnnotationNames.DatabaseTemplate;
            ValueGeneratedOnAdd = prefix + MySqlAnnotationNames.ValueGeneratedOnAdd;
        }

        public new static MySqlFullAnnotationNames Instance { get; } = new MySqlFullAnnotationNames(MySqlAnnotationNames.Prefix);

        public readonly string Serial;
        public readonly string DefaultSequenceName;
        public readonly string DefaultSequenceSchema;
        public readonly string SequenceName;
        public readonly string SequenceSchema;
        public readonly string IndexMethod;
        public readonly string MySqlExtensionPrefix;
        public readonly string DatabaseTemplate;
        public readonly string ValueGeneratedOnAdd;
    }
}