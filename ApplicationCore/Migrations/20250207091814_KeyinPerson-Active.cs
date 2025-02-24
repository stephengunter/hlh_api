using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApplicationCore.Migrations
{
    /// <inheritdoc />
    public partial class KeyinPersonActive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "Keyin.Persons",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Active",
                table: "Keyin.Persons");
        }
    }
}
