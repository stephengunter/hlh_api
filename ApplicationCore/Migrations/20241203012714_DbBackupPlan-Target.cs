using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApplicationCore.Migrations
{
    /// <inheritdoc />
    public partial class DbBackupPlanTarget : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IT.DbBackupPlan_IT.Hosts_TargetHostId",
                table: "IT.DbBackupPlan");

            migrationBuilder.RenameColumn(
                name: "TargetHostId",
                table: "IT.DbBackupPlan",
                newName: "TargetServerId");

            migrationBuilder.RenameIndex(
                name: "IX_IT.DbBackupPlan_TargetHostId",
                table: "IT.DbBackupPlan",
                newName: "IX_IT.DbBackupPlan_TargetServerId");

            migrationBuilder.AddForeignKey(
                name: "FK_IT.DbBackupPlan_IT.Servers_TargetServerId",
                table: "IT.DbBackupPlan",
                column: "TargetServerId",
                principalTable: "IT.Servers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IT.DbBackupPlan_IT.Servers_TargetServerId",
                table: "IT.DbBackupPlan");

            migrationBuilder.RenameColumn(
                name: "TargetServerId",
                table: "IT.DbBackupPlan",
                newName: "TargetHostId");

            migrationBuilder.RenameIndex(
                name: "IX_IT.DbBackupPlan_TargetServerId",
                table: "IT.DbBackupPlan",
                newName: "IX_IT.DbBackupPlan_TargetHostId");

            migrationBuilder.AddForeignKey(
                name: "FK_IT.DbBackupPlan_IT.Hosts_TargetHostId",
                table: "IT.DbBackupPlan",
                column: "TargetHostId",
                principalTable: "IT.Hosts",
                principalColumn: "Id");
        }
    }
}
