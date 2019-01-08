// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.EntityFrameworkCore.TestUtilities.Xunit;
using Xunit.Sdk;

namespace Microsoft.EntityFrameworkCore.TestUtilities
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    [TraitDiscoverer("Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Utilities.MySqlConditionTraitDiscoverer", "Pomelo.EntityFrameworkCore.MySql.FunctionalTests")]
    public sealed class MySqlConditionAttribute : Attribute, ITestCondition, ITraitAttribute
    {
        public MySqlCondition Conditions { get; set; }

        public MySqlConditionAttribute(MySqlCondition conditions)
        {
            Conditions = conditions;
        }

        public bool IsMet
        {
            get
            {
                var isMet = true;
                if (Conditions.HasFlag(MySqlCondition.SupportsSequences))
                {
                    isMet &= TestEnvironment.GetFlag(nameof(MySqlCondition.SupportsSequences)) ?? true;
                }

                if (Conditions.HasFlag(MySqlCondition.SupportsOffset))
                {
                    isMet &= TestEnvironment.GetFlag(nameof(MySqlCondition.SupportsOffset)) ?? true;
                }

                if (Conditions.HasFlag(MySqlCondition.SupportsHiddenColumns))
                {
                    isMet &= TestEnvironment.GetFlag(nameof(MySqlCondition.SupportsHiddenColumns)) ?? false;
                }

                if (Conditions.HasFlag(MySqlCondition.SupportsMemoryOptimized))
                {
                    isMet &= TestEnvironment.GetFlag(nameof(MySqlCondition.SupportsMemoryOptimized)) ?? false;
                }

                if (Conditions.HasFlag(MySqlCondition.IsSqlAzure))
                {
                    isMet &= TestEnvironment.IsSqlAzure;
                }

                if (Conditions.HasFlag(MySqlCondition.IsNotSqlAzure))
                {
                    isMet &= !TestEnvironment.IsSqlAzure;
                }

                if (Conditions.HasFlag(MySqlCondition.SupportsAttach))
                {
                    var defaultConnection = new SqlConnectionStringBuilder(TestEnvironment.DefaultConnection);
                    isMet &= defaultConnection.DataSource.Contains("(localdb)")
                             || defaultConnection.UserInstance;
                }

                if (Conditions.HasFlag(MySqlCondition.IsNotTeamCity))
                {
                    isMet &= !TestEnvironment.IsTeamCity;
                }

                if (Conditions.HasFlag(MySqlCondition.SupportsFullTextSearch))
                {
                    isMet &= TestEnvironment.IsFullTestSearchSupported;
                }

                return isMet;
            }
        }

        public string SkipReason =>
            // ReSharper disable once UseStringInterpolation
            string.Format(
                "The test MySql does not meet these conditions: '{0}'",
                string.Join(
                    ", ", Enum.GetValues(typeof(MySqlCondition))
                        .Cast<Enum>()
                        .Where(f => Conditions.HasFlag(f))
                        .Select(f => Enum.GetName(typeof(MySqlCondition), f))));
    }

    [Flags]
    public enum MySqlCondition
    {
        SupportsSequences = 1 << 0,
        SupportsOffset = 1 << 1,
        IsSqlAzure = 1 << 2,
        IsNotSqlAzure = 1 << 3,
        SupportsMemoryOptimized = 1 << 4,
        SupportsAttach = 1 << 5,
        SupportsHiddenColumns = 1 << 6,
        IsNotTeamCity = 1 << 7,
        SupportsFullTextSearch = 1 << 8
    }
}
