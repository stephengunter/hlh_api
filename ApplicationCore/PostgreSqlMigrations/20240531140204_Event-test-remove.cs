using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApplicationCore.PostgreSqlMigrations
{
    /// <inheritdoc />
    public partial class Eventtestremove : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Test",
                table: "Events");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Test",
                table: "Events",
                type: "text",
                nullable: true);
        }
    }
}
