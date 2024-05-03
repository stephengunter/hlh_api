using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApplicationCore.Migrations
{
    /// <inheritdoc />
    public partial class JudgebookTypetablename : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files.Judgebooks_JudgebookTypes_TypeId",
                table: "Files.Judgebooks");

            migrationBuilder.DropForeignKey(
                name: "FK_JudgebookTypes_JudgebookTypes_ParentId",
                table: "JudgebookTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_JudgebookTypes",
                table: "JudgebookTypes");

            migrationBuilder.RenameTable(
                name: "JudgebookTypes",
                newName: "Files.JudgebookTypes");

            migrationBuilder.RenameIndex(
                name: "IX_JudgebookTypes_ParentId",
                table: "Files.JudgebookTypes",
                newName: "IX_Files.JudgebookTypes_ParentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Files.JudgebookTypes",
                table: "Files.JudgebookTypes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Files.Judgebooks_Files.JudgebookTypes_TypeId",
                table: "Files.Judgebooks",
                column: "TypeId",
                principalTable: "Files.JudgebookTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Files.JudgebookTypes_Files.JudgebookTypes_ParentId",
                table: "Files.JudgebookTypes",
                column: "ParentId",
                principalTable: "Files.JudgebookTypes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files.Judgebooks_Files.JudgebookTypes_TypeId",
                table: "Files.Judgebooks");

            migrationBuilder.DropForeignKey(
                name: "FK_Files.JudgebookTypes_Files.JudgebookTypes_ParentId",
                table: "Files.JudgebookTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Files.JudgebookTypes",
                table: "Files.JudgebookTypes");

            migrationBuilder.RenameTable(
                name: "Files.JudgebookTypes",
                newName: "JudgebookTypes");

            migrationBuilder.RenameIndex(
                name: "IX_Files.JudgebookTypes_ParentId",
                table: "JudgebookTypes",
                newName: "IX_JudgebookTypes_ParentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_JudgebookTypes",
                table: "JudgebookTypes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Files.Judgebooks_JudgebookTypes_TypeId",
                table: "Files.Judgebooks",
                column: "TypeId",
                principalTable: "JudgebookTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JudgebookTypes_JudgebookTypes_ParentId",
                table: "JudgebookTypes",
                column: "ParentId",
                principalTable: "JudgebookTypes",
                principalColumn: "Id");
        }
    }
}
