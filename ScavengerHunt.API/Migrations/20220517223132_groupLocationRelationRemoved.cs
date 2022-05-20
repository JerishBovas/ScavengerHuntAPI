using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScavengerHunt.API.Migrations
{
    public partial class groupLocationRelationRemoved : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Locations_Groups_GroupId",
                table: "Locations");

            migrationBuilder.DropIndex(
                name: "IX_Locations_GroupId",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "Locations");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GroupId",
                table: "Locations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Locations_GroupId",
                table: "Locations",
                column: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Locations_Groups_GroupId",
                table: "Locations",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
