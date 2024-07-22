using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApplicationCore.PostgreSqlMigrations
{
    /// <inheritdoc />
    public partial class Calendarorder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "Calendars",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Order",
                table: "Calendars");
        }
    }
}
