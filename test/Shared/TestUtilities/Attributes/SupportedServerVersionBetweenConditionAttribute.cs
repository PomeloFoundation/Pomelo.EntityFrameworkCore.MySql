using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Microting.EntityFrameworkCore.MySql.Tests.TestUtilities.Attributes
{
    /// <remarks>
    /// When the condition is not met, the test (or test class) gets the `category=failing` trait, which the test runner
    /// is configured to exclude. See <see cref="MySqlTestTrait"/>.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class SupportedServerVersionBetweenConditionAttribute : Attribute, global::Xunit.v3.ITraitAttribute
    {
        public ServerVersion MinVersionInclusive { get; }
        public ServerVersion MaxVersionExclusive { get; }

        public SupportedServerVersionBetweenConditionAttribute(string minVersionInclusive, string maxVersionExclusive)
        {
            MinVersionInclusive = ServerVersion.Parse(minVersionInclusive);
            MaxVersionExclusive = ServerVersion.Parse(maxVersionExclusive);
        }

        public virtual bool IsMet()
        {
            var currentVersion = AppConfig.ServerVersion;
            var isMet = currentVersion.Type == MinVersionInclusive.Type &&
                        currentVersion.TypeIdentifier == MinVersionInclusive.TypeIdentifier &&
                        currentVersion.Version >= MinVersionInclusive.Version &&
                        currentVersion.Type == MaxVersionExclusive.Type &&
                        currentVersion.TypeIdentifier == MaxVersionExclusive.TypeIdentifier &&
                        currentVersion.Version < MaxVersionExclusive.Version;

            return Invert
                ? !isMet
                : isMet;
        }

        public virtual IReadOnlyCollection<KeyValuePair<string, string>> GetTraits()
            => MySqlTestTrait.ForCondition(IsMet());

        /// <summary>
        /// An optional reason, documenting why the test is being excluded. Not evaluated by the test runner, which
        /// filters on the `category=failing` trait instead.
        /// </summary>
        public virtual string Skip { get; set; }

        public virtual bool Invert { get; set; }
    }
}
