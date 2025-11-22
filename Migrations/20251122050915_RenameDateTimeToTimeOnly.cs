using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeanScene.Web.Migrations
{
    /// <inheritdoc />
    public partial class RenameDateTimeToTimeOnly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StartDateTime",
                table: "SittingSchedule",
                newName: "StartTime");

            migrationBuilder.RenameColumn(
                name: "EndDateTime",
                table: "SittingSchedule",
                newName: "EndTime");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StartTime",
                table: "SittingSchedule",
                newName: "StartDateTime");

            migrationBuilder.RenameColumn(
                name: "EndTime",
                table: "SittingSchedule",
                newName: "EndDateTime");
        }
    }
}
