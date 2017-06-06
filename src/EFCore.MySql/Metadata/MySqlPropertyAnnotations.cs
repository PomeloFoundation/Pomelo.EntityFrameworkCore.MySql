// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using JetBrains.Annotations;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata;
//using Microsoft.EntityFrameworkCore.Metadata.Internal;

//namespace Microsoft.EntityFrameworkCore.Metadata
//{
//    public class MySqlPropertyAnnotations : RelationalPropertyAnnotations, IMySqlPropertyAnnotations
//    {
//        public MySqlPropertyAnnotations([NotNull] IProperty property, [CanBeNull] string providerPrefix) : base(property, MySqlFullAnnotationNames.Instance)
//        {
//        }

//        public MySqlPropertyAnnotations([NotNull] RelationalAnnotations annotations) : base(annotations, MySqlFullAnnotationNames.Instance)
//        {
//        }

//        public virtual MySqlValueGenerationStrategy? ValueGenerationStrategy
//        {
//            get
//            {
//                if ((Property.ValueGenerated != ValueGenerated.OnAdd)
//                    || !Property.ClrType.UnwrapNullableType().IsInteger()
//                    || (Property.MySql().GeneratedValueSql != null))
//                {
//                    return null;
//                }

//                var value = (MySqlValueGenerationStrategy?)Annotations.GetAnnotation(MySqlAnnotationNames.ValueGenerationStrategy);

//                return value ?? Property.DeclaringEntityType.Model.MySql().ValueGenerationStrategy;
//            }
//            [param: CanBeNull]
//            set { SetValueGenerationStrategy(value); }
//        }

//        protected virtual bool SetValueGenerationStrategy(MySqlValueGenerationStrategy? value)
//        {
//            if (value != null)
//            {
//                var propertyType = Property.ClrType;

//                if ((value == MySqlValueGenerationStrategy.AutoIncrement)
//                    && (!propertyType.IsInteger()
//                        || (propertyType == typeof(byte))
//                        || (propertyType == typeof(byte?))))
//                {
//                    throw new ArgumentException("Bad identity type");
//                    //Property.Name, Property.DeclaringEntityType.Name, propertyType.Name));
//                }
//            }

//            return Annotations.SetAnnotation(MySqlAnnotationNames.ValueGenerationStrategy, value);
//        }
//    }
//}
