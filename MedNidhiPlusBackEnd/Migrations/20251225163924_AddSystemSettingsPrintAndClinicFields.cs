using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedNidhiPlusBackEnd.Migrations
{
    /// <inheritdoc />
    public partial class AddSystemSettingsPrintAndClinicFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClinicAddress",
                table: "SystemSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ClinicEmail",
                table: "SystemSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ClinicGstNumber",
                table: "SystemSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ClinicPhone",
                table: "SystemSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DefaultInvoiceDesign",
                table: "SystemSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DefaultInvoicePrintMode",
                table: "SystemSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PdfFooterMessage",
                table: "SystemSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PdfHeaderMessage",
                table: "SystemSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PdfThankYouMessage",
                table: "SystemSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_SystemSettings_Id",
                table: "SystemSettings",
                column: "Id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SystemSettings_Id",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "ClinicAddress",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "ClinicEmail",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "ClinicGstNumber",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "ClinicPhone",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "DefaultInvoiceDesign",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "DefaultInvoicePrintMode",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "PdfFooterMessage",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "PdfHeaderMessage",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "PdfThankYouMessage",
                table: "SystemSettings");
        }
    }
}
