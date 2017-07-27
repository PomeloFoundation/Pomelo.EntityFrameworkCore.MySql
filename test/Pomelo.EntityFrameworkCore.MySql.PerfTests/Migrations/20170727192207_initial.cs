﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Pomelo.EntityFrameworkCore.MySql.PerfTests.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false)
                        .Annotation("MySql:ValueGeneratedOnAdd", true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    Name = table.Column<string>(maxLength: 127, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 127, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(maxLength: 127, nullable: false),
                    LoginProvider = table.Column<string>(maxLength: 127, nullable: false),
                    Name = table.Column<string>(maxLength: 127, nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false)
                        .Annotation("MySql:ValueGeneratedOnAdd", true),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    Email = table.Column<string>(maxLength: 127, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 127, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 127, nullable: true),
                    PasswordHash = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    SecurityStamp = table.Column<string>(nullable: true),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    UserName = table.Column<string>(maxLength: 127, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Blogs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGeneratedOnAdd", true),
                    Title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CrmAdmins",
                columns: table => new
                {
                    Id = table.Column<uint>(nullable: false)
                        .Annotation("MySql:ValueGeneratedOnAdd", true),
                    Password = table.Column<string>(maxLength: 20, nullable: true),
                    Username = table.Column<string>(maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrmAdmins", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CrmMenus",
                columns: table => new
                {
                    Id = table.Column<uint>(nullable: false)
                        .Annotation("MySql:ValueGeneratedOnAdd", true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrmMenus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CrmRoles",
                columns: table => new
                {
                    Id = table.Column<uint>(nullable: false)
                        .Annotation("MySql:ValueGeneratedOnAdd", true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrmRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DataTypesSimple",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGeneratedOnAdd", true),
                    TypeBool = table.Column<bool>(nullable: false),
                    TypeBoolN = table.Column<bool>(nullable: true),
                    TypeByte = table.Column<byte>(nullable: false),
                    TypeByteN = table.Column<byte>(nullable: true),
                    TypeChar = table.Column<char>(nullable: false),
                    TypeCharN = table.Column<char>(nullable: true),
                    TypeDateTime = table.Column<DateTime>(nullable: false),
                    TypeDateTimeN = table.Column<DateTime>(nullable: true),
                    TypeDateTimeOffset = table.Column<DateTimeOffset>(nullable: false),
                    TypeDateTimeOffsetN = table.Column<DateTimeOffset>(nullable: true),
                    TypeDecimal = table.Column<decimal>(nullable: false),
                    TypeDecimalN = table.Column<decimal>(nullable: true),
                    TypeDouble = table.Column<double>(nullable: false),
                    TypeDoubleN = table.Column<double>(nullable: true),
                    TypeEnum = table.Column<int>(nullable: false),
                    TypeEnumByte = table.Column<byte>(nullable: false),
                    TypeEnumByteN = table.Column<byte>(nullable: true),
                    TypeEnumN = table.Column<int>(nullable: true),
                    TypeFloat = table.Column<float>(nullable: false),
                    TypeFloatN = table.Column<float>(nullable: true),
                    TypeGuid = table.Column<Guid>(nullable: false),
                    TypeGuidN = table.Column<Guid>(nullable: true),
                    TypeInt = table.Column<int>(nullable: false),
                    TypeIntN = table.Column<int>(nullable: true),
                    TypeLong = table.Column<long>(nullable: false),
                    TypeLongN = table.Column<long>(nullable: true),
                    TypeSbyte = table.Column<sbyte>(nullable: false),
                    TypeSbyteN = table.Column<sbyte>(nullable: true),
                    TypeShort = table.Column<short>(nullable: false),
                    TypeShortN = table.Column<short>(nullable: true),
                    TypeTimeSpan = table.Column<TimeSpan>(nullable: false),
                    TypeTimeSpanN = table.Column<TimeSpan>(nullable: true),
                    TypeUint = table.Column<uint>(nullable: false),
                    TypeUintN = table.Column<uint>(nullable: true),
                    TypeUlong = table.Column<ulong>(nullable: false),
                    TypeUlongN = table.Column<ulong>(nullable: true),
                    TypeUshort = table.Column<ushort>(nullable: false),
                    TypeUshortN = table.Column<ushort>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataTypesSimple", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DataTypesVariable",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGeneratedOnAdd", true),
                    TypeByteArray = table.Column<byte[]>(nullable: false),
                    TypeByteArray255 = table.Column<byte[]>(maxLength: 255, nullable: false),
                    TypeByteArray255N = table.Column<byte[]>(maxLength: 255, nullable: true),
                    TypeByteArrayN = table.Column<byte[]>(nullable: true),
                    TypeJsonArray = table.Column<JsonObject<List<string>>>(nullable: false),
                    TypeJsonArrayN = table.Column<JsonObject<List<string>>>(nullable: true),
                    TypeJsonObject = table.Column<JsonObject<Dictionary<string, string>>>(nullable: false),
                    TypeJsonObjectN = table.Column<JsonObject<Dictionary<string, string>>>(nullable: true),
                    TypeString = table.Column<string>(nullable: false),
                    TypeString255 = table.Column<string>(maxLength: 255, nullable: false),
                    TypeString255N = table.Column<string>(maxLength: 255, nullable: true),
                    TypeStringN = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataTypesVariable", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GeneratedContacts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false)
                        .Annotation("MySql:ValueGeneratedOnAdd", true),
                    Address = table.Column<string>(type: @"VARCHAR(63) GENERATED ALWAYS AS (
						CONCAT_WS(', ',
							`ContactInfo` ->> ""$.Address"",
                            `ContactInfo` ->> ""$.City"",
                            `ContactInfo` ->> ""$.State"",
                            `ContactInfo` ->> ""$.Zip""
						)) STORED", nullable: true)
                        .Annotation("MySql:ValueGeneratedOnAddOrUpdate", true),
                    ContactInfo = table.Column<JsonObject<Dictionary<string, string>>>(nullable: true),
                    Email = table.Column<string>(type: @"VARCHAR(63) GENERATED ALWAYS AS (
						`ContactInfo` ->> ""$.Email""
					) VIRTUAL", nullable: true)
                        .Annotation("MySql:ValueGeneratedOnAddOrUpdate", true),
                    Name = table.Column<string>(type: @"VARCHAR(63) GENERATED ALWAYS AS (
						`Names` ->> ""$[0]""
					) VIRTUAL", nullable: true)
                        .Annotation("MySql:ValueGeneratedOnAddOrUpdate", true),
                    Names = table.Column<JsonObject<List<string>>>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneratedContacts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GeneratedTime",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGeneratedOnAdd", true),
                    CreatedDateTime = table.Column<DateTime>(type: "DATETIME", nullable: false)
                        .Annotation("MySql:ValueGeneratedOnAdd", true),
                    CreatedDateTime3 = table.Column<DateTime>(type: "DATETIME(3)", nullable: false)
                        .Annotation("MySql:ValueGeneratedOnAdd", true),
                    CreatedDateTime6 = table.Column<DateTime>(nullable: false)
                        .Annotation("MySql:ValueGeneratedOnAdd", true),
                    CreatedTimestamp = table.Column<DateTime>(type: "TIMESTAMP", nullable: false)
                        .Annotation("MySql:ValueGeneratedOnAdd", true),
                    CreatedTimestamp3 = table.Column<DateTime>(type: "TIMESTAMP(3)", nullable: false)
                        .Annotation("MySql:ValueGeneratedOnAdd", true),
                    CreatedTimestamp6 = table.Column<DateTime>(type: "TIMESTAMP(6)", nullable: false)
                        .Annotation("MySql:ValueGeneratedOnAdd", true),
                    Name = table.Column<string>(nullable: true),
                    UpdatedDateTime = table.Column<DateTime>(type: "DATETIME", nullable: false)
                        .Annotation("MySql:ValueGeneratedOnAddOrUpdate", true),
                    UpdatedDateTime3 = table.Column<DateTime>(type: "DATETIME(3)", nullable: false)
                        .Annotation("MySql:ValueGeneratedOnAddOrUpdate", true),
                    UpdatedDateTime6 = table.Column<DateTime>(nullable: false)
                        .Annotation("MySql:ValueGeneratedOnAddOrUpdate", true),
                    UpdatedTimetamp = table.Column<DateTime>(type: "TIMESTAMP", nullable: false)
                        .Annotation("MySql:ValueGeneratedOnAddOrUpdate", true),
                    UpdatedTimetamp3 = table.Column<DateTime>(type: "TIMESTAMP(3)", nullable: false)
                        .Annotation("MySql:ValueGeneratedOnAddOrUpdate", true),
                    UpdatedTimetamp6 = table.Column<DateTime>(type: "TIMESTAMP(6)", nullable: false)
                        .Annotation("MySql:ValueGeneratedOnAddOrUpdate", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneratedTime", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PeopleFamilies",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGeneratedOnAdd", true),
                    LastName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PeopleFamilies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGeneratedOnAdd", true),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGeneratedOnAdd", true),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(maxLength: 127, nullable: false),
                    ProviderKey = table.Column<string>(maxLength: 127, nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(maxLength: 127, nullable: false),
                    RoleId = table.Column<string>(maxLength: 127, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BlogPost",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGeneratedOnAdd", true),
                    BlogId = table.Column<int>(nullable: false),
                    Content = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogPost", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BlogPost_Blogs_BlogId",
                        column: x => x.BlogId,
                        principalTable: "Blogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CrmAdminMenu",
                columns: table => new
                {
                    AdminId = table.Column<uint>(nullable: false),
                    MenuId = table.Column<uint>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrmAdminMenu", x => new { x.AdminId, x.MenuId });
                    table.ForeignKey(
                        name: "FK_CrmAdminMenu_CrmAdmins_AdminId",
                        column: x => x.AdminId,
                        principalTable: "CrmAdmins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CrmAdminMenu_CrmMenus_MenuId",
                        column: x => x.MenuId,
                        principalTable: "CrmMenus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CrmAdminRole",
                columns: table => new
                {
                    AdminId = table.Column<uint>(nullable: false),
                    RoleId = table.Column<uint>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrmAdminRole", x => new { x.AdminId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_CrmAdminRole_CrmAdmins_AdminId",
                        column: x => x.AdminId,
                        principalTable: "CrmAdmins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CrmAdminRole_CrmRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "CrmRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "People",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGeneratedOnAdd", true),
                    Discriminator = table.Column<string>(maxLength: 63, nullable: false),
                    FamilyId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    TeacherId = table.Column<int>(nullable: true),
                    Grade = table.Column<int>(nullable: true),
                    Occupation = table.Column<string>(nullable: true),
                    OnPta = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_People", x => x.Id);
                    table.ForeignKey(
                        name: "FK_People_PeopleFamilies_FamilyId",
                        column: x => x.FamilyId,
                        principalTable: "PeopleFamilies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_People_People_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BlogPost_BlogId",
                table: "BlogPost",
                column: "BlogId");

            migrationBuilder.CreateIndex(
                name: "IX_CrmAdminMenu_MenuId",
                table: "CrmAdminMenu",
                column: "MenuId");

            migrationBuilder.CreateIndex(
                name: "IX_CrmAdminRole_RoleId",
                table: "CrmAdminRole",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_GeneratedContacts_Email",
                table: "GeneratedContacts",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_GeneratedContacts_Name",
                table: "GeneratedContacts",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_People_FamilyId",
                table: "People",
                column: "FamilyId");

            migrationBuilder.CreateIndex(
                name: "IX_People_Discriminator",
                table: "People",
                column: "Discriminator");

            migrationBuilder.CreateIndex(
                name: "IX_People_TeacherId",
                table: "People",
                column: "TeacherId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "BlogPost");

            migrationBuilder.DropTable(
                name: "CrmAdminMenu");

            migrationBuilder.DropTable(
                name: "CrmAdminRole");

            migrationBuilder.DropTable(
                name: "DataTypesSimple");

            migrationBuilder.DropTable(
                name: "DataTypesVariable");

            migrationBuilder.DropTable(
                name: "GeneratedContacts");

            migrationBuilder.DropTable(
                name: "GeneratedTime");

            migrationBuilder.DropTable(
                name: "People");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Blogs");

            migrationBuilder.DropTable(
                name: "CrmMenus");

            migrationBuilder.DropTable(
                name: "CrmAdmins");

            migrationBuilder.DropTable(
                name: "CrmRoles");

            migrationBuilder.DropTable(
                name: "PeopleFamilies");
        }
    }
}
