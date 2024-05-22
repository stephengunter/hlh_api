using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApplicationCore.Migrations
{
    /// <inheritdoc />
    public partial class EventremoveTag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tag_Tag_ParentId",
                table: "Tag");

            migrationBuilder.DropForeignKey(
                name: "FK_TagPost_Events_EventId",
                table: "TagPost");

            migrationBuilder.DropForeignKey(
                name: "FK_TagPost_Tag_TagId",
                table: "TagPost");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TagPost",
                table: "TagPost");

            migrationBuilder.DropIndex(
                name: "IX_TagPost_EventId",
                table: "TagPost");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tag",
                table: "Tag");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "TagPost");

            migrationBuilder.RenameTable(
                name: "TagPost",
                newName: "TagPosts");

            migrationBuilder.RenameTable(
                name: "Tag",
                newName: "Tags");

            migrationBuilder.RenameIndex(
                name: "IX_TagPost_TagId",
                table: "TagPosts",
                newName: "IX_TagPosts_TagId");

            migrationBuilder.RenameIndex(
                name: "IX_Tag_ParentId",
                table: "Tags",
                newName: "IX_Tags_ParentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TagPosts",
                table: "TagPosts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tags",
                table: "Tags",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TagPosts_Tags_TagId",
                table: "TagPosts",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Tags_ParentId",
                table: "Tags",
                column: "ParentId",
                principalTable: "Tags",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TagPosts_Tags_TagId",
                table: "TagPosts");

            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Tags_ParentId",
                table: "Tags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tags",
                table: "Tags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TagPosts",
                table: "TagPosts");

            migrationBuilder.RenameTable(
                name: "Tags",
                newName: "Tag");

            migrationBuilder.RenameTable(
                name: "TagPosts",
                newName: "TagPost");

            migrationBuilder.RenameIndex(
                name: "IX_Tags_ParentId",
                table: "Tag",
                newName: "IX_Tag_ParentId");

            migrationBuilder.RenameIndex(
                name: "IX_TagPosts_TagId",
                table: "TagPost",
                newName: "IX_TagPost_TagId");

            migrationBuilder.AddColumn<int>(
                name: "EventId",
                table: "TagPost",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tag",
                table: "Tag",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TagPost",
                table: "TagPost",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_TagPost_EventId",
                table: "TagPost",
                column: "EventId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tag_Tag_ParentId",
                table: "Tag",
                column: "ParentId",
                principalTable: "Tag",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TagPost_Events_EventId",
                table: "TagPost",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TagPost_Tag_TagId",
                table: "TagPost",
                column: "TagId",
                principalTable: "Tag",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
