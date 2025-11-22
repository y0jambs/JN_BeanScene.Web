using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeanScene.Web.Migrations
{
    /// <inheritdoc />
    public partial class RemoveIsClosedFromSittingSchedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsClosed",
                table: "SittingSchedule");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsClosed",
                table: "SittingSchedule",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
