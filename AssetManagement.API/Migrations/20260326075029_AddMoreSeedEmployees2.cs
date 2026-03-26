using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AssetManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class AddMoreSeedEmployees2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AssetCategories",
                keyColumn: "Id",
                keyValue: new Guid("01c7667e-c8c3-4437-9251-7f72cadd5593"),
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 3, 26, 7, 50, 28, 984, DateTimeKind.Unspecified).AddTicks(7567), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "AssetCategories",
                keyColumn: "Id",
                keyValue: new Guid("7807dfa2-4e33-4c82-8736-815f5a4d8f11"),
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 3, 26, 7, 50, 28, 984, DateTimeKind.Unspecified).AddTicks(7570), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "AssetCategories",
                keyColumn: "Id",
                keyValue: new Guid("e8a88ae2-f263-45cf-8086-91ff3a270431"),
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 3, 26, 7, 50, 28, 984, DateTimeKind.Unspecified).AddTicks(7280), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "AssetTypes",
                keyColumn: "Id",
                keyValue: new Guid("12f30718-65e0-4426-acb9-b28e980de613"),
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 3, 26, 7, 50, 28, 984, DateTimeKind.Unspecified).AddTicks(9947), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "AssetTypes",
                keyColumn: "Id",
                keyValue: new Guid("3c16c20c-3dd8-428e-b3c3-46d65d0464b9"),
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 3, 26, 7, 50, 28, 984, DateTimeKind.Unspecified).AddTicks(9933), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "AssetTypes",
                keyColumn: "Id",
                keyValue: new Guid("5cb47334-4c76-4601-a08c-46e312ed59a7"),
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 3, 26, 7, 50, 28, 984, DateTimeKind.Unspecified).AddTicks(9698), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "AssetTypes",
                keyColumn: "Id",
                keyValue: new Guid("5e3c0b99-efda-4d23-a60c-6868d78794d2"),
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 3, 26, 7, 50, 28, 984, DateTimeKind.Unspecified).AddTicks(9925), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "AssetTypes",
                keyColumn: "Id",
                keyValue: new Guid("60891474-041a-436c-9e2e-94bb162d454d"),
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 3, 26, 7, 50, 28, 984, DateTimeKind.Unspecified).AddTicks(9954), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "AssetTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1684a71-0609-49a6-b637-251de31a32c2"),
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 3, 26, 7, 50, 28, 984, DateTimeKind.Unspecified).AddTicks(9950), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "AssetTypes",
                keyColumn: "Id",
                keyValue: new Guid("a6b45145-1844-4c52-9188-a6891a34fee2"),
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 3, 26, 7, 50, 28, 984, DateTimeKind.Unspecified).AddTicks(9952), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Branches",
                keyColumn: "Id",
                keyValue: new Guid("943dad51-08de-42b3-b184-de1f272743b6"),
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 3, 26, 7, 50, 27, 968, DateTimeKind.Unspecified).AddTicks(514), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Branches",
                keyColumn: "Id",
                keyValue: new Guid("f0bd8ca8-d641-40d1-8562-ef748bfa6ec7"),
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 3, 26, 7, 50, 27, 968, DateTimeKind.Unspecified).AddTicks(797), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "SpecDefinitions",
                keyColumn: "Id",
                keyValue: new Guid("283067c6-7e80-4107-9b42-dab7df858814"),
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 3, 26, 7, 50, 28, 985, DateTimeKind.Unspecified).AddTicks(2535), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "SpecDefinitions",
                keyColumn: "Id",
                keyValue: new Guid("555d1ab0-cfb4-424a-bc9e-4660d8af8c78"),
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 3, 26, 7, 50, 28, 985, DateTimeKind.Unspecified).AddTicks(2315), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "SpecDefinitions",
                keyColumn: "Id",
                keyValue: new Guid("934bb7e0-7176-4929-8ae6-25b91f415084"),
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 3, 26, 7, 50, 28, 985, DateTimeKind.Unspecified).AddTicks(2538), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "SpecDefinitions",
                keyColumn: "Id",
                keyValue: new Guid("9a40815c-e5f2-4365-b870-509d514c6c70"),
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 3, 26, 7, 50, 28, 985, DateTimeKind.Unspecified).AddTicks(2541), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "SpecDefinitions",
                keyColumn: "Id",
                keyValue: new Guid("fc2e60b5-a17d-4cff-b5a3-dc8f4fe0e85d"),
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 3, 26, 7, 50, 28, 985, DateTimeKind.Unspecified).AddTicks(2547), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-4222-b222-222222222222"),
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 3, 26, 7, 50, 28, 315, DateTimeKind.Unspecified).AddTicks(3867), new TimeSpan(0, 0, 0, 0, 0)), "$2a$11$7yWi1lBJozMSk6.CpEiTI.Y1oHa8/qy5XzrCuGdiPGwCHDoW2JzcC", new DateTimeOffset(new DateTime(2026, 3, 26, 7, 50, 28, 315, DateTimeKind.Unspecified).AddTicks(3873), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-4333-b333-333333333333"),
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 3, 26, 7, 50, 28, 446, DateTimeKind.Unspecified).AddTicks(9896), new TimeSpan(0, 0, 0, 0, 0)), "$2a$11$zxrFFNCherbEt3kkGmqvceaqnBnObKsoveajcdS.EmP5w4MW/8ceC", new DateTimeOffset(new DateTime(2026, 3, 26, 7, 50, 28, 446, DateTimeKind.Unspecified).AddTicks(9898), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("ba9ed1ab-bba9-4bf9-b527-ac21fa42f8c1"),
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 3, 26, 7, 50, 28, 182, DateTimeKind.Unspecified).AddTicks(4809), new TimeSpan(0, 0, 0, 0, 0)), "$2a$11$TslXCCJvZJ1CEtSErc31eOXrV9cQvLnS9o9qNQqbedywTW.HEyHfe", new DateTimeOffset(new DateTime(2026, 3, 26, 7, 50, 28, 182, DateTimeKind.Unspecified).AddTicks(4978), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "BranchId", "CreatedAt", "Department", "Email", "EmployeeId", "FullName", "PasswordHash", "Phone", "Role", "Status", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("44444444-4444-4444-b444-444444444444"), new Guid("943dad51-08de-42b3-b184-de1f272743b6"), new DateTimeOffset(new DateTime(2026, 3, 26, 7, 50, 28, 580, DateTimeKind.Unspecified).AddTicks(1686), new TimeSpan(0, 0, 0, 0, 0)), null, "johndoe@assetorg.com", "EMP-004", "John Doe", "$2a$11$iqH63QlYVjdkgB0LCywbJeVBNZOhPXa2NzX1bRypUqNJWWTLCtZnS", null, "Employee", "Active", new DateTimeOffset(new DateTime(2026, 3, 26, 7, 50, 28, 580, DateTimeKind.Unspecified).AddTicks(1688), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("55555555-5555-4555-b555-555555555555"), new Guid("f0bd8ca8-d641-40d1-8562-ef748bfa6ec7"), new DateTimeOffset(new DateTime(2026, 3, 26, 7, 50, 28, 712, DateTimeKind.Unspecified).AddTicks(5841), new TimeSpan(0, 0, 0, 0, 0)), null, "janesmith@assetorg.com", "EMP-005", "Jane Smith", "$2a$11$pH3hixuYnC.qGXqbrWQSv.hdsqEC2kv1plI4hImWFaosRr4QMRQW2", null, "Employee", "Active", new DateTimeOffset(new DateTime(2026, 3, 26, 7, 50, 28, 712, DateTimeKind.Unspecified).AddTicks(5844), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("66666666-6666-4666-b666-666666666666"), new Guid("943dad51-08de-42b3-b184-de1f272743b6"), new DateTimeOffset(new DateTime(2026, 3, 26, 7, 50, 28, 852, DateTimeKind.Unspecified).AddTicks(6737), new TimeSpan(0, 0, 0, 0, 0)), null, "mikej@assetorg.com", "EMP-006", "Mike Johnson", "$2a$11$.M7AO4mnWZVQu42HtQUCBuXqa6Y7SwgybZxk9jBsWZTMcQwQ2PLK2", null, "Employee", "Active", new DateTimeOffset(new DateTime(2026, 3, 26, 7, 50, 28, 852, DateTimeKind.Unspecified).AddTicks(6741), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("77777777-7777-4777-b777-777777777777"), new Guid("f0bd8ca8-d641-40d1-8562-ef748bfa6ec7"), new DateTimeOffset(new DateTime(2026, 3, 26, 7, 50, 28, 984, DateTimeKind.Unspecified).AddTicks(2342), new TimeSpan(0, 0, 0, 0, 0)), null, "emilyd@assetorg.com", "EMP-007", "Emily Davis", "$2a$11$mf1rzFGxjGDBj0z1yPe2pO60lts19iqUYdo2Z3HGbiW6i7/GRbye.", null, "Employee", "Active", new DateTimeOffset(new DateTime(2026, 3, 26, 7, 50, 28, 984, DateTimeKind.Unspecified).AddTicks(2345), new TimeSpan(0, 0, 0, 0, 0)) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-b444-444444444444"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-4555-b555-555555555555"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-4666-b666-666666666666"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-4777-b777-777777777777"));

            migrationBuilder.UpdateData(
                table: "AssetCategories",
                keyColumn: "Id",
                keyValue: new Guid("01c7667e-c8c3-4437-9251-7f72cadd5593"),
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 3, 26, 6, 8, 20, 392, DateTimeKind.Unspecified).AddTicks(8117), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "AssetCategories",
                keyColumn: "Id",
                keyValue: new Guid("7807dfa2-4e33-4c82-8736-815f5a4d8f11"),
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 3, 26, 6, 8, 20, 392, DateTimeKind.Unspecified).AddTicks(8119), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "AssetCategories",
                keyColumn: "Id",
                keyValue: new Guid("e8a88ae2-f263-45cf-8086-91ff3a270431"),
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 3, 26, 6, 8, 20, 392, DateTimeKind.Unspecified).AddTicks(7840), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "AssetTypes",
                keyColumn: "Id",
                keyValue: new Guid("12f30718-65e0-4426-acb9-b28e980de613"),
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 3, 26, 6, 8, 20, 393, DateTimeKind.Unspecified).AddTicks(1991), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "AssetTypes",
                keyColumn: "Id",
                keyValue: new Guid("3c16c20c-3dd8-428e-b3c3-46d65d0464b9"),
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 3, 26, 6, 8, 20, 393, DateTimeKind.Unspecified).AddTicks(1880), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "AssetTypes",
                keyColumn: "Id",
                keyValue: new Guid("5cb47334-4c76-4601-a08c-46e312ed59a7"),
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 3, 26, 6, 8, 20, 393, DateTimeKind.Unspecified).AddTicks(1613), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "AssetTypes",
                keyColumn: "Id",
                keyValue: new Guid("5e3c0b99-efda-4d23-a60c-6868d78794d2"),
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 3, 26, 6, 8, 20, 393, DateTimeKind.Unspecified).AddTicks(1878), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "AssetTypes",
                keyColumn: "Id",
                keyValue: new Guid("60891474-041a-436c-9e2e-94bb162d454d"),
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 3, 26, 6, 8, 20, 393, DateTimeKind.Unspecified).AddTicks(2024), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "AssetTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1684a71-0609-49a6-b637-251de31a32c2"),
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 3, 26, 6, 8, 20, 393, DateTimeKind.Unspecified).AddTicks(1995), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "AssetTypes",
                keyColumn: "Id",
                keyValue: new Guid("a6b45145-1844-4c52-9188-a6891a34fee2"),
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 3, 26, 6, 8, 20, 393, DateTimeKind.Unspecified).AddTicks(1998), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Branches",
                keyColumn: "Id",
                keyValue: new Guid("943dad51-08de-42b3-b184-de1f272743b6"),
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 3, 26, 6, 8, 19, 669, DateTimeKind.Unspecified).AddTicks(5449), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Branches",
                keyColumn: "Id",
                keyValue: new Guid("f0bd8ca8-d641-40d1-8562-ef748bfa6ec7"),
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 3, 26, 6, 8, 19, 669, DateTimeKind.Unspecified).AddTicks(5846), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "SpecDefinitions",
                keyColumn: "Id",
                keyValue: new Guid("283067c6-7e80-4107-9b42-dab7df858814"),
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 3, 26, 6, 8, 20, 393, DateTimeKind.Unspecified).AddTicks(4956), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "SpecDefinitions",
                keyColumn: "Id",
                keyValue: new Guid("555d1ab0-cfb4-424a-bc9e-4660d8af8c78"),
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 3, 26, 6, 8, 20, 393, DateTimeKind.Unspecified).AddTicks(4671), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "SpecDefinitions",
                keyColumn: "Id",
                keyValue: new Guid("934bb7e0-7176-4929-8ae6-25b91f415084"),
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 3, 26, 6, 8, 20, 393, DateTimeKind.Unspecified).AddTicks(4959), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "SpecDefinitions",
                keyColumn: "Id",
                keyValue: new Guid("9a40815c-e5f2-4365-b870-509d514c6c70"),
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 3, 26, 6, 8, 20, 393, DateTimeKind.Unspecified).AddTicks(4962), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "SpecDefinitions",
                keyColumn: "Id",
                keyValue: new Guid("fc2e60b5-a17d-4cff-b5a3-dc8f4fe0e85d"),
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 3, 26, 6, 8, 20, 393, DateTimeKind.Unspecified).AddTicks(4967), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-4222-b222-222222222222"),
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 3, 26, 6, 8, 20, 259, DateTimeKind.Unspecified).AddTicks(120), new TimeSpan(0, 0, 0, 0, 0)), "$2a$11$X8Z7NQ19Ir9sivrcT95v3utVODiwWnh3sLACoKMSO61w34VZYql8u", new DateTimeOffset(new DateTime(2026, 3, 26, 6, 8, 20, 259, DateTimeKind.Unspecified).AddTicks(127), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-4333-b333-333333333333"),
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 3, 26, 6, 8, 20, 391, DateTimeKind.Unspecified).AddTicks(4863), new TimeSpan(0, 0, 0, 0, 0)), "$2a$11$2hI5EFn1462Q6qMWerj6vOAknAzeAUeZz4aQk96ZuX2DHIVQETEf6", new DateTimeOffset(new DateTime(2026, 3, 26, 6, 8, 20, 391, DateTimeKind.Unspecified).AddTicks(4866), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("ba9ed1ab-bba9-4bf9-b527-ac21fa42f8c1"),
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 3, 26, 6, 8, 19, 998, DateTimeKind.Unspecified).AddTicks(9008), new TimeSpan(0, 0, 0, 0, 0)), "$2a$11$nh5j.QP07UHftDVR/aili.bb2L0D9oitwGRJVjMxHn.cHVTo1c.6e", new DateTimeOffset(new DateTime(2026, 3, 26, 6, 8, 19, 998, DateTimeKind.Unspecified).AddTicks(9556), new TimeSpan(0, 0, 0, 0, 0)) });
        }
    }
}
