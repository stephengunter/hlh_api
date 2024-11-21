using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApplicationCore.Migrations
{
    /// <inheritdoc />
    public partial class Serversadd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IT.Databases_IT.Hosts_HostId",
                table: "IT.Databases");

            migrationBuilder.DropColumn(
                name: "CredentialInfoId",
                table: "IT.Databases");

            migrationBuilder.RenameColumn(
                name: "HostId",
                table: "IT.Databases",
                newName: "ServerId");

            migrationBuilder.RenameIndex(
                name: "IX_IT.Databases_HostId",
                table: "IT.Databases",
                newName: "IX_IT.Databases_ServerId");

            migrationBuilder.CreateTable(
                name: "IT.Servers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HostId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ps = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Removed = table.Column<bool>(type: "bit", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IT.Servers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IT.Servers_IT.Hosts_HostId",
                        column: x => x.HostId,
                        principalTable: "IT.Hosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IT.Servers_HostId",
                table: "IT.Servers",
                column: "HostId");

            migrationBuilder.AddForeignKey(
                name: "FK_IT.Databases_IT.Servers_ServerId",
                table: "IT.Databases",
                column: "ServerId",
                principalTable: "IT.Servers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IT.Databases_IT.Servers_ServerId",
                table: "IT.Databases");

            migrationBuilder.DropTable(
                name: "IT.Servers");

            migrationBuilder.RenameColumn(
                name: "ServerId",
                table: "IT.Databases",
                newName: "HostId");

            migrationBuilder.RenameIndex(
                name: "IX_IT.Databases_ServerId",
                table: "IT.Databases",
                newName: "IX_IT.Databases_HostId");

            migrationBuilder.AddColumn<string>(
                name: "CredentialInfoId",
                table: "IT.Databases",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_IT.Databases_IT.Hosts_HostId",
                table: "IT.Databases",
                column: "HostId",
                principalTable: "IT.Hosts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
