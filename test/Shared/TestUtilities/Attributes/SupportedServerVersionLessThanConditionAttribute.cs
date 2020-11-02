using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.TestUtilities.Xunit;
using Pomelo.EntityFrameworkCore.MySql.Storage;

namespace Pomelo.EntityFrameworkCore.MySql.Tests.TestUtilities.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class SupportedServerVersionLessThanConditionAttribute : Attribute, ITestCondition
    {
        protected ServerVersionSupport ServerVersionSupport { get; }

        public SupportedServerVersionLessThanConditionAttribute(params string[] versionsOrKeys)
        {
            ServerVersionSupport = ServerVersion.GetSupport(versionsOrKeys);
        }

        public virtual ValueTask<bool> IsMetAsync()
        {
            var currentVersion = AppConfig.ServerVersion;
            var isMet = !ServerVersionSupport.IsSupported(currentVersion);

            if (!isMet && string.IsNullOrEmpty(Skip))
            {
                Skip = $"Test is supported only on server versions lower than {ServerVersionSupport}.";
            }

            return new ValueTask<bool>(isMet);
        }

        public virtual string SkipReason => Skip;
        public virtual string Skip { get; set; }
    }
}
