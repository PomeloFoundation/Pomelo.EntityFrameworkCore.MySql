namespace Microsoft.EntityFrameworkCore.Scaffolding.Internal
{
    public class MySqlScaffoldingCodeGenerator : IScaffoldingProviderCodeGenerator
    {
        public virtual string GenerateUseProvider(string connectionString, string language)
        {
            return $".UseMySql(\"{connectionString}\")";
        }
    }
}
