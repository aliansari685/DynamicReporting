using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DynamicReporting.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDafaultToReportDefinitionTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AddColumn<bool>(
            //    name: "IsDefault",
            //    table: "ReportDefinitions",
            //    type: "bit",
            //    nullable: false,
            //    defaultValue: false);

            //migrationBuilder.UpdateData(
            //    table: nameof(ShopTestDbContext.ReportDefinitions),
            //    nameof(ReportDefinition.Id),
            //    1,
            //    nameof(ReportDefinition.IsDefault),
            //    value: true
            //);

            migrationBuilder.CreateIndex(
                name: "UX_ReportDefinitions_IsDefault",
                table: "ReportDefinitions",
                column: "IsDefault",
                unique: true,
                filter: "[IsDefault] = 1"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "ReportDefinitions");
            migrationBuilder.DropIndex(
                name: "UX_ReportDefinitions_IsDefault",
                table: "ReportDefinitions"
            );
        }
    }
}
