using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApplicationCore.Migrations
{
    /// <inheritdoc />
    public partial class DbBackupPlanTargetPath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TargetHostId",
                table: "IT.DbBackupPlan",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TargetPath",
                table: "IT.DbBackupPlan",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TargetHostId",
                table: "IT.DbBackupPlan");

            migrationBuilder.DropColumn(
                name: "TargetPath",
                table: "IT.DbBackupPlan");
        }
    }
}
