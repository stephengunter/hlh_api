using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApplicationCore.Migrations
{
    /// <inheritdoc />
    public partial class Attachment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "References",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "Height",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "PreviewPath",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "Width",
                table: "Attachments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "References",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Height",
                table: "Attachments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PreviewPath",
                table: "Attachments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Attachments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Width",
                table: "Attachments",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
