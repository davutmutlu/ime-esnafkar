using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EsnafKar.Migrations
{
    /// <inheritdoc />
    public partial class GiderTablolari : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GiderKategorileri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GiderKategorileri", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Giderler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsletmeId = table.Column<int>(type: "int", nullable: false),
                    GiderKategorisiId = table.Column<int>(type: "int", nullable: false),
                    Tutar = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Tarih = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Giderler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Giderler_GiderKategorileri_GiderKategorisiId",
                        column: x => x.GiderKategorisiId,
                        principalTable: "GiderKategorileri",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Giderler_Isletmeler_IsletmeId",
                        column: x => x.IsletmeId,
                        principalTable: "Isletmeler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "GiderKategorileri",
                columns: new[] { "Id", "Ad" },
                values: new object[,]
                {
                    { 1, "Kira" },
                    { 2, "Maaş" },
                    { 3, "Elektrik" },
                    { 4, "Su" },
                    { 5, "Doğalgaz" },
                    { 6, "İnternet / Telefon" },
                    { 7, "Vergi / Harç" },
                    { 8, "Bakım / Onarım" },
                    { 9, "Diğer" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Giderler_GiderKategorisiId",
                table: "Giderler",
                column: "GiderKategorisiId");

            migrationBuilder.CreateIndex(
                name: "IX_Giderler_IsletmeId",
                table: "Giderler",
                column: "IsletmeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Giderler");

            migrationBuilder.DropTable(
                name: "GiderKategorileri");
        }
    }
}
