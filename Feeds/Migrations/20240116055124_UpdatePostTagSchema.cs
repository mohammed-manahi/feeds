using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Feeds.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePostTagSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostTags_Posts_PostTagId",
                table: "PostTags");

            migrationBuilder.DropForeignKey(
                name: "FK_PostTags_Tags_PostTagId",
                table: "PostTags");

            migrationBuilder.DropIndex(
                name: "IX_PostTags_PostTagId",
                table: "PostTags");

            migrationBuilder.DropColumn(
                name: "PostTagId",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "PostTagId",
                table: "PostTags");

            migrationBuilder.DropColumn(
                name: "PostTagId",
                table: "Posts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PostTagId",
                table: "Tags",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PostTagId",
                table: "PostTags",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PostTagId",
                table: "Posts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PostTags_PostTagId",
                table: "PostTags",
                column: "PostTagId");

            migrationBuilder.AddForeignKey(
                name: "FK_PostTags_Posts_PostTagId",
                table: "PostTags",
                column: "PostTagId",
                principalTable: "Posts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PostTags_Tags_PostTagId",
                table: "PostTags",
                column: "PostTagId",
                principalTable: "Tags",
                principalColumn: "Id");
        }
    }
}
