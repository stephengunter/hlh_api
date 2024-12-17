using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApplicationCore.Migrations
{
    /// <inheritdoc />
    public partial class SystemAppIBaseCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ParentId",
                table: "IT.SystemApps",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_IT.SystemApps_ParentId",
                table: "IT.SystemApps",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_IT.SystemApps_IT.SystemApps_ParentId",
                table: "IT.SystemApps",
                column: "ParentId",
                principalTable: "IT.SystemApps",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IT.SystemApps_IT.SystemApps_ParentId",
                table: "IT.SystemApps");

            migrationBuilder.DropIndex(
                name: "IX_IT.SystemApps_ParentId",
                table: "IT.SystemApps");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "IT.SystemApps");
        }
    }
}
