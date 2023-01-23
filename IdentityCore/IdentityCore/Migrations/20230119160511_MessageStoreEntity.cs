using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdentityCore.Migrations
{
    public partial class MessageStoreEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tempMessages_AspNetUsers_RecieverId",
                table: "tempMessages");

            migrationBuilder.RenameColumn(
                name: "RecieverId",
                table: "tempMessages",
                newName: "ReceiverId");

            migrationBuilder.RenameIndex(
                name: "IX_tempMessages_RecieverId",
                table: "tempMessages",
                newName: "IX_tempMessages_ReceiverId");

            migrationBuilder.CreateTable(
                name: "messageStore",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BelongsTo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SenderId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ReceiverId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_messageStore", x => x.Id);
                    table.ForeignKey(
                        name: "FK_messageStore_AspNetUsers_BelongsTo",
                        column: x => x.BelongsTo,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_messageStore_AspNetUsers_ReceiverId",
                        column: x => x.ReceiverId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_messageStore_AspNetUsers_SenderId",
                        column: x => x.SenderId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_messageStore_BelongsTo",
                table: "messageStore",
                column: "BelongsTo");

            migrationBuilder.CreateIndex(
                name: "IX_messageStore_ReceiverId",
                table: "messageStore",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_messageStore_SenderId",
                table: "messageStore",
                column: "SenderId");

            migrationBuilder.AddForeignKey(
                name: "FK_tempMessages_AspNetUsers_ReceiverId",
                table: "tempMessages",
                column: "ReceiverId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tempMessages_AspNetUsers_ReceiverId",
                table: "tempMessages");

            migrationBuilder.DropTable(
                name: "messageStore");

            migrationBuilder.RenameColumn(
                name: "ReceiverId",
                table: "tempMessages",
                newName: "RecieverId");

            migrationBuilder.RenameIndex(
                name: "IX_tempMessages_ReceiverId",
                table: "tempMessages",
                newName: "IX_tempMessages_RecieverId");

            migrationBuilder.AddForeignKey(
                name: "FK_tempMessages_AspNetUsers_RecieverId",
                table: "tempMessages",
                column: "RecieverId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
