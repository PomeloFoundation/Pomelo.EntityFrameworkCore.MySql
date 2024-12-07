using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities.Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.Tests.TestUtilities.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class SupportedServerVersionBetweenConditionAttribute : Attribute, ITestCondition
    {
        public ServerVersion MinVersionInclusive { get; }
        public ServerVersion MaxVersionExclusive { get; }

        public SupportedServerVersionBetweenConditionAttribute(string minVersionInclusive, string maxVersionExclusive)
        {
            MinVersionInclusive = ServerVersion.Parse(minVersionInclusive);
            MaxVersionExclusive = ServerVersion.Parse(maxVersionExclusive);
        }

        public virtual ValueTask<bool> IsMetAsync()
        {
            var currentVersion = AppConfig.ServerVersion;
            var isMet = currentVersion.Type == MinVersionInclusive.Type &&
                        currentVersion.TypeIdentifier == MinVersionInclusive.TypeIdentifier &&
                        currentVersion.Version >= MinVersionInclusive.Version &&
                        currentVersion.Type == MaxVersionExclusive.Type &&
                        currentVersion.TypeIdentifier == MaxVersionExclusive.TypeIdentifier &&
                        currentVersion.Version < MaxVersionExclusive.Version;

            if (Invert)
            {
                isMet = !isMet;
            }

            if (!isMet && string.IsNullOrEmpty(Skip))
            {
                Skip = $"The test is not supported on server version {currentVersion}.";
            }

            return new ValueTask<bool>(isMet);
        }

        public virtual string SkipReason => Skip;
        public virtual string Skip { get; set; }
        public virtual bool Invert { get; set; }
    }
}
