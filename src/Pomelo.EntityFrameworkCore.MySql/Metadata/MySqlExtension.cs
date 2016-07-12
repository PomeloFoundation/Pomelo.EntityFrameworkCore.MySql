// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Metadata
{
    public class MySqlExtension : IMySqlExtension
    {
        readonly IModel _model;
        readonly string _annotationName;

        MySqlExtension(
            [NotNull] IMutableModel model,
            [NotNull] string annotationPrefix,
            [NotNull] string name,
            [CanBeNull] string schema = null)
            : this(model, BuildAnnotationName(annotationPrefix, name, schema))
        {
            Check.NotNull(model, nameof(model));
            Check.NotEmpty(annotationPrefix, nameof(annotationPrefix));
            Check.NotEmpty(name, nameof(name));
            Check.NullButNotEmpty(schema, nameof(schema));

            SetData(new MySqlExtensionData
            {
                Name = name,
                Schema = schema
            });
        }

        MySqlExtension(IModel model, string annotationName)
        {
            _model = model;
            _annotationName = annotationName;
        }

        public static MySqlExtension GetOrAddMySqlExtension(
            [NotNull] IMutableModel model,
            [NotNull] string annotationPrefix,
            [NotNull] string name,
            [CanBeNull] string schema = null)
            => FindMySqlExtension(model, annotationPrefix, name, schema) ?? new MySqlExtension(model, annotationPrefix, name, schema);

        public static MySqlExtension FindMySqlExtension(
            [NotNull] IMutableModel model,
            [NotNull] string annotationPrefix,
            [NotNull] string name,
            [CanBeNull] string schema = null)
            => (MySqlExtension)FindMySqlExtension((IModel)model, annotationPrefix, name, schema);

        public static IMySqlExtension FindMySqlExtension(
            [NotNull] IModel model,
            [NotNull] string annotationPrefix,
            [NotNull] string name,
            [CanBeNull] string schema = null)
        {
            Check.NotNull(model, nameof(model));
            Check.NotEmpty(name, nameof(name));
            Check.NullButNotEmpty(schema, nameof(schema));

            var annotationName = BuildAnnotationName(annotationPrefix, name, schema);

            return model[annotationName] == null ? null : new MySqlExtension(model, annotationName);
        }

        static string BuildAnnotationName(string annotationPrefix, string name, string schema)
            => annotationPrefix + schema + "." + name;

        public static IEnumerable<IMySqlExtension> GetMySqlExtensions([NotNull] IModel model, [NotNull] string annotationPrefix)
        {
            Check.NotNull(model, nameof(model));
            Check.NotEmpty(annotationPrefix, nameof(annotationPrefix));

            return model.GetAnnotations()
                .Where(a => a.Name.StartsWith(annotationPrefix, StringComparison.Ordinal))
                .Select(a => new MySqlExtension(model, a.Name));
        }

        public virtual Model Model => (Model)_model;

        public virtual string Name => GetData().Name;

        public virtual string Schema => GetData().Schema ?? Model.Relational().DefaultSchema;

        public virtual string Version
        {
            get { return GetData().Version; }
            set
            {
                var data = GetData();
                data.Version = value;
                SetData(data);
            }
        }

        MySqlExtensionData GetData() => MySqlExtensionData.Deserialize((string)Model[_annotationName]);

        void SetData(MySqlExtensionData data)
        {
            Model[_annotationName] = data.Serialize();
        }

        IModel IMySqlExtension.Model => _model;

        public override string ToString()
        {
            var sb = new StringBuilder();
            if (Schema != null)
            {
                sb.Append(Schema);
                sb.Append('.');
            }
            sb.Append(Name);
            if (Version != null)
            {
                sb.Append("' (");
                sb.Append(Version);
                sb.Append(')');
            }
            return sb.ToString();
        }

        class MySqlExtensionData
        {
            public string Name { get; set; }
            public string Schema { get; set; }
            public string Version { get; set; }

            public string Serialize()
            {
                var builder = new StringBuilder();

                EscapeAndQuote(builder, Name);
                builder.Append(", ");
                EscapeAndQuote(builder, Schema);
                builder.Append(", ");
                EscapeAndQuote(builder, Version);

                return builder.ToString();
            }

            public static MySqlExtensionData Deserialize([NotNull] string value)
            {
                Check.NotEmpty(value, nameof(value));

                try
                {
                    var data = new MySqlExtensionData();

                    // ReSharper disable PossibleInvalidOperationException
                    var position = 0;
                    data.Name = ExtractValue(value, ref position);
                    data.Schema = ExtractValue(value, ref position);
                    data.Version = ExtractValue(value, ref position);
                    // ReSharper restore PossibleInvalidOperationException

                    return data;
                }
                catch (Exception ex)
                {
                    throw new ArgumentException(RelationalStrings.BadSequenceString, ex);
                }
            }

            private static string ExtractValue(string value, ref int position)
            {
                position = value.IndexOf('\'', position) + 1;

                var end = value.IndexOf('\'', position);

                while ((end + 1 < value.Length)
                       && (value[end + 1] == '\''))
                {
                    end = value.IndexOf('\'', end + 2);
                }

                var extracted = value.Substring(position, end - position).Replace("''", "'");
                position = end + 1;

                return extracted.Length == 0 ? null : extracted;
            }

            private static void EscapeAndQuote(StringBuilder builder, object value)
            {
                builder.Append("'");

                if (value != null)
                {
                    builder.Append(value.ToString().Replace("'", "''"));
                }

                builder.Append("'");
            }
        }
    }
}