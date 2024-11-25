using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApplicationCore.Migrations
{
    /// <inheritdoc />
    public partial class DbBackupPlanMinutesInterval : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MinutesInterval",
                table: "IT.DbBackupPlan",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StartTime",
                table: "IT.DbBackupPlan",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "IT.DbBackupPlan",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MinutesInterval",
                table: "IT.DbBackupPlan");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "IT.DbBackupPlan");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "IT.DbBackupPlan");
        }
    }
}
