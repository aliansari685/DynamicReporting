#nullable disable

namespace DynamicReporting.Api.Migrations
{
    /// <inheritdoc />
    public partial class ReportDefinitionsTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReportDefinitions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    BaseTable = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SelectedColumns = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportDefinitions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ProductId",
                table: "OrderItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerId",
                table: "Orders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_SupplierId",
                table: "Products",
                column: "SupplierId");


            // --- Seed Default Data ---
            var defaultSelectedColumns = new List<SelectedColumn>
            {
                new() { Table = nameof(Order), Column = nameof(Order.OrderId) },
                new() { Table = nameof(Order), Column = nameof(Order.OrderDate) },
                new() { Table = nameof(Order), Column = nameof(Order.TotalAmount) },
                new() { Table = nameof(Order), Column = nameof(Order.Status) },
                new() { Table = nameof(Customer), Column = nameof(Customer.FullName) }
            };

            var report = new ReportDefinition()
            {
                Name = "Default",
                BaseTable = nameof(Order),
                CreatedBy = "Admin",
                SelectedColumns = defaultSelectedColumns,
            };

            // --- Insert Seed Data ---
            var json = System.Text.Json.JsonSerializer.Serialize(defaultSelectedColumns);

            migrationBuilder.InsertData(
                table: nameof(ShopTestDbContext.ReportDefinitions),
                columns:
                [
                    nameof(ReportDefinition.Name),
                    nameof(ReportDefinition.BaseTable),
                    nameof(ReportDefinition.SelectedColumns),
                    nameof(ReportDefinition.CreatedBy),
                    nameof(ReportDefinition.CreatedAt),

                ],
                values:
                [
                    "Default",
                    nameof(Order),
                    json,
                    "Admin",
                    DateTime.UtcNow
                ]
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "ReportDefinitions");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Suppliers");
        }
    }
}
