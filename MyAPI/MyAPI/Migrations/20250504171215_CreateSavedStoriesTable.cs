using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyAPI.Migrations
{
    /// <inheritdoc />
    public partial class CreateSavedStoriesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SavedStories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StoryId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SavedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedStories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SavedStories_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SavedStories_Stories_StoryId",
                        column: x => x.StoryId,
                        principalTable: "Stories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SavedStories_StoryId",
                table: "SavedStories",
                column: "StoryId");

            migrationBuilder.CreateIndex(
                name: "IX_SavedStories_UserId",
                table: "SavedStories",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SavedStories");
        }
    }
}
