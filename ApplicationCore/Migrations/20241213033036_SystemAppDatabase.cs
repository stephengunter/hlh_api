using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApplicationCore.Migrations
{
    /// <inheritdoc />
    public partial class SystemAppDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IT.SystemApps_IT.Servers_ServerId",
                table: "IT.SystemApps");

            migrationBuilder.DropIndex(
                name: "IX_IT.SystemApps_ServerId",
                table: "IT.SystemApps");

            migrationBuilder.AddColumn<int>(
                name: "Importance",
                table: "IT.SystemApps",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "SystemAppDatabases",
                columns: table => new
                {
                    SystemAppId = table.Column<int>(type: "int", nullable: false),
                    DatabaseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemAppDatabases", x => new { x.SystemAppId, x.DatabaseId });
                    table.ForeignKey(
                        name: "FK_SystemAppDatabases_IT.Databases_DatabaseId",
                        column: x => x.DatabaseId,
                        principalTable: "IT.Databases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SystemAppDatabases_IT.SystemApps_SystemAppId",
                        column: x => x.SystemAppId,
                        principalTable: "IT.SystemApps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SystemAppDatabases_DatabaseId",
                table: "SystemAppDatabases",
                column: "DatabaseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SystemAppDatabases");

            migrationBuilder.DropColumn(
                name: "Importance",
                table: "IT.SystemApps");

            migrationBuilder.CreateIndex(
                name: "IX_IT.SystemApps_ServerId",
                table: "IT.SystemApps",
                column: "ServerId");

            migrationBuilder.AddForeignKey(
                name: "FK_IT.SystemApps_IT.Servers_ServerId",
                table: "IT.SystemApps",
                column: "ServerId",
                principalTable: "IT.Servers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
