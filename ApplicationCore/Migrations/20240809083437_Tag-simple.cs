using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApplicationCore.Migrations
{
    /// <inheritdoc />
    public partial class Tagsimple : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Tags_ParentId",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_ParentId",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "Removed",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "SubIds",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "PostId",
                table: "TagPosts");

            migrationBuilder.RenameColumn(
                name: "PostType",
                table: "TagPosts",
                newName: "EntityType");

            migrationBuilder.AddColumn<string>(
                name: "EntityId",
                table: "TagPosts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EntityId",
                table: "TagPosts");

            migrationBuilder.RenameColumn(
                name: "EntityType",
                table: "TagPosts",
                newName: "PostType");

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "Tags",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ParentId",
                table: "Tags",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Removed",
                table: "Tags",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SubIds",
                table: "Tags",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PostId",
                table: "TagPosts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_ParentId",
                table: "Tags",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Tags_ParentId",
                table: "Tags",
                column: "ParentId",
                principalTable: "Tags",
                principalColumn: "Id");
        }
    }
}
