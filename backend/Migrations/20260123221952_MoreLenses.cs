using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class MoreLenses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Lenses",
                columns: new[] { "Id", "Aperture", "Brand", "Compatibility", "Description", "FocalLength", "IsPopular", "MaxFocal", "MinFocal", "Name", "Price", "Type" },
                values: new object[,]
                {
                    { 6, "f/1.4", "Canon", "Canon EF", "Стандартный объектив с большой диафрагмой для портретов.", "50mm", null, 50, 50, "Canon EF 50mm f/1.4", 25000m, "portrait" },
                    { 7, "f/1.8", "Nikon", "Nikon F", "Доступный портретный объектив с отличной оптикой.", "50mm", null, 50, 50, "Nikon AF-S 50mm f/1.8G", 15000m, "portrait" },
                    { 8, "f/1.8", "Sony", "Sony FE", "Широкоугольный портретный объектив для полнокадровых камер.", "35mm", null, 35, 35, "Sony FE 35mm f/1.8", 40000m, "portrait" },
                    { 9, "f/2.8", "Tamron", "Sony FE", "Легкий телезум для спорта и портретов.", "70-180mm", null, 180, 70, "Tamron 70-180mm f/2.8", 95000m, "sport" },
                    { 10, "f/2.8", "Sigma", "Sony FE / Leica L", "Высококачественный зум для пейзажной съемки.", "24-70mm", null, 70, 24, "Sigma 24-70mm f/2.8 DG DN", 85000m, "landscape" },
                    { 11, "f/3.5-5.6", "Canon", "Canon EF-S", "Бюджетный кит-объектив для начинающих.", "18-55mm", null, 55, 18, "Canon EF-S 18-55mm f/3.5-5.6", 12000m, "landscape" },
                    { 12, "f/3.5-5.6", "Nikon", "Nikon F", "Компактный зум для повседневной съемки.", "18-55mm", null, 55, 18, "Nikon AF-P 18-55mm f/3.5-5.6G", 10000m, "landscape" },
                    { 13, "f/2.8", "Sony", "Sony E", "Профессиональный зум с постоянной диафрагмой.", "16-55mm", null, 55, 16, "Sony E 16-55mm f/2.8 G", 120000m, "landscape" },
                    { 14, "f/5-6.3", "Sigma", "Sony FE / Leica L", "Длиннофокусный зум для дикой природы.", "100-400mm", null, 400, 100, "Sigma 100-400mm f/5-6.3 DG DN OS", 70000m, "sport" },
                    { 15, "f/2.8-4", "Tamron", "Canon EF / Nikon F", "Универсальный зум с хорошей оптикой.", "17-70mm", null, 70, 17, "Tamron 17-70mm f/2.8-4", 50000m, "landscape" },
                    { 16, "f/4.5-5.6", "Canon", "Canon EF", "L-серия для профессиональной спортивной съемки.", "100-400mm", null, 400, 100, "Canon EF 100-400mm f/4.5-5.6L IS II", 180000m, "sport" },
                    { 17, "f/4.5-5.6", "Nikon", "Nikon F", "Мощный зум с оптической стабилизацией.", "80-400mm", null, 400, 80, "Nikon AF-S 80-400mm f/4.5-5.6G ED VR", 160000m, "sport" },
                    { 18, "f/4.5-5.6", "Sony", "Sony FE", "G Master зум для профессионалов.", "100-400mm", null, 400, 100, "Sony FE 100-400mm f/4.5-5.6 GM OSS", 220000m, "sport" },
                    { 19, "f/5-6.3", "Sigma", "Canon EF / Nikon F / Sony FE", "Супер-телезум для птиц и спорта.", "150-600mm", null, 600, 150, "Sigma 150-600mm f/5-6.3 DG OS HSM", 90000m, "sport" },
                    { 20, "f/5-6.3", "Tamron", "Canon EF / Nikon F", "Вторая версия с улучшенной оптикой.", "150-600mm", null, 600, 150, "Tamron 150-600mm f/5-6.3 Di VC USD G2", 100000m, "sport" },
                    { 21, "f/4", "Canon", "Canon EF", "Фишай объектив для креативной съемки.", "8-15mm", null, 15, 8, "Canon EF 8-15mm f/4L Fisheye USM", 120000m, "landscape" },
                    { 22, "f/4", "Nikon", "Nikon F", "Широкоугольный зум с стабилизацией.", "16-35mm", null, 35, 16, "Nikon AF-S 16-35mm f/4G ED VR", 110000m, "landscape" },
                    { 23, "f/4", "Sony", "Sony FE", "Сверхширокоугольный зум для архитектуры.", "12-24mm", null, 24, 12, "Sony FE 12-24mm f/4 G", 160000m, "landscape" },
                    { 24, "f/2.8", "Sigma", "Canon EF / Nikon F / Sony FE", "Высококачественный широкоугольник.", "14-24mm", null, 24, 14, "Sigma 14-24mm f/2.8 DG HSM", 130000m, "landscape" },
                    { 25, "f/3.5-4.5", "Tamron", "Canon EF / Nikon F", "Доступный сверхширокоугольный зум.", "10-24mm", null, 24, 10, "Tamron 10-24mm f/3.5-4.5", 40000m, "landscape" },
                    { 26, "f/2.8", "Canon", "Canon EF", "Макро-объектив с оптической стабилизацией.", "100mm", null, 100, 100, "Canon EF 100mm f/2.8L Macro IS USM", 80000m, "macro" },
                    { 27, "f/2.8", "Nikon", "Nikon F", "Классический макро-объектив Nikon.", "105mm", null, 105, 105, "Nikon AF-S 105mm f/2.8G ED-IF VR", 90000m, "macro" },
                    { 28, "f/2.8", "Sony", "Sony FE", "G-объектив для макросъемки.", "90mm", null, 90, 90, "Sony FE 90mm f/2.8 Macro G OSS", 100000m, "macro" },
                    { 29, "f/2.8", "Sigma", "Canon EF / Nikon F / Sony FE", "Доступный макро-объектив с хорошей оптикой.", "70mm", null, 70, 70, "Sigma 70mm f/2.8 DG Macro", 50000m, "macro" },
                    { 30, "f/2.8", "Tamron", "Canon EF / Nikon F", "Макро с вибро-подавлением.", "90mm", null, 90, 90, "Tamron 90mm f/2.8 Di VC USD Macro", 55000m, "macro" },
                    { 31, "f/2.8", "Canon", "Canon EF", "Супер-телефото для профессионалов.", "400mm", null, 400, 400, "Canon EF 400mm f/2.8L IS III USM", 1200000m, "sport" },
                    { 32, "f/2.8", "Nikon", "Nikon F", "Флагманский телеобъектив Nikon.", "400mm", null, 400, 400, "Nikon AF-S 400mm f/2.8E FL ED VR", 1300000m, "sport" },
                    { 33, "f/2.8", "Sony", "Sony FE", "G Master 400мм для спорта и дикой природы.", "400mm", null, 400, 400, "Sony FE 400mm f/2.8 GM OSS", 1400000m, "sport" },
                    { 34, "f/1.4", "Sigma", "Canon EF / Nikon F / Sony FE", "Арт-объектив с большой диафрагмой.", "50mm", null, 50, 50, "Sigma 50mm f/1.4 DG HSM", 60000m, "portrait" },
                    { 35, "f/1.8", "Tamron", "Canon EF / Nikon F", "Широкоугольный объектив с стабилизацией.", "35mm", null, 35, 35, "Tamron 35mm f/1.8 VC", 30000m, "portrait" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Lenses",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Lenses",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Lenses",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Lenses",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Lenses",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Lenses",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Lenses",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Lenses",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Lenses",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Lenses",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Lenses",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Lenses",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Lenses",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Lenses",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Lenses",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Lenses",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Lenses",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Lenses",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Lenses",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Lenses",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Lenses",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Lenses",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Lenses",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Lenses",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Lenses",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "Lenses",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "Lenses",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "Lenses",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "Lenses",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "Lenses",
                keyColumn: "Id",
                keyValue: 35);
        }
    }
}
