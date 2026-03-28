using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssetManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class AddDispatchDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DispatchDate",
                table: "Assets",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AssetCategories",
                keyColumn: "Id",
                keyValue: new Guid("01c7667e-c8c3-4437-9251-7f72cadd5593"),
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 3, 28, 9, 40, 24, 371, DateTimeKind.Unspecified).AddTicks(960), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "AssetCategories",
                keyColumn: "Id",
                keyValue: new Guid("7807dfa2-4e33-4c82-8736-815f5a4d8f11"),
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 3, 28, 9, 40, 24, 371, DateTimeKind.Unspecified).AddTicks(960), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "AssetCategories",
                keyColumn: "Id",
                keyValue: new Guid("e8a88ae2-f263-45cf-8086-91ff3a270431"),
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 3, 28, 9, 40, 24, 371, DateTimeKind.Unspecified).AddTicks(960), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "AssetTypes",
                keyColumn: "Id",
                keyValue: new Guid("12f30718-65e0-4426-acb9-b28e980de613"),
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 3, 28, 9, 40, 24, 371, DateTimeKind.Unspecified).AddTicks(1110), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "AssetTypes",
                keyColumn: "Id",
                keyValue: new Guid("3c16c20c-3dd8-428e-b3c3-46d65d0464b9"),
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 3, 28, 9, 40, 24, 371, DateTimeKind.Unspecified).AddTicks(1110), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "AssetTypes",
                keyColumn: "Id",
                keyValue: new Guid("5cb47334-4c76-4601-a08c-46e312ed59a7"),
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 3, 28, 9, 40, 24, 371, DateTimeKind.Unspecified).AddTicks(1070), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "AssetTypes",
                keyColumn: "Id",
                keyValue: new Guid("5e3c0b99-efda-4d23-a60c-6868d78794d2"),
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 3, 28, 9, 40, 24, 371, DateTimeKind.Unspecified).AddTicks(1070), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "AssetTypes",
                keyColumn: "Id",
                keyValue: new Guid("60891474-041a-436c-9e2e-94bb162d454d"),
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 3, 28, 9, 40, 24, 371, DateTimeKind.Unspecified).AddTicks(1130), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "AssetTypes",
                keyColumn: "Id",
                keyValue: new Guid("a1684a71-0609-49a6-b637-251de31a32c2"),
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 3, 28, 9, 40, 24, 371, DateTimeKind.Unspecified).AddTicks(1120), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "AssetTypes",
                keyColumn: "Id",
                keyValue: new Guid("a6b45145-1844-4c52-9188-a6891a34fee2"),
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 3, 28, 9, 40, 24, 371, DateTimeKind.Unspecified).AddTicks(1120), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Branches",
                keyColumn: "Id",
                keyValue: new Guid("943dad51-08de-42b3-b184-de1f272743b6"),
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 3, 28, 9, 40, 23, 548, DateTimeKind.Unspecified).AddTicks(7800), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Branches",
                keyColumn: "Id",
                keyValue: new Guid("f0bd8ca8-d641-40d1-8562-ef748bfa6ec7"),
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 3, 28, 9, 40, 23, 548, DateTimeKind.Unspecified).AddTicks(7800), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "SpecDefinitions",
                keyColumn: "Id",
                keyValue: new Guid("283067c6-7e80-4107-9b42-dab7df858814"),
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 3, 28, 9, 40, 24, 371, DateTimeKind.Unspecified).AddTicks(1340), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "SpecDefinitions",
                keyColumn: "Id",
                keyValue: new Guid("555d1ab0-cfb4-424a-bc9e-4660d8af8c78"),
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 3, 28, 9, 40, 24, 371, DateTimeKind.Unspecified).AddTicks(1330), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "SpecDefinitions",
                keyColumn: "Id",
                keyValue: new Guid("934bb7e0-7176-4929-8ae6-25b91f415084"),
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 3, 28, 9, 40, 24, 371, DateTimeKind.Unspecified).AddTicks(1340), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "SpecDefinitions",
                keyColumn: "Id",
                keyValue: new Guid("9a40815c-e5f2-4365-b870-509d514c6c70"),
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 3, 28, 9, 40, 24, 371, DateTimeKind.Unspecified).AddTicks(1350), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "SpecDefinitions",
                keyColumn: "Id",
                keyValue: new Guid("fc2e60b5-a17d-4cff-b5a3-dc8f4fe0e85d"),
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 3, 28, 9, 40, 24, 371, DateTimeKind.Unspecified).AddTicks(1350), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-4222-b222-222222222222"),
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 3, 28, 9, 40, 23, 781, DateTimeKind.Unspecified).AddTicks(5720), new TimeSpan(0, 0, 0, 0, 0)), "$2a$11$6.IBx37wAaQjraCp1dXRYebqOpWLIkYykCadFQMT667LTXOaV3Xj2", new DateTimeOffset(new DateTime(2026, 3, 28, 9, 40, 23, 781, DateTimeKind.Unspecified).AddTicks(5720), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-4333-b333-333333333333"),
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 3, 28, 9, 40, 23, 899, DateTimeKind.Unspecified).AddTicks(150), new TimeSpan(0, 0, 0, 0, 0)), "$2a$11$JeS5gXlN3FsxBT4QcxrF8eEd36p/ynWtGv42dcAaA2TxMk9CEKIg6", new DateTimeOffset(new DateTime(2026, 3, 28, 9, 40, 23, 899, DateTimeKind.Unspecified).AddTicks(150), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-b444-444444444444"),
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 3, 28, 9, 40, 24, 17, DateTimeKind.Unspecified).AddTicks(7260), new TimeSpan(0, 0, 0, 0, 0)), "$2a$11$xxCU5SFUyx2QzP246Z2XLO/hMsBLPOSDWEYSgyoBpVIHqPFyLuFJK", new DateTimeOffset(new DateTime(2026, 3, 28, 9, 40, 24, 17, DateTimeKind.Unspecified).AddTicks(7260), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-4555-b555-555555555555"),
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 3, 28, 9, 40, 24, 135, DateTimeKind.Unspecified).AddTicks(380), new TimeSpan(0, 0, 0, 0, 0)), "$2a$11$Z0gWmxiLQ88dD6glzKqyD.oQ6nl6h1gdcSaipXtwrW02CPNTDbBgG", new DateTimeOffset(new DateTime(2026, 3, 28, 9, 40, 24, 135, DateTimeKind.Unspecified).AddTicks(380), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-4666-b666-666666666666"),
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 3, 28, 9, 40, 24, 252, DateTimeKind.Unspecified).AddTicks(7290), new TimeSpan(0, 0, 0, 0, 0)), "$2a$11$TbhEw5zEWBsrWdChjLrBS.7iaM1BkBBkBcOvCerwUqR8r9Nip0Ho2", new DateTimeOffset(new DateTime(2026, 3, 28, 9, 40, 24, 252, DateTimeKind.Unspecified).AddTicks(7290), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-4777-b777-777777777777"),
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 3, 28, 9, 40, 24, 371, DateTimeKind.Unspecified).AddTicks(240), new TimeSpan(0, 0, 0, 0, 0)), "$2a$11$UpTD3IIxilUKbKTLMu9.OuO9weVDgtb.oFF67343mHD3ErmjuU4IO", new DateTimeOffset(new DateTime(2026, 3, 28, 9, 40, 24, 371, DateTimeKind.Unspecified).AddTicks(240), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("ba9ed1ab-bba9-4bf9-b527-ac21fa42f8c1"),
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 3, 28, 9, 40, 23, 664, DateTimeKind.Unspecified).AddTicks(6470), new TimeSpan(0, 0, 0, 0, 0)), "$2a$11$JMOmHo0KErzq3Glbhg0knuvYAYqmGcKD9V8u9rc1OS3Oac97ylg6q", new DateTimeOffset(new DateTime(2026, 3, 28, 9, 40, 23, 664, DateTimeKind.Unspecified).AddTicks(6470), new TimeSpan(0, 0, 0, 0, 0)) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DispatchDate",
                table: "Assets");

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
                keyValue: new Guid("44444444-4444-4444-b444-444444444444"),
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 3, 26, 7, 50, 28, 580, DateTimeKind.Unspecified).AddTicks(1686), new TimeSpan(0, 0, 0, 0, 0)), "$2a$11$iqH63QlYVjdkgB0LCywbJeVBNZOhPXa2NzX1bRypUqNJWWTLCtZnS", new DateTimeOffset(new DateTime(2026, 3, 26, 7, 50, 28, 580, DateTimeKind.Unspecified).AddTicks(1688), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-4555-b555-555555555555"),
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 3, 26, 7, 50, 28, 712, DateTimeKind.Unspecified).AddTicks(5841), new TimeSpan(0, 0, 0, 0, 0)), "$2a$11$pH3hixuYnC.qGXqbrWQSv.hdsqEC2kv1plI4hImWFaosRr4QMRQW2", new DateTimeOffset(new DateTime(2026, 3, 26, 7, 50, 28, 712, DateTimeKind.Unspecified).AddTicks(5844), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-4666-b666-666666666666"),
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 3, 26, 7, 50, 28, 852, DateTimeKind.Unspecified).AddTicks(6737), new TimeSpan(0, 0, 0, 0, 0)), "$2a$11$.M7AO4mnWZVQu42HtQUCBuXqa6Y7SwgybZxk9jBsWZTMcQwQ2PLK2", new DateTimeOffset(new DateTime(2026, 3, 26, 7, 50, 28, 852, DateTimeKind.Unspecified).AddTicks(6741), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-4777-b777-777777777777"),
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 3, 26, 7, 50, 28, 984, DateTimeKind.Unspecified).AddTicks(2342), new TimeSpan(0, 0, 0, 0, 0)), "$2a$11$mf1rzFGxjGDBj0z1yPe2pO60lts19iqUYdo2Z3HGbiW6i7/GRbye.", new DateTimeOffset(new DateTime(2026, 3, 26, 7, 50, 28, 984, DateTimeKind.Unspecified).AddTicks(2345), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("ba9ed1ab-bba9-4bf9-b527-ac21fa42f8c1"),
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 3, 26, 7, 50, 28, 182, DateTimeKind.Unspecified).AddTicks(4809), new TimeSpan(0, 0, 0, 0, 0)), "$2a$11$TslXCCJvZJ1CEtSErc31eOXrV9cQvLnS9o9qNQqbedywTW.HEyHfe", new DateTimeOffset(new DateTime(2026, 3, 26, 7, 50, 28, 182, DateTimeKind.Unspecified).AddTicks(4978), new TimeSpan(0, 0, 0, 0, 0)) });
        }
    }
}
