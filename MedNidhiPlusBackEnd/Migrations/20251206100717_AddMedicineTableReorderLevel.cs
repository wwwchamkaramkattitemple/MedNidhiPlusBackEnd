using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedNidhiPlusBackEnd.Migrations
{
    /// <inheritdoc />
    public partial class AddMedicineTableReorderLevel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReorderLevel",
                table: "Medicines",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReorderLevel",
                table: "Medicines");
        }
    }
}
