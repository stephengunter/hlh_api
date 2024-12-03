using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApplicationCore.Migrations
{
    /// <inheritdoc />
    public partial class DbBackupPlanTargetHost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_IT.DbBackupPlan_TargetHostId",
                table: "IT.DbBackupPlan",
                column: "TargetHostId");

            migrationBuilder.AddForeignKey(
                name: "FK_IT.DbBackupPlan_IT.Hosts_TargetHostId",
                table: "IT.DbBackupPlan",
                column: "TargetHostId",
                principalTable: "IT.Hosts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IT.DbBackupPlan_IT.Hosts_TargetHostId",
                table: "IT.DbBackupPlan");

            migrationBuilder.DropIndex(
                name: "IX_IT.DbBackupPlan_TargetHostId",
                table: "IT.DbBackupPlan");
        }
    }
}
