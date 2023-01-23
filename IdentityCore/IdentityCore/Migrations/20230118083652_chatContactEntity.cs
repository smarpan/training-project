using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdentityCore.Migrations
{
    public partial class chatContactEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "chatContacts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ContactUserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chatContacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_chatContacts_AspNetUsers_ContactUserId",
                        column: x => x.ContactUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_chatContacts_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_chatContacts_ContactUserId",
                table: "chatContacts",
                column: "ContactUserId");

            migrationBuilder.CreateIndex(
                name: "IX_chatContacts_UserId",
                table: "chatContacts",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "chatContacts");
        }
    }
}
