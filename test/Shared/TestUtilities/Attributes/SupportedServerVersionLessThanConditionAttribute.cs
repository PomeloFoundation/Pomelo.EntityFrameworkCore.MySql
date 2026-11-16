using System;
using System.Collections.Generic;
using System.Linq;

namespace Microting.EntityFrameworkCore.MySql.Tests.TestUtilities.Attributes
{
    /// <summary>
    /// Use the `propertiesOrVersions` constructor parameter, for OR conditions.
    /// Use multiple <see cref="SupportedServerVersionLessThanConditionAttribute"/> attributes, for AND conditions.
    /// </summary>
    /// <remarks>
    /// When the condition is not met, the test (or test class) gets the `category=failing` trait, which the test runner
    /// is configured to exclude. See <see cref="MySqlTestTrait"/>.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class SupportedServerVersionLessThanConditionAttribute : Attribute, global::Xunit.v3.ITraitAttribute
    {
        protected string[] PropertiesOrVersions { get; }

        public SupportedServerVersionLessThanConditionAttribute(params string[] propertiesOrVersions)
        {
            PropertiesOrVersions = propertiesOrVersions;
        }

        public virtual bool IsMet()
        {
            var currentVersion = AppConfig.ServerVersion;
            return !PropertiesOrVersions.Any(s => currentVersion.Supports.PropertyOrVersion(s));
        }

        public virtual IReadOnlyCollection<KeyValuePair<string, string>> GetTraits()
            => MySqlTestTrait.ForCondition(IsMet());

        /// <summary>
        /// An optional reason, documenting why the test is being excluded. Not evaluated by the test runner, which
        /// filters on the `category=failing` trait instead.
        /// </summary>
        public virtual string Skip { get; set; }
    }
}
