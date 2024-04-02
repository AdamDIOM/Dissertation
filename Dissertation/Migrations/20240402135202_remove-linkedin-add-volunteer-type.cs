using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dissertation.Migrations
{
    /// <inheritdoc />
    public partial class removelinkedinaddvolunteertype : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LinkedInEnding",
                table: "Volunteer");

            migrationBuilder.RenameColumn(
                name: "Level",
                table: "Volunteer",
                newName: "Type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Volunteer",
                newName: "Level");

            migrationBuilder.AddColumn<string>(
                name: "LinkedInEnding",
                table: "Volunteer",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
