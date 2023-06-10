using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ooadproject.Migrations
{
    public partial class Requester : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Request_Student_RequesterID",
                table: "Request");

            migrationBuilder.AlterColumn<int>(
                name: "RequesterID",
                table: "Request",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Request_Student_RequesterID",
                table: "Request",
                column: "RequesterID",
                principalTable: "Student",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Request_Student_RequesterID",
                table: "Request");

            migrationBuilder.AlterColumn<int>(
                name: "RequesterID",
                table: "Request",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Request_Student_RequesterID",
                table: "Request",
                column: "RequesterID",
                principalTable: "Student",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
