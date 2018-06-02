using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Migrations.Design;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.Logging;
using Pomelo.EntityFrameworkCore.MySql.Internal;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;
using Pomelo.EntityFrameworkCore.TestUtilities;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.Migrations
{
    public class ModelSnapshotMySqlTest
    {
        [Fact]
        public virtual void Column_types_for_some_string_properties_are_stored_in_the_snapshot()
        {
            Test(
                builder =>
                {
                    builder.Entity<DataTypesVariable>(eb =>
                    {
                        // Need to specify the type until EF#12212 is fixed
                        eb.Property(e => e.TypeJsonArray).HasColumnType("json");
                        eb.Property(e => e.TypeJsonArrayN).HasColumnType("json");
                        eb.Property(e => e.TypeJsonObject).HasColumnType("json");
                        eb.Property(e => e.TypeJsonObjectN).HasColumnType("json");
                    });
                },
                @"builder
    .HasAnnotation(""Relational:MaxIdentifierLength"", 64);

builder.Entity(""Pomelo.EntityFrameworkCore.MySql.Migrations.ModelSnapshotMySqlTest+DataTypesVariable"", b =>
    {
        b.Property<int>(""Id"")
            .ValueGeneratedOnAdd();

        b.Property<byte[]>(""TypeByteArray"")
            .IsRequired();

        b.Property<byte[]>(""TypeByteArray255"")
            .IsRequired()
            .HasMaxLength(255);

        b.Property<byte[]>(""TypeByteArray255N"")
            .HasMaxLength(255);

        b.Property<byte[]>(""TypeByteArrayN"");

        b.Property<string>(""TypeJsonArray"")
            .IsRequired()
            .HasColumnType(""json"");

        b.Property<string>(""TypeJsonArrayN"")
            .HasColumnType(""json"");

        b.Property<string>(""TypeJsonObject"")
            .IsRequired()
            .HasColumnType(""json"");

        b.Property<string>(""TypeJsonObjectN"")
            .HasColumnType(""json"");

        b.Property<string>(""TypeString"")
            .IsRequired();

        b.Property<string>(""TypeString255"")
            .IsRequired()
            .HasMaxLength(255);

        b.Property<string>(""TypeString255N"")
            .HasMaxLength(255);

        b.Property<string>(""TypeStringN"");

        b.HasKey(""Id"");

        b.ToTable(""DataTypesVariable"");
    });
",
                o => { Assert.Null(o.GetEntityTypes().First().FindProperty("Id")[CoreAnnotationNames.ValueGeneratorFactoryAnnotation]); }
                );
        }

        public class DataTypesVariable
        {
            public int Id { get; set; }

            // string not null
            [Required]
            public string TypeString { get; set; }

            [Required]
            [MaxLength(255)]
            public string TypeString255 { get; set; }

            // string null
            public string TypeStringN { get; set; }

            [MaxLength(255)]
            public string TypeString255N { get; set; }


            // binary not null
            [Required]
            [MaxLength(255)]
            public byte[] TypeByteArray255 { get; set; }

            [Required]
            public byte[] TypeByteArray { get; set; }

            // binary null
            [MaxLength(255)]
            public byte[] TypeByteArray255N { get; set; }

            public byte[] TypeByteArrayN { get; set; }


            // json not null
            [Required]
            public JsonObject<List<string>> TypeJsonArray { get; set; }

            [Required]
            public JsonObject<Dictionary<string, string>> TypeJsonObject { get; set; }

            // json null
            public JsonObject<List<string>> TypeJsonArrayN { get; set; }

            public JsonObject<Dictionary<string, string>> TypeJsonObjectN { get; set; }
        }

        protected virtual ICollection<BuildReference> GetReferences() => new List<BuildReference>
        {
            BuildReference.ByName("Microsoft.EntityFrameworkCore"),
            BuildReference.ByName("Microsoft.EntityFrameworkCore.Relational"),
            BuildReference.ByName("Pomelo.EntityFrameworkCore.MySql")
        };

        protected void Test(Action<ModelBuilder> buildModel, string expectedCode, Action<IModel> assert)
        {
            var modelBuilder = CreateConventionalModelBuilder();
            modelBuilder.HasChangeTrackingStrategy(ChangeTrackingStrategy.Snapshot);
            buildModel(modelBuilder);

            modelBuilder.GetInfrastructure().Metadata.Validate();
            CreateModelValidator().Validate(modelBuilder.Model);
            var model = modelBuilder.Model;

            var generator = new CSharpSnapshotGenerator(new CSharpSnapshotGeneratorDependencies(new CSharpHelper()));

            var builder = new IndentedStringBuilder();
            generator.Generate("builder", model, builder);
            var code = builder.ToString();

            Assert.Equal(expectedCode, code, ignoreLineEndingDifferences: true);

            var build = new BuildSource
            {
                Sources =
                {
                    @"
                    using System;
                    using Microsoft.EntityFrameworkCore;
                    using Microsoft.EntityFrameworkCore.Metadata;
                    using Microsoft.EntityFrameworkCore.Metadata.Conventions;
                    using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

                    public static class ModelSnapshot
                    {
                        public static IModel Model
                        {
                            get
                            {
                                var builder = new ModelBuilder(new ConventionSet());
                                " + code + @"

                                return builder.Model;
                            }
                        }
                   }
                "
                }
            };

            foreach (var buildReference in GetReferences())
            {
                build.References.Add(buildReference);
            }

            var assembly = build.BuildInMemory();
            var factoryType = assembly.GetType("ModelSnapshot");
            var property = factoryType.GetTypeInfo().GetDeclaredProperty("Model");
            var value = (IModel)property.GetValue(null);

            Assert.NotNull(value);
            assert(value);
        }

        protected ModelBuilder CreateConventionalModelBuilder() => new ModelBuilder(MySqlConventionSetBuilder.Build());

        protected ModelValidator CreateModelValidator()
            => new RelationalModelValidator(
                new ModelValidatorDependencies(
                    new DiagnosticsLogger<DbLoggerCategory.Model.Validation>(
                        new ListLoggerFactory(new List<(LogLevel, EventId, string)>(), l => l == DbLoggerCategory.Model.Validation.Name),
                        new LoggingOptions(),
                        new DiagnosticListener("Fake")),
                    new DiagnosticsLogger<DbLoggerCategory.Model>(
                        new ListLoggerFactory(new List<(LogLevel, EventId, string)>(), l => l == DbLoggerCategory.Model.Name),
                        new LoggingOptions(),
                        new DiagnosticListener("Fake"))),
                new RelationalModelValidatorDependencies(
#pragma warning disable 618
                    TestServiceFactory.Instance.Create<ObsoleteRelationalTypeMapper>(),
#pragma warning restore 618
                    new MySqlTypeMappingSource(
                        TestServiceFactory.Instance.Create<TypeMappingSourceDependencies>(),
                        TestServiceFactory.Instance.Create<RelationalTypeMappingSourceDependencies>(),
                        new MySqlOptions())));
    }
}
