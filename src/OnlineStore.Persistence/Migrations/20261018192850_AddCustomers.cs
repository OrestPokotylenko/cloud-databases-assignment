using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineStore.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PublicId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.Sql(
                """
                WITH RankedByEmail AS
                (
                    SELECT
                        [CustomerPublicId],
                        LTRIM(RTRIM([CustomerFullName])) AS [FullName],
                        LOWER(LTRIM(RTRIM([CustomerEmail]))) AS [Email],
                        [OrderDate],
                        ROW_NUMBER() OVER
                        (
                            PARTITION BY LOWER(LTRIM(RTRIM([CustomerEmail])))
                            ORDER BY [OrderDate], [Id]
                        ) AS [EmailRank]
                    FROM [Orders]
                ),
                UniqueEmails AS
                (
                    SELECT
                        [CustomerPublicId],
                        [FullName],
                        [Email],
                        [OrderDate]
                    FROM RankedByEmail
                    WHERE [EmailRank] = 1
                ),
                RankedPublicIds AS
                (
                    SELECT
                        [CustomerPublicId],
                        [FullName],
                        [Email],
                        [OrderDate],
                        ROW_NUMBER() OVER
                        (
                            PARTITION BY [CustomerPublicId]
                            ORDER BY [Email]
                        ) AS [PublicIdRank]
                    FROM UniqueEmails
                )
                INSERT INTO [Customers]
                    ([PublicId], [FirstName], [LastName], [Email], [CreatedAt])
                SELECT
                    CASE
                        WHEN [PublicIdRank] = 1 THEN [CustomerPublicId]
                        ELSE NEWID()
                    END,
                    LEFT(
                        CASE
                            WHEN CHARINDEX(' ', [FullName]) > 0
                                THEN LEFT([FullName], CHARINDEX(' ', [FullName]) - 1)
                            ELSE [FullName]
                        END,
                        100),
                    LEFT(
                        CASE
                            WHEN CHARINDEX(' ', [FullName]) > 0
                                THEN LTRIM(SUBSTRING([FullName], CHARINDEX(' ', [FullName]) + 1, 200))
                            ELSE '-'
                        END,
                        100),
                    [Email],
                    [OrderDate]
                FROM RankedPublicIds;

                UPDATE orders
                SET orders.[CustomerId] = customers.[Id]
                FROM [Orders] orders
                INNER JOIN [Customers] customers
                    ON customers.[Email] = LOWER(LTRIM(RTRIM(orders.[CustomerEmail])));
                """);

            migrationBuilder.AlterColumn<int>(
                name: "CustomerId",
                table: "Orders",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.DropColumn(
                name: "CustomerEmail",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CustomerFullName",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CustomerPhoneNumber",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CustomerPublicId",
                table: "Orders");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerId",
                table: "Orders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Email",
                table: "Customers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_PublicId",
                table: "Customers",
                column: "PublicId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Customers_CustomerId",
                table: "Orders",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Customers_CustomerId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_CustomerId",
                table: "Orders");

            migrationBuilder.AddColumn<string>(
                name: "CustomerEmail",
                table: "Orders",
                type: "nvarchar(320)",
                maxLength: 320,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerFullName",
                table: "Orders",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerPhoneNumber",
                table: "Orders",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CustomerPublicId",
                table: "Orders",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.Sql(
                """
                UPDATE orders
                SET
                    orders.[CustomerPublicId] = customers.[PublicId],
                    orders.[CustomerFullName] = customers.[FirstName] + ' ' + customers.[LastName],
                    orders.[CustomerEmail] = customers.[Email]
                FROM [Orders] orders
                INNER JOIN [Customers] customers
                    ON customers.[Id] = orders.[CustomerId];
                """);

            migrationBuilder.AlterColumn<string>(
                name: "CustomerEmail",
                table: "Orders",
                type: "nvarchar(320)",
                maxLength: 320,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(320)",
                oldMaxLength: 320,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CustomerFullName",
                table: "Orders",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CustomerPublicId",
                table: "Orders",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "Orders");

            migrationBuilder.DropTable(
                name: "Customers");
        }
    }
}
