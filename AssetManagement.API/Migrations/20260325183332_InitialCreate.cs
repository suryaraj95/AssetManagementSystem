using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AssetManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssetCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AssetIdConfigs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BranchCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    CategoryCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    TypeCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    LastSequence = table.Column<int>(type: "integer", nullable: false),
                    Pattern = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetIdConfigs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Branches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Address = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Branches", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AssetTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetTypes_AssetCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "AssetCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    FullName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    EmployeeId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Role = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    BranchId = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Department = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SpecDefinitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AssetTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    SpecName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SpecDataType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    IsRequired = table.Column<bool>(type: "boolean", nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecDefinitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SpecDefinitions_AssetTypes_AssetTypeId",
                        column: x => x.AssetTypeId,
                        principalTable: "AssetTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Assets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AssetId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    SerialNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    AssetTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    BranchId = table.Column<Guid>(type: "uuid", nullable: false),
                    BrandName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Condition = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PurchaseDate = table.Column<DateTime>(type: "date", nullable: true),
                    PurchaseCost = table.Column<decimal>(type: "numeric(12,2)", nullable: true),
                    WarrantyExpiry = table.Column<DateTime>(type: "date", nullable: true),
                    AssignedEmployeeId = table.Column<Guid>(type: "uuid", nullable: true),
                    AssignedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CourierTrackingAttachment = table.Column<string>(type: "text", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Assets_AssetTypes_AssetTypeId",
                        column: x => x.AssetTypeId,
                        principalTable: "AssetTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Assets_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Assets_Users_AssignedEmployeeId",
                        column: x => x.AssignedEmployeeId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false),
                    ReferenceId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssetHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AssetId = table.Column<Guid>(type: "uuid", nullable: false),
                    Action = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PerformedById = table.Column<Guid>(type: "uuid", nullable: true),
                    FromEmployeeId = table.Column<Guid>(type: "uuid", nullable: true),
                    ToEmployeeId = table.Column<Guid>(type: "uuid", nullable: true),
                    OldStatus = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    NewStatus = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    OldCondition = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    NewCondition = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Remarks = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetHistories_Assets_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Assets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetHistories_Users_FromEmployeeId",
                        column: x => x.FromEmployeeId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssetHistories_Users_PerformedById",
                        column: x => x.PerformedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssetHistories_Users_ToEmployeeId",
                        column: x => x.ToEmployeeId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AssetRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RequestNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    RequestedById = table.Column<Guid>(type: "uuid", nullable: false),
                    RequestType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    AssetTypeId = table.Column<Guid>(type: "uuid", nullable: true),
                    ReplacementAssetId = table.Column<Guid>(type: "uuid", nullable: true),
                    Reason = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ApprovedByHrId = table.Column<Guid>(type: "uuid", nullable: true),
                    HrApprovedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ApprovedByAdminId = table.Column<Guid>(type: "uuid", nullable: true),
                    AdminApprovedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    AssignedAssetId = table.Column<Guid>(type: "uuid", nullable: true),
                    RejectionReason = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetRequests_AssetTypes_AssetTypeId",
                        column: x => x.AssetTypeId,
                        principalTable: "AssetTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AssetRequests_Assets_AssignedAssetId",
                        column: x => x.AssignedAssetId,
                        principalTable: "Assets",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AssetRequests_Assets_ReplacementAssetId",
                        column: x => x.ReplacementAssetId,
                        principalTable: "Assets",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AssetRequests_Users_ApprovedByAdminId",
                        column: x => x.ApprovedByAdminId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssetRequests_Users_ApprovedByHrId",
                        column: x => x.ApprovedByHrId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssetRequests_Users_RequestedById",
                        column: x => x.RequestedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AssetSpecValues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AssetId = table.Column<Guid>(type: "uuid", nullable: false),
                    SpecDefinitionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetSpecValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetSpecValues_Assets_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Assets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetSpecValues_SpecDefinitions_SpecDefinitionId",
                        column: x => x.SpecDefinitionId,
                        principalTable: "SpecDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AssetCategories",
                columns: new[] { "Id", "CreatedAt", "Description", "IsActive", "Name" },
                values: new object[,]
                {
                    { new Guid("01c7667e-c8c3-4437-9251-7f72cadd5593"), new DateTimeOffset(new DateTime(2026, 3, 25, 18, 33, 32, 123, DateTimeKind.Unspecified).AddTicks(5402), new TimeSpan(0, 0, 0, 0, 0)), "Tables, Chairs, etc.", true, "Furniture" },
                    { new Guid("7807dfa2-4e33-4c82-8736-815f5a4d8f11"), new DateTimeOffset(new DateTime(2026, 3, 25, 18, 33, 32, 123, DateTimeKind.Unspecified).AddTicks(5404), new TimeSpan(0, 0, 0, 0, 0)), "Keyboards, Mice, etc.", true, "Accessories" },
                    { new Guid("e8a88ae2-f263-45cf-8086-91ff3a270431"), new DateTimeOffset(new DateTime(2026, 3, 25, 18, 33, 32, 123, DateTimeKind.Unspecified).AddTicks(5246), new TimeSpan(0, 0, 0, 0, 0)), "Laptops, Mobiles, etc.", true, "Electronic Gadgets" }
                });

            migrationBuilder.InsertData(
                table: "Branches",
                columns: new[] { "Id", "Address", "Code", "CreatedAt", "IsActive", "Name" },
                values: new object[,]
                {
                    { new Guid("943dad51-08de-42b3-b184-de1f272743b6"), "Coimbatore, TN", "CBE", new DateTimeOffset(new DateTime(2026, 3, 25, 18, 33, 31, 940, DateTimeKind.Unspecified).AddTicks(8923), new TimeSpan(0, 0, 0, 0, 0)), true, "Coimbatore" },
                    { new Guid("f0bd8ca8-d641-40d1-8562-ef748bfa6ec7"), "Perundurai, TN", "PDR", new DateTimeOffset(new DateTime(2026, 3, 25, 18, 33, 31, 940, DateTimeKind.Unspecified).AddTicks(9091), new TimeSpan(0, 0, 0, 0, 0)), true, "Perundurai" }
                });

            migrationBuilder.InsertData(
                table: "AssetTypes",
                columns: new[] { "Id", "CategoryId", "CreatedAt", "IsActive", "Name" },
                values: new object[,]
                {
                    { new Guid("12f30718-65e0-4426-acb9-b28e980de613"), new Guid("7807dfa2-4e33-4c82-8736-815f5a4d8f11"), new DateTimeOffset(new DateTime(2026, 3, 25, 18, 33, 32, 123, DateTimeKind.Unspecified).AddTicks(6864), new TimeSpan(0, 0, 0, 0, 0)), true, "Mouse" },
                    { new Guid("3c16c20c-3dd8-428e-b3c3-46d65d0464b9"), new Guid("7807dfa2-4e33-4c82-8736-815f5a4d8f11"), new DateTimeOffset(new DateTime(2026, 3, 25, 18, 33, 32, 123, DateTimeKind.Unspecified).AddTicks(6862), new TimeSpan(0, 0, 0, 0, 0)), true, "Keyboard" },
                    { new Guid("5cb47334-4c76-4601-a08c-46e312ed59a7"), new Guid("e8a88ae2-f263-45cf-8086-91ff3a270431"), new DateTimeOffset(new DateTime(2026, 3, 25, 18, 33, 32, 123, DateTimeKind.Unspecified).AddTicks(6729), new TimeSpan(0, 0, 0, 0, 0)), true, "Laptops" },
                    { new Guid("5e3c0b99-efda-4d23-a60c-6868d78794d2"), new Guid("e8a88ae2-f263-45cf-8086-91ff3a270431"), new DateTimeOffset(new DateTime(2026, 3, 25, 18, 33, 32, 123, DateTimeKind.Unspecified).AddTicks(6860), new TimeSpan(0, 0, 0, 0, 0)), true, "Chargers" },
                    { new Guid("60891474-041a-436c-9e2e-94bb162d454d"), new Guid("01c7667e-c8c3-4437-9251-7f72cadd5593"), new DateTimeOffset(new DateTime(2026, 3, 25, 18, 33, 32, 123, DateTimeKind.Unspecified).AddTicks(6921), new TimeSpan(0, 0, 0, 0, 0)), true, "Chair" },
                    { new Guid("a1684a71-0609-49a6-b637-251de31a32c2"), new Guid("e8a88ae2-f263-45cf-8086-91ff3a270431"), new DateTimeOffset(new DateTime(2026, 3, 25, 18, 33, 32, 123, DateTimeKind.Unspecified).AddTicks(6869), new TimeSpan(0, 0, 0, 0, 0)), true, "Monitor" },
                    { new Guid("a6b45145-1844-4c52-9188-a6891a34fee2"), new Guid("01c7667e-c8c3-4437-9251-7f72cadd5593"), new DateTimeOffset(new DateTime(2026, 3, 25, 18, 33, 32, 123, DateTimeKind.Unspecified).AddTicks(6919), new TimeSpan(0, 0, 0, 0, 0)), true, "Table" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "BranchId", "CreatedAt", "Department", "Email", "EmployeeId", "FullName", "PasswordHash", "Phone", "Role", "Status", "UpdatedAt" },
                values: new object[] { new Guid("ba9ed1ab-bba9-4bf9-b527-ac21fa42f8c1"), new Guid("943dad51-08de-42b3-b184-de1f272743b6"), new DateTimeOffset(new DateTime(2026, 3, 25, 18, 33, 32, 123, DateTimeKind.Unspecified).AddTicks(2208), new TimeSpan(0, 0, 0, 0, 0)), null, "admin@assetorg.com", "EMP-001", "System Admin", "$2a$11$U.QwdcipbxqHIeoN70VT4.mG1VxmjyeEMjcnD/g6T5FK6pFGP0zE2", null, "Admin", "Active", new DateTimeOffset(new DateTime(2026, 3, 25, 18, 33, 32, 123, DateTimeKind.Unspecified).AddTicks(2362), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.InsertData(
                table: "SpecDefinitions",
                columns: new[] { "Id", "AssetTypeId", "CreatedAt", "DisplayOrder", "IsRequired", "SpecDataType", "SpecName" },
                values: new object[,]
                {
                    { new Guid("283067c6-7e80-4107-9b42-dab7df858814"), new Guid("5cb47334-4c76-4601-a08c-46e312ed59a7"), new DateTimeOffset(new DateTime(2026, 3, 25, 18, 33, 32, 123, DateTimeKind.Unspecified).AddTicks(8403), new TimeSpan(0, 0, 0, 0, 0)), 2, true, "text", "Processor" },
                    { new Guid("555d1ab0-cfb4-424a-bc9e-4660d8af8c78"), new Guid("5cb47334-4c76-4601-a08c-46e312ed59a7"), new DateTimeOffset(new DateTime(2026, 3, 25, 18, 33, 32, 123, DateTimeKind.Unspecified).AddTicks(8270), new TimeSpan(0, 0, 0, 0, 0)), 1, true, "text", "RAM" },
                    { new Guid("934bb7e0-7176-4929-8ae6-25b91f415084"), new Guid("5cb47334-4c76-4601-a08c-46e312ed59a7"), new DateTimeOffset(new DateTime(2026, 3, 25, 18, 33, 32, 123, DateTimeKind.Unspecified).AddTicks(8405), new TimeSpan(0, 0, 0, 0, 0)), 3, true, "text", "Storage" },
                    { new Guid("9a40815c-e5f2-4365-b870-509d514c6c70"), new Guid("5cb47334-4c76-4601-a08c-46e312ed59a7"), new DateTimeOffset(new DateTime(2026, 3, 25, 18, 33, 32, 123, DateTimeKind.Unspecified).AddTicks(8419), new TimeSpan(0, 0, 0, 0, 0)), 4, false, "text", "Screen Size" },
                    { new Guid("fc2e60b5-a17d-4cff-b5a3-dc8f4fe0e85d"), new Guid("5cb47334-4c76-4601-a08c-46e312ed59a7"), new DateTimeOffset(new DateTime(2026, 3, 25, 18, 33, 32, 123, DateTimeKind.Unspecified).AddTicks(8426), new TimeSpan(0, 0, 0, 0, 0)), 5, true, "text", "OS" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetHistories_AssetId",
                table: "AssetHistories",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetHistories_FromEmployeeId",
                table: "AssetHistories",
                column: "FromEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetHistories_PerformedById",
                table: "AssetHistories",
                column: "PerformedById");

            migrationBuilder.CreateIndex(
                name: "IX_AssetHistories_ToEmployeeId",
                table: "AssetHistories",
                column: "ToEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetRequests_ApprovedByAdminId",
                table: "AssetRequests",
                column: "ApprovedByAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetRequests_ApprovedByHrId",
                table: "AssetRequests",
                column: "ApprovedByHrId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetRequests_AssetTypeId",
                table: "AssetRequests",
                column: "AssetTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetRequests_AssignedAssetId",
                table: "AssetRequests",
                column: "AssignedAssetId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetRequests_ReplacementAssetId",
                table: "AssetRequests",
                column: "ReplacementAssetId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetRequests_RequestedById",
                table: "AssetRequests",
                column: "RequestedById");

            migrationBuilder.CreateIndex(
                name: "IX_AssetRequests_RequestNumber",
                table: "AssetRequests",
                column: "RequestNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Assets_AssetId",
                table: "Assets",
                column: "AssetId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Assets_AssetTypeId",
                table: "Assets",
                column: "AssetTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_AssignedEmployeeId",
                table: "Assets",
                column: "AssignedEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_BranchId",
                table: "Assets",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetSpecValues_AssetId_SpecDefinitionId",
                table: "AssetSpecValues",
                columns: new[] { "AssetId", "SpecDefinitionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssetSpecValues_SpecDefinitionId",
                table: "AssetSpecValues",
                column: "SpecDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetTypes_CategoryId",
                table: "AssetTypes",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Branches_Code",
                table: "Branches",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SpecDefinitions_AssetTypeId_SpecName",
                table: "SpecDefinitions",
                columns: new[] { "AssetTypeId", "SpecName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_BranchId",
                table: "Users",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_EmployeeId",
                table: "Users",
                column: "EmployeeId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetHistories");

            migrationBuilder.DropTable(
                name: "AssetIdConfigs");

            migrationBuilder.DropTable(
                name: "AssetRequests");

            migrationBuilder.DropTable(
                name: "AssetSpecValues");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "Assets");

            migrationBuilder.DropTable(
                name: "SpecDefinitions");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "AssetTypes");

            migrationBuilder.DropTable(
                name: "Branches");

            migrationBuilder.DropTable(
                name: "AssetCategories");
        }
    }
}
