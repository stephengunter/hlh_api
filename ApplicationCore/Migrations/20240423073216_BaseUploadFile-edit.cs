using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApplicationCore.Migrations
{
    /// <inheritdoc />
    public partial class BaseUploadFileedit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "Path",
                table: "Attachments");

            migrationBuilder.AddColumn<string>(
                name: "DirectoryPath",
                table: "Attachments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Ext",
                table: "Attachments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "Attachments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "FileSize",
                table: "Attachments",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "Host",
                table: "Attachments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DirectoryPath",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "Ext",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "FileName",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "FileSize",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "Host",
                table: "Attachments");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Attachments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Path",
                table: "Attachments",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
