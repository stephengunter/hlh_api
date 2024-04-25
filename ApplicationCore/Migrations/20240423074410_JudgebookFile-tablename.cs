using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApplicationCore.Migrations
{
    /// <inheritdoc />
    public partial class JudgebookFiletablename : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_JudgebookFiles",
                table: "JudgebookFiles");

            migrationBuilder.RenameTable(
                name: "JudgebookFiles",
                newName: "Files.Judgebooks");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Files.Judgebooks",
                table: "Files.Judgebooks",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Files.Judgebooks",
                table: "Files.Judgebooks");

            migrationBuilder.RenameTable(
                name: "Files.Judgebooks",
                newName: "JudgebookFiles");

            migrationBuilder.AddPrimaryKey(
                name: "PK_JudgebookFiles",
                table: "JudgebookFiles",
                column: "Id");
        }
    }
}
