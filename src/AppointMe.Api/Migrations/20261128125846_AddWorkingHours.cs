using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppointMe.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkingHours : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "working_hours",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    business_id = table.Column<Guid>(type: "uuid", nullable: false),
                    day_of_week = table.Column<int>(type: "integer", nullable: false),
                    is_closed = table.Column<bool>(type: "boolean", nullable: false),
                    open_time = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    close_time = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    break_start = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    break_end = table.Column<TimeOnly>(type: "time without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_working_hours", x => x.id);
                    table.ForeignKey(
                        name: "fk_working_hours_businesses_business_id",
                        column: x => x.business_id,
                        principalTable: "businesses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_working_hours_business_id_day_of_week",
                table: "working_hours",
                columns: new[] { "business_id", "day_of_week" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "working_hours");
        }
    }
}
