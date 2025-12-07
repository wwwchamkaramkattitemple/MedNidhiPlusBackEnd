using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedNidhiPlusBackEnd.Migrations
{
    /// <inheritdoc />
    public partial class AddMedicineCategoryTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Medicines");

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Medicines",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MedicineCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicineCategories", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MedicineCategories");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Medicines");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Medicines",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
