using System.Collections.Generic;
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Pomelo.EntityFrameworkCore.MySql.PerfTests;
using Pomelo.EntityFrameworkCore.MySql.PerfTests.Models;

namespace Pomelo.EntityFrameworkCore.MySql.PerfTests.Migrations
{
    [DbContext(typeof(AppDb))]
    [Migration("20170727192207_initial")]
    partial class initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.1");

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(127);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(127);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasMaxLength(127);

                    b.Property<string>("ProviderKey")
                        .HasMaxLength(127);

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasMaxLength(127);

                    b.Property<string>("RoleId")
                        .HasMaxLength(127);

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasMaxLength(127);

                    b.Property<string>("LoginProvider")
                        .HasMaxLength(127);

                    b.Property<string>("Name")
                        .HasMaxLength(127);

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("Pomelo.EntityFrameworkCore.MySql.PerfTests.Models.AppIdentityUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasMaxLength(127);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(127);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(127);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(127);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("Pomelo.EntityFrameworkCore.MySql.PerfTests.Models.Blog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.ToTable("Blogs");
                });

            modelBuilder.Entity("Pomelo.EntityFrameworkCore.MySql.PerfTests.Models.BlogPost", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("BlogId");

                    b.Property<string>("Content");

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.HasIndex("BlogId");

                    b.ToTable("BlogPost");
                });

            modelBuilder.Entity("Pomelo.EntityFrameworkCore.MySql.PerfTests.Models.CrmAdmin", b =>
                {
                    b.Property<uint>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Password")
                        .HasMaxLength(20);

                    b.Property<string>("Username")
                        .HasMaxLength(10);

                    b.HasKey("Id");

                    b.ToTable("CrmAdmins");
                });

            modelBuilder.Entity("Pomelo.EntityFrameworkCore.MySql.PerfTests.Models.CrmAdminMenu", b =>
                {
                    b.Property<uint>("AdminId");

                    b.Property<uint>("MenuId");

                    b.HasKey("AdminId", "MenuId");

                    b.HasIndex("MenuId");

                    b.ToTable("CrmAdminMenu");
                });

            modelBuilder.Entity("Pomelo.EntityFrameworkCore.MySql.PerfTests.Models.CrmAdminRole", b =>
                {
                    b.Property<uint>("AdminId");

                    b.Property<uint>("RoleId");

                    b.HasKey("AdminId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("CrmAdminRole");
                });

            modelBuilder.Entity("Pomelo.EntityFrameworkCore.MySql.PerfTests.Models.CrmMenu", b =>
                {
                    b.Property<uint>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("CrmMenus");
                });

            modelBuilder.Entity("Pomelo.EntityFrameworkCore.MySql.PerfTests.Models.CrmRole", b =>
                {
                    b.Property<uint>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("CrmRoles");
                });

            modelBuilder.Entity("Pomelo.EntityFrameworkCore.MySql.PerfTests.Models.DataTypesSimple", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("TypeBool");

                    b.Property<bool?>("TypeBoolN");

                    b.Property<byte>("TypeByte");

                    b.Property<byte?>("TypeByteN");

                    b.Property<char>("TypeChar");

                    b.Property<char?>("TypeCharN");

                    b.Property<DateTime>("TypeDateTime");

                    b.Property<DateTime?>("TypeDateTimeN");

                    b.Property<DateTimeOffset>("TypeDateTimeOffset");

                    b.Property<DateTimeOffset?>("TypeDateTimeOffsetN");

                    b.Property<decimal>("TypeDecimal");

                    b.Property<decimal?>("TypeDecimalN");

                    b.Property<double>("TypeDouble");

                    b.Property<double?>("TypeDoubleN");

                    b.Property<int>("TypeEnum");

                    b.Property<byte>("TypeEnumByte");

                    b.Property<byte?>("TypeEnumByteN");

                    b.Property<int?>("TypeEnumN");

                    b.Property<float>("TypeFloat");

                    b.Property<float?>("TypeFloatN");

                    b.Property<Guid>("TypeGuid");

                    b.Property<Guid?>("TypeGuidN");

                    b.Property<int>("TypeInt");

                    b.Property<int?>("TypeIntN");

                    b.Property<long>("TypeLong");

                    b.Property<long?>("TypeLongN");

                    b.Property<sbyte>("TypeSbyte");

                    b.Property<sbyte?>("TypeSbyteN");

                    b.Property<short>("TypeShort");

                    b.Property<short?>("TypeShortN");

                    b.Property<TimeSpan>("TypeTimeSpan");

                    b.Property<TimeSpan?>("TypeTimeSpanN");

                    b.Property<uint>("TypeUint");

                    b.Property<uint?>("TypeUintN");

                    b.Property<ulong>("TypeUlong");

                    b.Property<ulong?>("TypeUlongN");

                    b.Property<ushort>("TypeUshort");

                    b.Property<ushort?>("TypeUshortN");

                    b.HasKey("Id");

                    b.ToTable("DataTypesSimple");
                });

            modelBuilder.Entity("Pomelo.EntityFrameworkCore.MySql.PerfTests.Models.DataTypesVariable", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<byte[]>("TypeByteArray")
                        .IsRequired();

                    b.Property<byte[]>("TypeByteArray255")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<byte[]>("TypeByteArray255N")
                        .HasMaxLength(255);

                    b.Property<byte[]>("TypeByteArrayN");

                    b.Property<JsonObject<List<string>>>("TypeJsonArray")
                        .IsRequired();

                    b.Property<JsonObject<List<string>>>("TypeJsonArrayN");

                    b.Property<JsonObject<Dictionary<string, string>>>("TypeJsonObject")
                        .IsRequired();

                    b.Property<JsonObject<Dictionary<string, string>>>("TypeJsonObjectN");

                    b.Property<string>("TypeString")
                        .IsRequired();

                    b.Property<string>("TypeString255")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<string>("TypeString255N")
                        .HasMaxLength(255);

                    b.Property<string>("TypeStringN");

                    b.HasKey("Id");

                    b.ToTable("DataTypesVariable");
                });

            modelBuilder.Entity("Pomelo.EntityFrameworkCore.MySql.PerfTests.Models.GeneratedContact", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Address")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType(@"VARCHAR(63) GENERATED ALWAYS AS (
						CONCAT_WS(', ',
							`ContactInfo` ->> ""$.Address"",
                            `ContactInfo` ->> ""$.City"",
                            `ContactInfo` ->> ""$.State"",
                            `ContactInfo` ->> ""$.Zip""
						)) STORED");

                    b.Property<JsonObject<Dictionary<string, string>>>("ContactInfo");

                    b.Property<string>("Email")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType(@"VARCHAR(63) GENERATED ALWAYS AS (
						`ContactInfo` ->> ""$.Email""
					) VIRTUAL");

                    b.Property<string>("Name")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType(@"VARCHAR(63) GENERATED ALWAYS AS (
						`Names` ->> ""$[0]""
					) VIRTUAL");

                    b.Property<JsonObject<List<string>>>("Names");

                    b.HasKey("Id");

                    b.HasIndex("Email");

                    b.HasIndex("Name");

                    b.ToTable("GeneratedContacts");
                });

            modelBuilder.Entity("Pomelo.EntityFrameworkCore.MySql.PerfTests.Models.GeneratedTime", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedDateTime")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("DATETIME");

                    b.Property<DateTime>("CreatedDateTime3")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("DATETIME(3)");

                    b.Property<DateTime>("CreatedDateTime6")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedTimestamp")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TIMESTAMP");

                    b.Property<DateTime>("CreatedTimestamp3")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TIMESTAMP(3)");

                    b.Property<DateTime>("CreatedTimestamp6")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TIMESTAMP(6)");

                    b.Property<string>("Name");

                    b.Property<DateTime>("UpdatedDateTime")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("DATETIME");

                    b.Property<DateTime>("UpdatedDateTime3")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("DATETIME(3)");

                    b.Property<DateTime>("UpdatedDateTime6")
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<DateTime>("UpdatedTimetamp")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("TIMESTAMP");

                    b.Property<DateTime>("UpdatedTimetamp3")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("TIMESTAMP(3)");

                    b.Property<DateTime>("UpdatedTimetamp6")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("TIMESTAMP(6)");

                    b.HasKey("Id");

                    b.ToTable("GeneratedTime");
                });

            modelBuilder.Entity("Pomelo.EntityFrameworkCore.MySql.PerfTests.Models.Person", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasMaxLength(63);

                    b.Property<int?>("FamilyId");

                    b.Property<string>("Name");

                    b.Property<int?>("TeacherId");

                    b.HasKey("Id");

                    b.HasIndex("FamilyId");

                    b.ToTable("People");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Person");
                });

            modelBuilder.Entity("Pomelo.EntityFrameworkCore.MySql.PerfTests.Models.PersonFamily", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("LastName");

                    b.HasKey("Id");

                    b.ToTable("PeopleFamilies");
                });

            modelBuilder.Entity("Pomelo.EntityFrameworkCore.MySql.PerfTests.Models.PersonKid", b =>
                {
                    b.HasBaseType("Pomelo.EntityFrameworkCore.MySql.PerfTests.Models.Person");

                    b.Property<int>("Grade");

                    b.HasIndex("Discriminator");

                    b.HasIndex("TeacherId");

                    b.ToTable("PersonKid");

                    b.HasDiscriminator().HasValue("PersonKid");
                });

            modelBuilder.Entity("Pomelo.EntityFrameworkCore.MySql.PerfTests.Models.PersonParent", b =>
                {
                    b.HasBaseType("Pomelo.EntityFrameworkCore.MySql.PerfTests.Models.Person");

                    b.Property<string>("Occupation");

                    b.Property<bool>("OnPta");

                    b.ToTable("PersonParent");

                    b.HasDiscriminator().HasValue("PersonParent");
                });

            modelBuilder.Entity("Pomelo.EntityFrameworkCore.MySql.PerfTests.Models.PersonTeacher", b =>
                {
                    b.HasBaseType("Pomelo.EntityFrameworkCore.MySql.PerfTests.Models.Person");


                    b.ToTable("PersonTeacher");

                    b.HasDiscriminator().HasValue("PersonTeacher");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole")
                        .WithMany("Claims")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Pomelo.EntityFrameworkCore.MySql.PerfTests.Models.AppIdentityUser")
                        .WithMany("Claims")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Pomelo.EntityFrameworkCore.MySql.PerfTests.Models.AppIdentityUser")
                        .WithMany("Logins")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Pomelo.EntityFrameworkCore.MySql.PerfTests.Models.AppIdentityUser")
                        .WithMany("Roles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Pomelo.EntityFrameworkCore.MySql.PerfTests.Models.BlogPost", b =>
                {
                    b.HasOne("Pomelo.EntityFrameworkCore.MySql.PerfTests.Models.Blog", "Blog")
                        .WithMany("Posts")
                        .HasForeignKey("BlogId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Pomelo.EntityFrameworkCore.MySql.PerfTests.Models.CrmAdminMenu", b =>
                {
                    b.HasOne("Pomelo.EntityFrameworkCore.MySql.PerfTests.Models.CrmAdmin", "Admin")
                        .WithMany("AdminMenus")
                        .HasForeignKey("AdminId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Pomelo.EntityFrameworkCore.MySql.PerfTests.Models.CrmMenu", "Menu")
                        .WithMany("AdminMenus")
                        .HasForeignKey("MenuId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Pomelo.EntityFrameworkCore.MySql.PerfTests.Models.CrmAdminRole", b =>
                {
                    b.HasOne("Pomelo.EntityFrameworkCore.MySql.PerfTests.Models.CrmAdmin", "Admin")
                        .WithMany("AdminRoles")
                        .HasForeignKey("AdminId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Pomelo.EntityFrameworkCore.MySql.PerfTests.Models.CrmRole", "Role")
                        .WithMany("AdminRoles")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Pomelo.EntityFrameworkCore.MySql.PerfTests.Models.Person", b =>
                {
                    b.HasOne("Pomelo.EntityFrameworkCore.MySql.PerfTests.Models.PersonFamily", "Family")
                        .WithMany("Members")
                        .HasForeignKey("FamilyId");
                });

            modelBuilder.Entity("Pomelo.EntityFrameworkCore.MySql.PerfTests.Models.PersonKid", b =>
                {
                    b.HasOne("Pomelo.EntityFrameworkCore.MySql.PerfTests.Models.PersonTeacher", "Teacher")
                        .WithMany("Students")
                        .HasForeignKey("TeacherId");
                });
        }
    }
}
