using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Lenses",
                columns: new[] { "Id", "Aperture", "Brand", "Compatibility", "Description", "FocalLength", "IsPopular", "MaxFocal", "MinFocal", "Name", "Price", "Type" },
                values: new object[,]
                {
                    { 1, "f/1.8", "Canon", "Canon EF", "Классический портретный объектив.", "85mm", true, 85, 85, "Canon EF 85mm f/1.8", 35000m, "portrait" },
                    { 2, "f/2.8", "Canon", "Canon EF", "Универсальный зум для пейзажей и репортажей.", "24–70mm", true, 70, 24, "Canon EF 24-70mm f/2.8", 90000m, "landscape" },
                    { 3, "f/2.8", "Nikon", "Nikon F", "Телезум для спорта и съёмки с расстояния.", "70–200mm", true, 200, 70, "Nikon AF-S 70-200mm f/2.8", 130000m, "sport" },
                    { 4, "f/2.8", "Sony", "Sony FE", "Макрообъектив для съёмки мелких деталей.", "90mm", null, 90, 90, "Sony FE 90mm f/2.8 Macro", 110000m, "macro" },
                    { 5, "f/1.4", "Sigma", "Sony E / m4/3", "Широкоугольный объектив для пейзажей и интерьеров.", "16mm", null, 16, 16, "Sigma 16mm f/1.4 DC DN", 45000m, "landscape" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Lenses",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Lenses",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Lenses",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Lenses",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Lenses",
                keyColumn: "Id",
                keyValue: 5);
        }
    }
}
