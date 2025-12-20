using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedNidhiPlusBackEnd.Migrations
{
    /// <inheritdoc />
    public partial class Lockedstatusadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsLocked",
                table: "Invoices",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsLocked",
                table: "Invoices");
        }
    }
}
