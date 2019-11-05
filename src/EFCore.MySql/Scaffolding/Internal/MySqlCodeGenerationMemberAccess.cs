using System.Reflection;

namespace Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal
{
    internal class MySqlCodeGenerationMemberAccess
    {
        public MemberInfo MemberInfo { get; }

        public MySqlCodeGenerationMemberAccess(MemberInfo memberInfo)
        {
            MemberInfo = memberInfo;
        }
    }
}
