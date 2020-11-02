// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

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
