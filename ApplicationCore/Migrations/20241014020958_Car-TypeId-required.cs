using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApplicationCore.Migrations
{
    /// <inheritdoc />
    public partial class CarTypeIdrequired : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cars.Cars_Cars.Types_TypeId",
                table: "Cars.Cars");

            migrationBuilder.AlterColumn<int>(
                name: "TypeId",
                table: "Cars.Cars",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Cars.Cars_Cars.Types_TypeId",
                table: "Cars.Cars",
                column: "TypeId",
                principalTable: "Cars.Types",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cars.Cars_Cars.Types_TypeId",
                table: "Cars.Cars");

            migrationBuilder.AlterColumn<int>(
                name: "TypeId",
                table: "Cars.Cars",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Cars.Cars_Cars.Types_TypeId",
                table: "Cars.Cars",
                column: "TypeId",
                principalTable: "Cars.Types",
                principalColumn: "Id");
        }
    }
}
