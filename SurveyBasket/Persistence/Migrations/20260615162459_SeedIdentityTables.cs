using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SurveyBasket.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedIdentityTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "IsDefault", "IsDeleted", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "44de9d22-fd50-40e1-a5ce-3d22c23dcca4", "18e4d39d-ca3f-4fe9-929a-ddf00935d3c0", false, false, "Admin", "ADMIN" },
                    { "73b8342a-0a45-4811-9563-6cdcdb8bb6f3", "054e565a-ea77-4c00-a637-e25def9cae4b", true, false, "Member", "MEMBER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "2b3708dc-02f3-43e9-971c-483d248b64e6", 0, "95ff9891-348a-42cd-a2f2-4def7be5bcab", "admin@survey-basket.com", true, "Admin", "User", false, null, "ADMIN@SURVEY-BASKET.COM", "ADMIN@SURVEY-BASKET.COM", "AQAAAAIAAYagAAAAEAq0Q0hY08fU2ESndaVUqNTEEcnGUmGDL/Ra2VAVuDFBNmXbcpDt6iLw9ttMjxzs0A==", null, false, "82F0552C44C54A2FAE4BAD46B6F2A344", false, "admin@survey-basket.com" });

            migrationBuilder.InsertData(
                table: "AspNetRoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "RoleId" },
                values: new object[,]
                {
                    { 1, "Permissions", "poll:read", "44de9d22-fd50-40e1-a5ce-3d22c23dcca4" },
                    { 2, "Permissions", "poll:add", "44de9d22-fd50-40e1-a5ce-3d22c23dcca4" },
                    { 3, "Permissions", "poll:update", "44de9d22-fd50-40e1-a5ce-3d22c23dcca4" },
                    { 4, "Permissions", "poll:delete", "44de9d22-fd50-40e1-a5ce-3d22c23dcca4" },
                    { 5, "Permissions", "question:read", "44de9d22-fd50-40e1-a5ce-3d22c23dcca4" },
                    { 6, "Permissions", "question:add", "44de9d22-fd50-40e1-a5ce-3d22c23dcca4" },
                    { 7, "Permissions", "question:update", "44de9d22-fd50-40e1-a5ce-3d22c23dcca4" },
                    { 8, "Permissions", "user:read", "44de9d22-fd50-40e1-a5ce-3d22c23dcca4" },
                    { 9, "Permissions", "user:add", "44de9d22-fd50-40e1-a5ce-3d22c23dcca4" },
                    { 10, "Permissions", "user:update", "44de9d22-fd50-40e1-a5ce-3d22c23dcca4" },
                    { 11, "Permissions", "role:read", "44de9d22-fd50-40e1-a5ce-3d22c23dcca4" },
                    { 12, "Permissions", "role:add", "44de9d22-fd50-40e1-a5ce-3d22c23dcca4" },
                    { 13, "Permissions", "role:update", "44de9d22-fd50-40e1-a5ce-3d22c23dcca4" },
                    { 14, "Permissions", "result:read", "44de9d22-fd50-40e1-a5ce-3d22c23dcca4" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "44de9d22-fd50-40e1-a5ce-3d22c23dcca4", "2b3708dc-02f3-43e9-971c-483d248b64e6" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "73b8342a-0a45-4811-9563-6cdcdb8bb6f3");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "44de9d22-fd50-40e1-a5ce-3d22c23dcca4", "2b3708dc-02f3-43e9-971c-483d248b64e6" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "44de9d22-fd50-40e1-a5ce-3d22c23dcca4");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2b3708dc-02f3-43e9-971c-483d248b64e6");
        }
    }
}
