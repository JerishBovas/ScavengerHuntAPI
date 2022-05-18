using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScavengerHunt_API.Migrations
{
    public partial class UserStructureChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GroupLocation");

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

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateTable(
                name: "GroupLocation",
                columns: table => new
                {
                    GroupsId = table.Column<int>(type: "int", nullable: false),
                    LocationsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupLocation", x => new { x.GroupsId, x.LocationsId });
                    table.ForeignKey(
                        name: "FK_GroupLocation_Groups_GroupsId",
                        column: x => x.GroupsId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupLocation_Locations_LocationsId",
                        column: x => x.LocationsId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GroupLocation_LocationsId",
                table: "GroupLocation",
                column: "LocationsId");
        }
    }
}
