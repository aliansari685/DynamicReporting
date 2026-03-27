using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DynamicReporting.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddValueComparerForSelectedColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DownloadUrl",
                table: "ReportGenerations",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "JobId",
                table: "ReportGenerations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_JobId",
                table: "ReportGenerations",
                column: "JobId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_JobId",
                table: "ReportGenerations");

            migrationBuilder.DropColumn(
                name: "JobId",
                table: "ReportGenerations");

            migrationBuilder.AlterColumn<string>(
                name: "DownloadUrl",
                table: "ReportGenerations",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2048)",
                oldMaxLength: 2048);
        }
    }
}
