using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RazorBlog.Migrations
{
    [NoCoverage]
    public partial class AddComments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RazorBlog_Comments",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    PostId = table.Column<Guid>(nullable: false),
                    AuthorName = table.Column<string>(maxLength: 128, nullable: false),
                    AuthorEmail = table.Column<string>(maxLength: 128, nullable: false),
                    Body = table.Column<string>(nullable: false),
                    IsApproved = table.Column<bool>(nullable: false),
                    Published = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RazorBlog_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RazorBlog_Comments_RazorBlog_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "RazorBlog_Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RazorBlog_Comments_PostId",
                table: "RazorBlog_Comments",
                column: "PostId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RazorBlog_Comments");
        }
    }
}
