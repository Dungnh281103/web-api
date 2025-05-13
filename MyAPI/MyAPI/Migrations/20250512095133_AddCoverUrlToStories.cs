using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddCoverUrlToStories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CoverUrl",
                table: "ReadingHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CoverUrl",
                table: "ReadingHistories");
        }
    }
}
