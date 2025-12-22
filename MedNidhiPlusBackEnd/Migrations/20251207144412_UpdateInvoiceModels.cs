using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedNidhiPlusBackEnd.Migrations
{
    /// <inheritdoc />
    public partial class UpdateInvoiceModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ServiceCode",
                table: "InvoiceItems");

            migrationBuilder.AlterColumn<string>(
                name: "InvoiceNumber",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "InvoiceNumber",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ServiceCode",
                table: "InvoiceItems",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}
