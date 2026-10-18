using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineStore.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RenameOrderDatesForExplicitActions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Orders_ShippedAt",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "ShippedAt",
                table: "Orders",
                newName: "ShippingDate");

            migrationBuilder.RenameColumn(
                name: "OrderedAt",
                table: "Orders",
                newName: "OrderDate");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_OrderedAt_Id",
                table: "Orders",
                newName: "IX_Orders_OrderDate_Id");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Orders_ShippingDate",
                table: "Orders",
                sql: "[ShippingDate] IS NULL OR [ShippingDate] >= [OrderDate]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Orders_ShippingDate",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "ShippingDate",
                table: "Orders",
                newName: "ShippedAt");

            migrationBuilder.RenameColumn(
                name: "OrderDate",
                table: "Orders",
                newName: "OrderedAt");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_OrderDate_Id",
                table: "Orders",
                newName: "IX_Orders_OrderedAt_Id");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Orders_ShippedAt",
                table: "Orders",
                sql: "[ShippedAt] IS NULL OR [ShippedAt] >= [OrderedAt]");
        }
    }
}
