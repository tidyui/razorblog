using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RazorBlog.Migrations
{
    [NoCoverage]
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RazorBlog_Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Title = table.Column<string>(maxLength: 64, nullable: false),
                    Slug = table.Column<string>(maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RazorBlog_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RazorBlog_Posts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CategoryId = table.Column<Guid>(nullable: true),
                    Title = table.Column<string>(maxLength: 128, nullable: false),
                    Slug = table.Column<string>(maxLength: 128, nullable: false),
                    MetaKeywords = table.Column<string>(maxLength: 128, nullable: true),
                    MetaDescription = table.Column<string>(maxLength: 256, nullable: true),
                    Excerpt = table.Column<string>(nullable: true),
                    Body = table.Column<string>(nullable: true),
                    Published = table.Column<DateTime>(nullable: true),
                    LastModified = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RazorBlog_Posts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RazorBlog_Posts_RazorBlog_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "RazorBlog_Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RazorBlog_Tags",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    PostId = table.Column<Guid>(nullable: false),
                    Title = table.Column<string>(maxLength: 64, nullable: false),
                    Slug = table.Column<string>(maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RazorBlog_Tags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RazorBlog_Tags_RazorBlog_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "RazorBlog_Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RazorBlog_Categories_Slug",
                table: "RazorBlog_Categories",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RazorBlog_Posts_CategoryId",
                table: "RazorBlog_Posts",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_RazorBlog_Posts_Slug",
                table: "RazorBlog_Posts",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RazorBlog_Tags_PostId_Slug",
                table: "RazorBlog_Tags",
                columns: new[] { "PostId", "Slug" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RazorBlog_Tags");

            migrationBuilder.DropTable(
                name: "RazorBlog_Posts");

            migrationBuilder.DropTable(
                name: "RazorBlog_Categories");
        }
    }
}
