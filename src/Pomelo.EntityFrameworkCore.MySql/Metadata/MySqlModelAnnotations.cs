// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Metadata
{
    public class MySqlModelAnnotations : RelationalModelAnnotations, IMySqlModelAnnotations
    {
        public MySqlModelAnnotations([NotNull] IModel model)
            : base(model, MySqlFullAnnotationNames.Instance)
        {
        }

        public MySqlModelAnnotations([NotNull] RelationalAnnotations annotations)
            : base(annotations, MySqlFullAnnotationNames.Instance)
        {
        }

        public virtual IMySqlExtension GetOrAddMySqlExtension([CanBeNull] string name, [CanBeNull] string schema = null)
            => MySqlExtension.GetOrAddMySqlExtension((IMutableModel)Model,
                MySqlFullAnnotationNames.Instance.MySqlExtensionPrefix,
                name,
                schema);

        public virtual IReadOnlyList<IMySqlExtension> MySqlExtensions
            => MySqlExtension.GetMySqlExtensions(Model, MySqlFullAnnotationNames.Instance.MySqlExtensionPrefix).ToList();

        public virtual string DatabaseTemplate
        {
            get { return (string)Annotations.GetAnnotation(MySqlFullAnnotationNames.Instance.DatabaseTemplate, null); }
            [param: CanBeNull]
            set { SetDatabaseTemplate(value); }
        }

        protected virtual bool SetDatabaseTemplate([CanBeNull] string value)
            => Annotations.SetAnnotation(
                MySqlFullAnnotationNames.Instance.DatabaseTemplate,
                null,
                Check.NullButNotEmpty(value, nameof(value)));
    }
}
