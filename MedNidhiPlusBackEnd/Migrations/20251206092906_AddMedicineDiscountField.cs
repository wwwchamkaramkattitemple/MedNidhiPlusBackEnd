using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedNidhiPlusBackEnd.Migrations
{
    /// <inheritdoc />
    public partial class AddMedicineDiscountField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Medicines_MedicineCategories_MedicineCategoryId",
                table: "Medicines");

            migrationBuilder.DropIndex(
                name: "IX_Medicines_MedicineCategoryId",
                table: "Medicines");

            migrationBuilder.DropColumn(
                name: "MedicineCategoryId",
                table: "Medicines");

            migrationBuilder.AddColumn<decimal>(
                name: "Discount",
                table: "Medicines",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_Medicines_CategoryId",
                table: "Medicines",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Medicines_MedicineCategories_CategoryId",
                table: "Medicines",
                column: "CategoryId",
                principalTable: "MedicineCategories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Medicines_MedicineCategories_CategoryId",
                table: "Medicines");

            migrationBuilder.DropIndex(
                name: "IX_Medicines_CategoryId",
                table: "Medicines");

            migrationBuilder.DropColumn(
                name: "Discount",
                table: "Medicines");

            migrationBuilder.AddColumn<int>(
                name: "MedicineCategoryId",
                table: "Medicines",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Medicines_MedicineCategoryId",
                table: "Medicines",
                column: "MedicineCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Medicines_MedicineCategories_MedicineCategoryId",
                table: "Medicines",
                column: "MedicineCategoryId",
                principalTable: "MedicineCategories",
                principalColumn: "Id");
        }
    }
}
