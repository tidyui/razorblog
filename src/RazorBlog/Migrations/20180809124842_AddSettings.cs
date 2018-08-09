using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RazorBlog.Migrations
{
    [NoCoverage]
    public partial class AddSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RazorBlog_Settings",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Key = table.Column<string>(maxLength: 64, nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RazorBlog_Settings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RazorBlog_Settings_Key",
                table: "RazorBlog_Settings",
                column: "Key",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RazorBlog_Settings");
        }
    }
}
