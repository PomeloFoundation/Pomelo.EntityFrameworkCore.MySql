// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

// ReSharper disable UnassignedGetOnlyAutoProperty

// ReSharper disable ClassNeverInstantiated.Local
// ReSharper disable MemberCanBePrivate.Local
namespace Microsoft.EntityFrameworkCore
{
    public class MySqlDatabaseCreatorTest
    {
        [ConditionalFact]
        public Task Create_checks_for_existence_and_retries_if_no_proccess_until_it_passes()
        {
            return Create_checks_for_existence_and_retries_until_it_passes(233, async: false);
        }

        [ConditionalFact]
        public Task Create_checks_for_existence_and_retries_if_timeout_until_it_passes()
        {
            return Create_checks_for_existence_and_retries_until_it_passes(-2, async: false);
        }

        [ConditionalFact]
        public Task Create_checks_for_existence_and_retries_if_cannot_open_until_it_passes()
        {
            return Create_checks_for_existence_and_retries_until_it_passes(4060, async: false);
        }

        [ConditionalFact]
        public Task Create_checks_for_existence_and_retries_if_cannot_attach_file_until_it_passes()
        {
            return Create_checks_for_existence_and_retries_until_it_passes(1832, async: false);
        }

        [ConditionalFact]
        public Task Create_checks_for_existence_and_retries_if_cannot_open_file_until_it_passes()
        {
            return Create_checks_for_existence_and_retries_until_it_passes(5120, async: false);
        }

        [ConditionalFact]
        public Task CreateAsync_checks_for_existence_and_retries_if_no_proccess_until_it_passes()
        {
            return Create_checks_for_existence_and_retries_until_it_passes(233, async: true);
        }

        [ConditionalFact]
        public Task CreateAsync_checks_for_existence_and_retries_if_timeout_until_it_passes()
        {
            return Create_checks_for_existence_and_retries_until_it_passes(-2, async: true);
        }

        [ConditionalFact]
        public Task CreateAsync_checks_for_existence_and_retries_if_cannot_open_until_it_passes()
        {
            return Create_checks_for_existence_and_retries_until_it_passes(4060, async: true);
        }

        [ConditionalFact]
        public Task CreateAsync_checks_for_existence_and_retries_if_cannot_attach_file_until_it_passes()
        {
            return Create_checks_for_existence_and_retries_until_it_passes(1832, async: true);
        }

        [ConditionalFact]
        public Task CreateAsync_checks_for_existence_and_retries_if_cannot_open_file_until_it_passes()
        {
            return Create_checks_for_existence_and_retries_until_it_passes(5120, async: true);
        }

        private async Task Create_checks_for_existence_and_retries_until_it_passes(int errorNumber, bool async)
        {
            var customServices = new ServiceCollection()
                .AddScoped<IMySqlConnection, FakeMySqlConnection>()
                .AddScoped<IRelationalCommandBuilderFactory, FakeRelationalCommandBuilderFactory>()
                .AddScoped<IExecutionStrategyFactory, ExecutionStrategyFactory>();

            var contextServices = MySqlTestHelpers.Instance.CreateContextServices(customServices);

            var connection = (FakeMySqlConnection)contextServices.GetRequiredService<IMySqlConnection>();

            connection.ErrorNumber = errorNumber;
            connection.FailureCount = 2;

            var creator = (MySqlDatabaseCreator)contextServices.GetRequiredService<IRelationalDatabaseCreator>();

            creator.RetryDelay = TimeSpan.FromMilliseconds(1);
            creator.RetryTimeout = TimeSpan.FromMinutes(5);

            if (async)
            {
                await creator.CreateAsync();
            }
            else
            {
                creator.Create();
            }

            Assert.Equal(2, connection.OpenCount);
        }

        [ConditionalFact]
        public Task Create_checks_for_existence_and_ultimately_gives_up_waiting()
        {
            return Create_checks_for_existence_and_ultimately_gives_up_waiting_test(async: false);
        }

        [ConditionalFact]
        public Task CreateAsync_checks_for_existence_and_ultimately_gives_up_waiting()
        {
            return Create_checks_for_existence_and_ultimately_gives_up_waiting_test(async: true);
        }

        private async Task Create_checks_for_existence_and_ultimately_gives_up_waiting_test(bool async)
        {
            var customServices = new ServiceCollection()
                .AddScoped<IMySqlConnection, FakeMySqlConnection>()
                .AddScoped<IRelationalCommandBuilderFactory, FakeRelationalCommandBuilderFactory>()
                .AddScoped<IExecutionStrategyFactory, ExecutionStrategyFactory>();

            var contextServices = MySqlTestHelpers.Instance.CreateContextServices(customServices);

            var connection = (FakeMySqlConnection)contextServices.GetRequiredService<IMySqlConnection>();

            connection.ErrorNumber = 233;
            connection.FailureCount = 100;
            connection.FailDelay = 50;

            var creator = (MySqlDatabaseCreator)contextServices.GetRequiredService<IRelationalDatabaseCreator>();

            creator.RetryDelay = TimeSpan.FromMilliseconds(5);
            creator.RetryTimeout = TimeSpan.FromMilliseconds(100);

            if (async)
            {
                await Assert.ThrowsAsync<SqlException>(() => creator.CreateAsync());
            }
            else
            {
                Assert.Throws<SqlException>(() => creator.Create());
            }
        }

        private class FakeMySqlConnection : MySqlConnection
        {
            private readonly IDbContextOptions _options;

            public FakeMySqlConnection(IDbContextOptions options, RelationalConnectionDependencies dependencies)
                : base(dependencies)
            {
                _options = options;
            }

            public int ErrorNumber { get; set; }
            public int FailureCount { get; set; }
            public int FailDelay { get; set; }
            public int OpenCount { get; set; }

            public override bool Open(bool errorsExpected = false)
            {
                if (++OpenCount < FailureCount)
                {
                    Thread.Sleep(FailDelay);
                    throw SqlExceptionFactory.CreateSqlException(ErrorNumber);
                }

                return true;
            }

            public override async Task<bool> OpenAsync(CancellationToken cancellationToken, bool errorsExpected = false)
            {
                if (++OpenCount < FailureCount)
                {
                    await Task.Delay(FailDelay, cancellationToken);
                    throw SqlExceptionFactory.CreateSqlException(ErrorNumber);
                }

                return await Task.FromResult(true);
            }

            public override IMySqlConnection CreateMasterConnection()
                => new FakeMySqlConnection(_options, Dependencies);
        }

        private class FakeRelationalCommandBuilderFactory : IRelationalCommandBuilderFactory
        {
            public IRelationalCommandBuilder Create()
                => new FakeRelationalCommandBuilder();
        }

        private class FakeRelationalCommandBuilder : IRelationalCommandBuilder
        {
            private readonly List<IRelationalParameter> _parameters = new List<IRelationalParameter>();
            public IndentedStringBuilder Instance { get; } = new IndentedStringBuilder();

            public IReadOnlyList<IRelationalParameter> Parameters => _parameters;

            public IRelationalCommandBuilder AddParameter(IRelationalParameter parameter)
            {
                _parameters.Add(parameter);

                return this;
            }

            public IRelationalTypeMappingSource TypeMappingSource => null;

            public IRelationalCommand Build() => new FakeRelationalCommand();

            public IRelationalCommandBuilder Append(object value)
            {
                Instance.Append(value);

                return this;
            }

            public IRelationalCommandBuilder AppendLine()
            {
                Instance.AppendLine();

                return this;
            }

            public IRelationalCommandBuilder IncrementIndent()
            {
                Instance.IncrementIndent();

                return this;
            }

            public IRelationalCommandBuilder DecrementIndent()
            {
                Instance.DecrementIndent();

                return this;
            }

            public int CommandTextLength => Instance.Length;
        }

        private class FakeRelationalCommand : IRelationalCommand
        {
            public string CommandText { get; }

            public IReadOnlyList<IRelationalParameter> Parameters { get; }

            public IReadOnlyDictionary<string, object> ParameterValues => throw new NotImplementedException();

            public int ExecuteNonQuery(RelationalCommandParameterObject parameterObject)
            {
                return 0;
            }

            public Task<int> ExecuteNonQueryAsync(
                RelationalCommandParameterObject parameterObject,
                CancellationToken cancellationToken = default)
                => Task.FromResult(0);

            public RelationalDataReader ExecuteReader(RelationalCommandParameterObject parameterObject)
            {
                throw new NotImplementedException();
            }

            public Task<RelationalDataReader> ExecuteReaderAsync(
                RelationalCommandParameterObject parameterObject,
                CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException();
            }

            public object ExecuteScalar(RelationalCommandParameterObject parameterObject)
            {
                throw new NotImplementedException();
            }

            public Task<object> ExecuteScalarAsync(
                RelationalCommandParameterObject parameterObject,
                CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException();
            }
        }
    }
}
