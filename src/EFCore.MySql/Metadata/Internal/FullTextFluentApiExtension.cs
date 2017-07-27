using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Microsoft.EntityFrameworkCore
{
    public static class FullTextFluentApiExtension
    {
        public static IndexBuilder IsFullText([NotNull] this IndexBuilder indexBuilder, [CanBeNull] string sql)
        {
            indexBuilder.Metadata.AddAnnotation(MySqlAnnotationNames.FullTextIndex, "FULLTEXT");
            return indexBuilder;
        }
    }
}
