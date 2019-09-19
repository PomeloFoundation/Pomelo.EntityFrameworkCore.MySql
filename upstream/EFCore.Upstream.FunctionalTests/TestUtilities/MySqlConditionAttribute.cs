// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.TestUtilities.Xunit;

namespace Microsoft.EntityFrameworkCore.TestUtilities
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public sealed class MySqlConditionAttribute : Attribute, ITestCondition
    {
        public MySqlCondition Conditions { get; set; }

        public MySqlConditionAttribute(MySqlCondition conditions)
        {
            Conditions = conditions;
        }

        public ValueTask<bool> IsMetAsync()
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

            if (Conditions.HasFlag(MySqlCondition.IsNotCI))
            {
                isMet &= !TestEnvironment.IsCI;
            }

            if (Conditions.HasFlag(MySqlCondition.SupportsFullTextSearch))
            {
                isMet &= TestEnvironment.IsFullTestSearchSupported;
            }

            return new ValueTask<bool>(isMet);
        }

        public string SkipReason =>
            // ReSharper disable once UseStringInterpolation
            string.Format(
                "The test SQL Server does not meet these conditions: '{0}'",
                string.Join(
                    ", ", Enum.GetValues(typeof(MySqlCondition))
                        .Cast<Enum>()
                        .Where(f => Conditions.HasFlag(f))
                        .Select(f => Enum.GetName(typeof(MySqlCondition), f))));
    }
}
