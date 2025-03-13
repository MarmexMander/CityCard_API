using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CityCard_API.Migrations
{
    /// <inheritdoc />
    public partial class RemovedCustomRoleType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetRoles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "87d24e3b-489d-e621-9123-c808f97722b9");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b35d45df-1693-464c-9086-17c11d02de05");

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
        }
    }
}
