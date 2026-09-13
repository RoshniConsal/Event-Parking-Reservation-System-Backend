using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventParking.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddSeatLayoutType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SeatLayoutType",
                table: "Events",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Theatre");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SeatLayoutType",
                table: "Events");
        }
    }
}
