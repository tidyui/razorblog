using Microsoft.EntityFrameworkCore.Migrations;

namespace RazorBlog.Migrations
{
    [NoCoverage]
    public partial class AddAuthor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AuthorEmail",
                table: "RazorBlog_Posts",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AuthorName",
                table: "RazorBlog_Posts",
                maxLength: 128,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthorEmail",
                table: "RazorBlog_Posts");

            migrationBuilder.DropColumn(
                name: "AuthorName",
                table: "RazorBlog_Posts");
        }
    }
}
