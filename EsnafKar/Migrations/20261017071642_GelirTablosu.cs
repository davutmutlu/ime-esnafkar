using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EsnafKar.Migrations
{
    /// <inheritdoc />
    public partial class GelirTablosu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Gelirler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsletmeId = table.Column<int>(type: "int", nullable: false),
                    Tarih = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Tutar = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gelirler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Gelirler_Isletmeler_IsletmeId",
                        column: x => x.IsletmeId,
                        principalTable: "Isletmeler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Gelirler_IsletmeId",
                table: "Gelirler",
                column: "IsletmeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Gelirler");
        }
    }
}
