using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Internal;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Microsoft.EntityFrameworkCore.ChangeTracking.Internal
{
    public class MySqlChangeDetector : ChangeDetector
    {
        public override void DetectChanges(InternalEntityEntry entry)
        {
            var entityType = entry.EntityType;

            var properties = entityType.GetProperties();
            foreach (var property in properties)
            {
                var current = entry[property];
                var old = entry.GetOriginalValue(property);
                var flag = current != null && current.GetType().Name == typeof(JsonObject<>).Name;
                TypeInfo type = null;
                PropertyInfo originalProperty = null;
                if (flag)
                {
                    type = current.GetType().GetTypeInfo();
                    originalProperty = type.DeclaredProperties.Single(x => x.Name == "_originalValue");
                    old = (string)originalProperty.GetValue(current);
                    var prop = type.DeclaredProperties.Single(x => x.Name == "Json");
                    current = (string)prop.GetValue(current);
                }
                if (property.GetOriginalValueIndex() >= 0
                    && (!Equals(entry[property], entry.GetOriginalValue(property)) || flag && current != old))
                {
                    entry.SetPropertyModified(property);
                    if (flag)
                    {
                        originalProperty.SetValue(entry[property], current);
                    }
                }
            }

            base.DetectChanges(entry);
        }
    }
}
