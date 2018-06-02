using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.Migrations
{
    public class MySqlModelDifferTest
    {
        [Fact]
        public void Json_column_unchanged()
        {
            Execute(
                source =>
                {
                    source.Entity(
                        "DataTypes",
                        x =>
                        {
                            x.Property<int>("Id");
                            x.Property<string>("TypeJsonObject").IsRequired().HasColumnType("json");

                            x.HasKey("Id");
                        });
                },
                target =>
                {
                    target.Entity(
                        "DataTypes",
                        x =>
                        {
                            x.Property<int>("Id");
                            x.Property<JsonObject<Dictionary<string, string>>>("TypeJsonObject").IsRequired();

                            x.HasKey("Id");
                        });
                },
                operations =>
                {
                    Assert.Equal(0, operations.Count);
                });
        }

        protected void Execute(
            Action<ModelBuilder> buildSourceAction,
            Action<ModelBuilder> buildTargetAction,
            Action<IReadOnlyList<MigrationOperation>> assertAction)
            => Execute(m => { }, buildSourceAction, buildTargetAction, assertAction, null);

        protected void Execute(
            Action<ModelBuilder> buildCommonAction,
            Action<ModelBuilder> buildSourceAction,
            Action<ModelBuilder> buildTargetAction,
            Action<IReadOnlyList<MigrationOperation>> assertAction)
            => Execute(buildCommonAction, buildSourceAction, buildTargetAction, assertAction, null);

        protected void Execute(
            Action<ModelBuilder> buildCommonAction,
            Action<ModelBuilder> buildSourceAction,
            Action<ModelBuilder> buildTargetAction,
            Action<IReadOnlyList<MigrationOperation>> assertActionUp,
            Action<IReadOnlyList<MigrationOperation>> assertActionDown)
        {
            var sourceModelBuilder = CreateModelBuilder();
            buildCommonAction(sourceModelBuilder);
            buildSourceAction(sourceModelBuilder);
            sourceModelBuilder.GetInfrastructure().Metadata.Validate();

            var targetModelBuilder = CreateModelBuilder();
            buildCommonAction(targetModelBuilder);
            buildTargetAction(targetModelBuilder);
            targetModelBuilder.GetInfrastructure().Metadata.Validate();

            var modelDiffer = CreateModelDiffer(targetModelBuilder.Model);

            var operationsUp = modelDiffer.GetDifferences(sourceModelBuilder.Model, targetModelBuilder.Model);
            assertActionUp(operationsUp);

            if (assertActionDown != null)
            {
                modelDiffer = CreateModelDiffer(sourceModelBuilder.Model);

                var operationsDown = modelDiffer.GetDifferences(targetModelBuilder.Model, sourceModelBuilder.Model);
                assertActionDown(operationsDown);
            }
        }

        protected virtual TestHelpers TestHelpers => MySqlTestHelpers.Instance;
        protected virtual ModelBuilder CreateModelBuilder() => TestHelpers.CreateConventionBuilder();

        protected virtual IMigrationsModelDiffer CreateModelDiffer(IModel model)
        {
            var ctx = TestHelpers.CreateContext(
                TestHelpers.AddProviderOptions(new DbContextOptionsBuilder())
                    .UseModel(model).EnableSensitiveDataLogging().Options);
            return ctx.GetService<IMigrationsModelDiffer>();
        }
    }
}
