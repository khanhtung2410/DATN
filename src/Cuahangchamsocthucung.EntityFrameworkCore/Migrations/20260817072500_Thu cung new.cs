using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cuahangchamsocthucung.Migrations
{
    /// <inheritdoc />
    public partial class Thucungnew : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "ThuCungs",
                type: "nvarchar(400)",
                maxLength: 400,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ThuCungId",
                table: "LichChamSocs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_LichChamSocs_ThuCungId",
                table: "LichChamSocs",
                column: "ThuCungId");

            migrationBuilder.AddForeignKey(
                name: "FK_LichChamSocs_ThuCungs_ThuCungId",
                table: "LichChamSocs",
                column: "ThuCungId",
                principalTable: "ThuCungs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LichChamSocs_ThuCungs_ThuCungId",
                table: "LichChamSocs");

            migrationBuilder.DropIndex(
                name: "IX_LichChamSocs_ThuCungId",
                table: "LichChamSocs");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "ThuCungs");

            migrationBuilder.DropColumn(
                name: "ThuCungId",
                table: "LichChamSocs");
        }
    }
}
