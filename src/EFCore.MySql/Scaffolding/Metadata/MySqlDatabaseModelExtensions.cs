using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore.Scaffolding.Metadata
{
    public static class MySqlDatabaseModelExtensions
    {
        public static MySqlColumnModelAnnotations MySql(/* [NotNull] */ this ColumnModel column)
            => new MySqlColumnModelAnnotations(column);

        public static MySqlIndexModelAnnotations MySql(/* [NotNull] */ this IndexModel index)
            => new MySqlIndexModelAnnotations(index);
    }
}
