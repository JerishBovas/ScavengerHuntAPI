using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScavengerHunt.API.Migrations
{
    public partial class UPdatedGroup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "uniqueId",
                table: "Groups",
                newName: "UniqueId");

            migrationBuilder.RenameColumn(
                name: "longitude",
                table: "Coordinates",
                newName: "Longitude");

            migrationBuilder.RenameColumn(
                name: "latitude",
                table: "Coordinates",
                newName: "Latitude");

            migrationBuilder.AlterColumn<string>(
                name: "Ratings",
                table: "Locations",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UniqueId",
                table: "Groups",
                newName: "uniqueId");

            migrationBuilder.RenameColumn(
                name: "Longitude",
                table: "Coordinates",
                newName: "longitude");

            migrationBuilder.RenameColumn(
                name: "Latitude",
                table: "Coordinates",
                newName: "latitude");

            migrationBuilder.AlterColumn<int>(
                name: "Ratings",
                table: "Locations",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
