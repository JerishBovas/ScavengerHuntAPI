using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScavengerHunt.Migrations
{
    public partial class UserLogUpdateName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScoreLogs_UserSections_UserLogId",
                table: "ScoreLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSections_Users_UserId",
                table: "UserSections");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserSections",
                table: "UserSections");

            migrationBuilder.RenameTable(
                name: "UserSections",
                newName: "UserLogs");

            migrationBuilder.RenameIndex(
                name: "IX_UserSections_UserId",
                table: "UserLogs",
                newName: "IX_UserLogs_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserLogs",
                table: "UserLogs",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ScoreLogs_UserLogs_UserLogId",
                table: "ScoreLogs",
                column: "UserLogId",
                principalTable: "UserLogs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserLogs_Users_UserId",
                table: "UserLogs",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScoreLogs_UserLogs_UserLogId",
                table: "ScoreLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLogs_Users_UserId",
                table: "UserLogs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserLogs",
                table: "UserLogs");

            migrationBuilder.RenameTable(
                name: "UserLogs",
                newName: "UserSections");

            migrationBuilder.RenameIndex(
                name: "IX_UserLogs_UserId",
                table: "UserSections",
                newName: "IX_UserSections_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserSections",
                table: "UserSections",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ScoreLogs_UserSections_UserLogId",
                table: "ScoreLogs",
                column: "UserLogId",
                principalTable: "UserSections",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserSections_Users_UserId",
                table: "UserSections",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
