using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApplicationCore.Migrations
{
    /// <inheritdoc />
    public partial class JudgebookTypeadd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Files.Judgebooks");

            migrationBuilder.AddColumn<int>(
                name: "TypeId",
                table: "Files.Judgebooks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "JudgebookTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Key = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    Removed = table.Column<bool>(type: "bit", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JudgebookTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JudgebookTypes_JudgebookTypes_ParentId",
                        column: x => x.ParentId,
                        principalTable: "JudgebookTypes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Files.Judgebooks_TypeId",
                table: "Files.Judgebooks",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_JudgebookTypes_ParentId",
                table: "JudgebookTypes",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Files.Judgebooks_JudgebookTypes_TypeId",
                table: "Files.Judgebooks",
                column: "TypeId",
                principalTable: "JudgebookTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files.Judgebooks_JudgebookTypes_TypeId",
                table: "Files.Judgebooks");

            migrationBuilder.DropTable(
                name: "JudgebookTypes");

            migrationBuilder.DropIndex(
                name: "IX_Files.Judgebooks_TypeId",
                table: "Files.Judgebooks");

            migrationBuilder.DropColumn(
                name: "TypeId",
                table: "Files.Judgebooks");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Files.Judgebooks",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
