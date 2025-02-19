using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CityCard_API.Migrations
{
    /// <inheritdoc />
    public partial class IdntityDB_fix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUsers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d1cdc3fb-4bc1-434d-85a6-e366a0cb7664");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUsers",
                type: "character varying(13)",
                maxLength: 13,
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Discriminator", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "d1cdc3fb-4bc1-434d-85a6-e366a0cb7664", 0, "d1cdc3fb-4bc1-434d-85a6-e366a0cb7664", "CCUser", "admin@citycard.ua", true, false, null, null, null, "AQAAAAIAAYagAAAAEIlZJu/0K1lq+uZqp2F0JMTkhm2GJV8YCgUUTyOQzA4LFarHrgIbSd6m+WA0HuffEQ==", null, false, "d1cdc3fb-4bc1-434d-85a6-e366a0cb7664", false, "admin" });
        }
    }
}
