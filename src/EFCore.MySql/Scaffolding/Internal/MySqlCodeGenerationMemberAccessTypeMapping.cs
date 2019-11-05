using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Storage;

namespace Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal
{
    internal class MySqlCodeGenerationMemberAccessTypeMapping : RelationalTypeMapping
    {
        private const string DummyStoreType = "clrOnly";

        public MySqlCodeGenerationMemberAccessTypeMapping()
            : base(new RelationalTypeMappingParameters(new CoreTypeMappingParameters(typeof(string)), DummyStoreType))
        {
        }

        protected MySqlCodeGenerationMemberAccessTypeMapping(RelationalTypeMappingParameters parameters)
            : base(parameters)
        {
        }

        protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
            => new MySqlCodeGenerationMemberAccessTypeMapping(parameters);

        public override Expression GenerateCodeLiteral(object value)
            => value is MySqlCodeGenerationMemberAccess memberAccess
                ? Expression.MakeMemberAccess(null, memberAccess.MemberInfo)
                : null;
    }
}
