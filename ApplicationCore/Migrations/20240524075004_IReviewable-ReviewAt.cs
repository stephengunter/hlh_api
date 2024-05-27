using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApplicationCore.Migrations
{
    /// <inheritdoc />
    public partial class IReviewableReviewAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryPost_Categories_CategoryId",
                table: "CategoryPost");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CategoryPost",
                table: "CategoryPost");

            migrationBuilder.RenameTable(
                name: "CategoryPost",
                newName: "CategoryPosts");

            migrationBuilder.RenameIndex(
                name: "IX_CategoryPost_CategoryId",
                table: "CategoryPosts",
                newName: "IX_CategoryPosts_CategoryId");

            migrationBuilder.AddColumn<DateTime>(
                name: "ReviewedAt",
                table: "Files.Judgebooks",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CategoryPosts",
                table: "CategoryPosts",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryPosts_Categories_CategoryId",
                table: "CategoryPosts",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryPosts_Categories_CategoryId",
                table: "CategoryPosts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CategoryPosts",
                table: "CategoryPosts");

            migrationBuilder.DropColumn(
                name: "ReviewedAt",
                table: "Files.Judgebooks");

            migrationBuilder.RenameTable(
                name: "CategoryPosts",
                newName: "CategoryPost");

            migrationBuilder.RenameIndex(
                name: "IX_CategoryPosts_CategoryId",
                table: "CategoryPost",
                newName: "IX_CategoryPost_CategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CategoryPost",
                table: "CategoryPost",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryPost_Categories_CategoryId",
                table: "CategoryPost",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
