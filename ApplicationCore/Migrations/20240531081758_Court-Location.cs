using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApplicationCore.Migrations
{
    /// <inheritdoc />
    public partial class CourtLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LocationId",
                table: "Courts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Courts_LocationId",
                table: "Courts",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Courts_Locations_LocationId",
                table: "Courts",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courts_Locations_LocationId",
                table: "Courts");

            migrationBuilder.DropIndex(
                name: "IX_Courts_LocationId",
                table: "Courts");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "Courts");
        }
    }
}
