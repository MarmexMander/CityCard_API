using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CityCard_API.Migrations
{
    /// <inheritdoc />
    public partial class RemovedUserSeading : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "87d24e3b-489d-e621-9123-c808f97722b9", "d1cdc3fb-4bc1-434d-85a6-e366a0cb7664" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d1cdc3fb-4bc1-434d-85a6-e366a0cb7664");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "d1cdc3fb-4bc1-434d-85a6-e366a0cb7664", 0, "d1cdc3fb-4bc1-434d-85a6-e366a0cb7664", "admin@citycard.ua", true, false, null, null, null, "AQAAAAIAAYagAAAAEIlZJu/0K1lq+uZqp2F0JMTkhm2GJV8YCgUUTyOQzA4LFarHrgIbSd6m+WA0HuffEQ==", null, false, "d1cdc3fb-4bc1-434d-85a6-e366a0cb7664", false, "admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "87d24e3b-489d-e621-9123-c808f97722b9", "d1cdc3fb-4bc1-434d-85a6-e366a0cb7664" });
        }
    }
}
