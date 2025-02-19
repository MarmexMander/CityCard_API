using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CityCard_API.Migrations
{
    /// <inheritdoc />
    public partial class IdentityTypesFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1a0cf39b-43ec-49a9-8a29-68775014c2e9");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "ef6451c0-e923-4935-81d0-bdefdcb1716a", "cd8a6fd5-8ca8-490d-83a9-127580133ef1" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ef6451c0-e923-4935-81d0-bdefdcb1716a");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "cd8a6fd5-8ca8-490d-83a9-127580133ef1");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetRoles",
                type: "character varying(13)",
                maxLength: 13,
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Discriminator", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "87d24e3b-489d-e621-9123-c808f97722b9", "87d24e3b-489d-e621-9123-c808f97722b9", "CCRole", "Admin", null },
                    { "b35d45df-1693-464c-9086-17c11d02de05", "b35d45df-1693-464c-9086-17c11d02de05", "CCRole", "User", null }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Discriminator", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "d1cdc3fb-4bc1-434d-85a6-e366a0cb7664", 0, "d1cdc3fb-4bc1-434d-85a6-e366a0cb7664", "CCUser", "admin@citycard.ua", true, false, null, null, null, "AQAAAAIAAYagAAAAEIlZJu/0K1lq+uZqp2F0JMTkhm2GJV8YCgUUTyOQzA4LFarHrgIbSd6m+WA0HuffEQ==", null, false, "d1cdc3fb-4bc1-434d-85a6-e366a0cb7664", false, "admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "87d24e3b-489d-e621-9123-c808f97722b9", "d1cdc3fb-4bc1-434d-85a6-e366a0cb7664" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b35d45df-1693-464c-9086-17c11d02de05");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "87d24e3b-489d-e621-9123-c808f97722b9", "d1cdc3fb-4bc1-434d-85a6-e366a0cb7664" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "87d24e3b-489d-e621-9123-c808f97722b9");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d1cdc3fb-4bc1-434d-85a6-e366a0cb7664");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetRoles");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1a0cf39b-43ec-49a9-8a29-68775014c2e9", "1a0cf39b-43ec-49a9-8a29-68775014c2e9", "User", null },
                    { "ef6451c0-e923-4935-81d0-bdefdcb1716a", "ef6451c0-e923-4935-81d0-bdefdcb1716a", "Admin", null }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Discriminator", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "cd8a6fd5-8ca8-490d-83a9-127580133ef1", 0, "cd8a6fd5-8ca8-490d-83a9-127580133ef1", "IdentityUser", "admin@citycard.ua", true, false, null, null, null, "AQAAAAIAAYagAAAAEIlZJu/0K1lq+uZqp2F0JMTkhm2GJV8YCgUUTyOQzA4LFarHrgIbSd6m+WA0HuffEQ==", null, false, "cd8a6fd5-8ca8-490d-83a9-127580133ef1", false, "admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "ef6451c0-e923-4935-81d0-bdefdcb1716a", "cd8a6fd5-8ca8-490d-83a9-127580133ef1" });
        }
    }
}
