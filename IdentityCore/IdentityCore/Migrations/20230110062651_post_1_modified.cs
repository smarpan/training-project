using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdentityCore.Migrations
{
    public partial class post_1_modified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Images",
                table: "posts",
                newName: "Image");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Image",
                table: "posts",
                newName: "Images");
        }
    }
}
