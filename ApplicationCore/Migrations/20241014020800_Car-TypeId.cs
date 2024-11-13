using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApplicationCore.Migrations
{
    /// <inheritdoc />
    public partial class CarTypeId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TypeId",
                table: "Cars.Cars",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cars.Cars_TypeId",
                table: "Cars.Cars",
                column: "TypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cars.Cars_Cars.Types_TypeId",
                table: "Cars.Cars",
                column: "TypeId",
                principalTable: "Cars.Types",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cars.Cars_Cars.Types_TypeId",
                table: "Cars.Cars");

            migrationBuilder.DropIndex(
                name: "IX_Cars.Cars_TypeId",
                table: "Cars.Cars");

            migrationBuilder.DropColumn(
                name: "TypeId",
                table: "Cars.Cars");
        }
    }
}
