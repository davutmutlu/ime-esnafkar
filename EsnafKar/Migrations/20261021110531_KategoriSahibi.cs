using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EsnafKar.Migrations
{
    /// <inheritdoc />
    public partial class KategoriSahibi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "KullaniciId",
                table: "GiderKategorileri",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "GiderKategorileri",
                keyColumn: "Id",
                keyValue: 1,
                column: "KullaniciId",
                value: null);

            migrationBuilder.UpdateData(
                table: "GiderKategorileri",
                keyColumn: "Id",
                keyValue: 2,
                column: "KullaniciId",
                value: null);

            migrationBuilder.UpdateData(
                table: "GiderKategorileri",
                keyColumn: "Id",
                keyValue: 3,
                column: "KullaniciId",
                value: null);

            migrationBuilder.UpdateData(
                table: "GiderKategorileri",
                keyColumn: "Id",
                keyValue: 4,
                column: "KullaniciId",
                value: null);

            migrationBuilder.UpdateData(
                table: "GiderKategorileri",
                keyColumn: "Id",
                keyValue: 5,
                column: "KullaniciId",
                value: null);

            migrationBuilder.UpdateData(
                table: "GiderKategorileri",
                keyColumn: "Id",
                keyValue: 6,
                column: "KullaniciId",
                value: null);

            migrationBuilder.UpdateData(
                table: "GiderKategorileri",
                keyColumn: "Id",
                keyValue: 7,
                column: "KullaniciId",
                value: null);

            migrationBuilder.UpdateData(
                table: "GiderKategorileri",
                keyColumn: "Id",
                keyValue: 8,
                column: "KullaniciId",
                value: null);

            migrationBuilder.UpdateData(
                table: "GiderKategorileri",
                keyColumn: "Id",
                keyValue: 9,
                column: "KullaniciId",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KullaniciId",
                table: "GiderKategorileri");
        }
    }
}
