using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurveyBasket.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDisabledColumnToUsersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDisabled",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 1,
                column: "ClaimType",
                value: "permissions");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 2,
                column: "ClaimType",
                value: "permissions");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 3,
                column: "ClaimType",
                value: "permissions");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 4,
                column: "ClaimType",
                value: "permissions");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 5,
                column: "ClaimType",
                value: "permissions");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 6,
                column: "ClaimType",
                value: "permissions");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 7,
                column: "ClaimType",
                value: "permissions");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 8,
                column: "ClaimType",
                value: "permissions");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 9,
                column: "ClaimType",
                value: "permissions");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 10,
                column: "ClaimType",
                value: "permissions");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 11,
                column: "ClaimType",
                value: "permissions");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 12,
                column: "ClaimType",
                value: "permissions");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 13,
                column: "ClaimType",
                value: "permissions");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 14,
                column: "ClaimType",
                value: "permissions");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2b3708dc-02f3-43e9-971c-483d248b64e6",
                column: "IsDisabled",
                value: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDisabled",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 1,
                column: "ClaimType",
                value: "Permissions");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 2,
                column: "ClaimType",
                value: "Permissions");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 3,
                column: "ClaimType",
                value: "Permissions");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 4,
                column: "ClaimType",
                value: "Permissions");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 5,
                column: "ClaimType",
                value: "Permissions");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 6,
                column: "ClaimType",
                value: "Permissions");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 7,
                column: "ClaimType",
                value: "Permissions");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 8,
                column: "ClaimType",
                value: "Permissions");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 9,
                column: "ClaimType",
                value: "Permissions");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 10,
                column: "ClaimType",
                value: "Permissions");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 11,
                column: "ClaimType",
                value: "Permissions");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 12,
                column: "ClaimType",
                value: "Permissions");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 13,
                column: "ClaimType",
                value: "Permissions");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 14,
                column: "ClaimType",
                value: "Permissions");
        }
    }
}
