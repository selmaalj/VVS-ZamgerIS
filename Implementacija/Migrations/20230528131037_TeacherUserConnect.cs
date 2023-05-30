using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ooadproject.Migrations
{
    public partial class TeacherUserConnect : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Notification_RecipientID",
                table: "Notification",
                column: "RecipientID");

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_AspNetUsers_RecipientID",
                table: "Notification",
                column: "RecipientID",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notification_AspNetUsers_RecipientID",
                table: "Notification");

            migrationBuilder.DropIndex(
                name: "IX_Notification_RecipientID",
                table: "Notification");
        }
    }
}
