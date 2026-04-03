using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DynamicReporting.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddFileTypeToReportGenrationTBL : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileType",
                table: "ReportGenerations",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileType",
                table: "ReportGenerations");
        }
    }
}
