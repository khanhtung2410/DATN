using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cuahangchamsocthucung.Migrations
{
    /// <inheritdoc />
    public partial class AddMucChiTieuToCauHinhVip : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "MucChiTieu",
                table: "CauHinhVips",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MucChiTieu",
                table: "CauHinhVips");
        }
    }
}
