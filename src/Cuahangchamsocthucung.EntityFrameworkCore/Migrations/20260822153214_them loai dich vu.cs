using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cuahangchamsocthucung.Migrations
{
    /// <inheritdoc />
    public partial class themloaidichvu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LoaiDichVu",
                table: "DichVus",
                type: "int",
                nullable: false,
                defaultValue: 0);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LoaiDichVu",
                table: "DichVus");
        }
    }
}
