using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApplicationCore.Migrations
{
    /// <inheritdoc />
    public partial class DbBackupTasksadd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IT.DbBackupPlan_IT.Databases_DatabaseId",
                table: "IT.DbBackupPlan");

            migrationBuilder.DropForeignKey(
                name: "FK_IT.DbBackupPlan_IT.Servers_TargetServerId",
                table: "IT.DbBackupPlan");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IT.DbBackupPlan",
                table: "IT.DbBackupPlan");

            migrationBuilder.RenameTable(
                name: "IT.DbBackupPlan",
                newName: "IT.DbBackupPlans");

            migrationBuilder.RenameIndex(
                name: "IX_IT.DbBackupPlan_TargetServerId",
                table: "IT.DbBackupPlans",
                newName: "IX_IT.DbBackupPlans_TargetServerId");

            migrationBuilder.RenameIndex(
                name: "IX_IT.DbBackupPlan_DatabaseId",
                table: "IT.DbBackupPlans",
                newName: "IX_IT.DbBackupPlans_DatabaseId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_IT.DbBackupPlans",
                table: "IT.DbBackupPlans",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "IT.DbBackupTasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlanId = table.Column<int>(type: "int", nullable: false),
                    Ps = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TargetServerId = table.Column<int>(type: "int", nullable: true),
                    TargetPath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Done = table.Column<bool>(type: "bit", nullable: false),
                    Error = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IT.DbBackupTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IT.DbBackupTasks_IT.DbBackupPlans_PlanId",
                        column: x => x.PlanId,
                        principalTable: "IT.DbBackupPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IT.DbBackupTasks_PlanId",
                table: "IT.DbBackupTasks",
                column: "PlanId");

            migrationBuilder.AddForeignKey(
                name: "FK_IT.DbBackupPlans_IT.Databases_DatabaseId",
                table: "IT.DbBackupPlans",
                column: "DatabaseId",
                principalTable: "IT.Databases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IT.DbBackupPlans_IT.Servers_TargetServerId",
                table: "IT.DbBackupPlans",
                column: "TargetServerId",
                principalTable: "IT.Servers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IT.DbBackupPlans_IT.Databases_DatabaseId",
                table: "IT.DbBackupPlans");

            migrationBuilder.DropForeignKey(
                name: "FK_IT.DbBackupPlans_IT.Servers_TargetServerId",
                table: "IT.DbBackupPlans");

            migrationBuilder.DropTable(
                name: "IT.DbBackupTasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IT.DbBackupPlans",
                table: "IT.DbBackupPlans");

            migrationBuilder.RenameTable(
                name: "IT.DbBackupPlans",
                newName: "IT.DbBackupPlan");

            migrationBuilder.RenameIndex(
                name: "IX_IT.DbBackupPlans_TargetServerId",
                table: "IT.DbBackupPlan",
                newName: "IX_IT.DbBackupPlan_TargetServerId");

            migrationBuilder.RenameIndex(
                name: "IX_IT.DbBackupPlans_DatabaseId",
                table: "IT.DbBackupPlan",
                newName: "IX_IT.DbBackupPlan_DatabaseId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_IT.DbBackupPlan",
                table: "IT.DbBackupPlan",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_IT.DbBackupPlan_IT.Databases_DatabaseId",
                table: "IT.DbBackupPlan",
                column: "DatabaseId",
                principalTable: "IT.Databases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IT.DbBackupPlan_IT.Servers_TargetServerId",
                table: "IT.DbBackupPlan",
                column: "TargetServerId",
                principalTable: "IT.Servers",
                principalColumn: "Id");
        }
    }
}
