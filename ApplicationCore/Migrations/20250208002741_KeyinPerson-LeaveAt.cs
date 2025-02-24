using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApplicationCore.Migrations
{
    /// <inheritdoc />
    public partial class KeyinPersonLeaveAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Active",
                table: "Keyin.Persons");

            migrationBuilder.AddColumn<DateTime>(
                name: "LeaveAt",
                table: "Keyin.Persons",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LeaveAt",
                table: "Keyin.Persons");

            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "Keyin.Persons",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
