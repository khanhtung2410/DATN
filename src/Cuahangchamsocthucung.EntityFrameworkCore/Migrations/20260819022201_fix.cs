using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cuahangchamsocthucung.Migrations
{
    /// <inheritdoc />
    public partial class fix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LichChamSocId",
                table: "HoaDons",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_HoaDons_LichChamSocId",
                table: "HoaDons",
                column: "LichChamSocId");

            migrationBuilder.AddForeignKey(
                name: "FK_HoaDons_LichChamSocs_LichChamSocId",
                table: "HoaDons",
                column: "LichChamSocId",
                principalTable: "LichChamSocs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HoaDons_LichChamSocs_LichChamSocId",
                table: "HoaDons");

            migrationBuilder.DropIndex(
                name: "IX_HoaDons_LichChamSocId",
                table: "HoaDons");

            migrationBuilder.DropColumn(
                name: "LichChamSocId",
                table: "HoaDons");
        }
    }
}
