using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AppointMe.Api.Migrations
{
    /// <inheritdoc />
    public partial class SeedRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "id", "concurrency_stamp", "name", "normalized_name" },
                values: new object[,]
                {
                    { new Guid("8d04dce2-969a-435d-bba4-df3f325983dc"), "8d04dce2-969a-435d-bba4-df3f325983dc", "Customer", "CUSTOMER" },
                    { new Guid("b6d1b5f0-3c4e-4a8f-9d2e-6f7a1c2b3d4e"), "b6d1b5f0-3c4e-4a8f-9d2e-6f7a1c2b3d4e", "BusinessOwner", "BUSINESSOWNER" },
                    { new Guid("e2a7c9d4-5b1f-4e3a-8c6d-0f9b2a1e7c35"), "e2a7c9d4-5b1f-4e3a-8c6d-0f9b2a1e7c35", "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "roles",
                keyColumn: "id",
                keyValue: new Guid("8d04dce2-969a-435d-bba4-df3f325983dc"));

            migrationBuilder.DeleteData(
                table: "roles",
                keyColumn: "id",
                keyValue: new Guid("b6d1b5f0-3c4e-4a8f-9d2e-6f7a1c2b3d4e"));

            migrationBuilder.DeleteData(
                table: "roles",
                keyColumn: "id",
                keyValue: new Guid("e2a7c9d4-5b1f-4e3a-8c6d-0f9b2a1e7c35"));
        }
    }
}
