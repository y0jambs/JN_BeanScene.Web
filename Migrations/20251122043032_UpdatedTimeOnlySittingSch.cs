using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeanScene.Web.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedTimeOnlySittingSch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<TimeOnly>(
                name: "StartDateTime",
                table: "SittingSchedule",
                type: "time(0)",
                precision: 0,
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2(0)",
                oldPrecision: 0);

            migrationBuilder.AlterColumn<TimeOnly>(
                name: "EndDateTime",
                table: "SittingSchedule",
                type: "time(0)",
                precision: 0,
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2(0)",
                oldPrecision: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDateTime",
                table: "SittingSchedule",
                type: "datetime2(0)",
                precision: 0,
                nullable: false,
                oldClrType: typeof(TimeOnly),
                oldType: "time(0)",
                oldPrecision: 0);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDateTime",
                table: "SittingSchedule",
                type: "datetime2(0)",
                precision: 0,
                nullable: false,
                oldClrType: typeof(TimeOnly),
                oldType: "time(0)",
                oldPrecision: 0);
        }
    }
}
