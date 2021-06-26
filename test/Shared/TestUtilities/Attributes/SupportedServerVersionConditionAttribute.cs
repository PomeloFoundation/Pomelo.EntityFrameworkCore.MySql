using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.TestUtilities.Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.Tests.TestUtilities.Attributes
{
    /// <summary>
    /// Use the `propertiesOrVersions` constructor parameter, for OR conditions.
    /// Use multiple <see cref="SupportedServerVersionConditionAttribute"/> attributes, for AND conditions.
    /// </summary>
    /// <remarks>
    /// For facts and theories, they must be defined as conditional (ConditionalFact, ConditionalTheory) for this attribute to work.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class SupportedServerVersionConditionAttribute : Attribute, ITestCondition
    {
        protected string[] PropertiesOrVersions { get; }

        public SupportedServerVersionConditionAttribute(params string[] propertiesOrVersions)
        {
            PropertiesOrVersions = propertiesOrVersions;
        }

        public virtual ValueTask<bool> IsMetAsync()
        {
            var currentVersion = AppConfig.ServerVersion;
            var isMet = PropertiesOrVersions.Any(s => currentVersion.Supports.PropertyOrVersion(s));

            if (!isMet && string.IsNullOrEmpty(Skip))
            {
                Skip = $"The test is not supported on server version {currentVersion}.";
            }

            return new ValueTask<bool>(isMet);
        }

        public virtual string SkipReason => Skip;
        public virtual string Skip { get; set; }
    }
}
