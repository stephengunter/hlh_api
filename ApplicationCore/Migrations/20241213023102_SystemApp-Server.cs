using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApplicationCore.Migrations
{
    /// <inheritdoc />
    public partial class SystemAppServer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CredentialInfoId",
                table: "IT.SystemApps");

            migrationBuilder.DropColumn(
                name: "HostId",
                table: "IT.SystemApps");

            migrationBuilder.AddColumn<int>(
                name: "ServerId",
                table: "IT.SystemApps",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IT.SystemApps_IT.Servers_ServerId",
                table: "IT.SystemApps");

            migrationBuilder.DropIndex(
                name: "IX_IT.SystemApps_ServerId",
                table: "IT.SystemApps");

            migrationBuilder.DropColumn(
                name: "ServerId",
                table: "IT.SystemApps");

            migrationBuilder.AddColumn<string>(
                name: "CredentialInfoId",
                table: "IT.SystemApps",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "HostId",
                table: "IT.SystemApps",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
