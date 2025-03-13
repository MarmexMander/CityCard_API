using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CityCard_API.Migrations
{
    /// <inheritdoc />
    public partial class AddedNormalizedRoleName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "87d24e3b-489d-e621-9123-c808f97722b9",
                column: "NormalizedName",
                value: "ADMIN");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b35d45df-1693-464c-9086-17c11d02de05",
                column: "NormalizedName",
                value: "USER");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "87d24e3b-489d-e621-9123-c808f97722b9",
                column: "NormalizedName",
                value: null);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b35d45df-1693-464c-9086-17c11d02de05",
                column: "NormalizedName",
                value: null);
        }
    }
}
