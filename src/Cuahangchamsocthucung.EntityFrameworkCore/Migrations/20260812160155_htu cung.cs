using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cuahangchamsocthucung.Migrations
{
    /// <inheritdoc />
    public partial class htucung : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Trangthai",
                table: "DichVus",
                newName: "TrangThai");

            migrationBuilder.RenameColumn(
                name: "Tendichvu",
                table: "DichVus",
                newName: "TenDichVu");

            migrationBuilder.RenameColumn(
                name: "Mota",
                table: "DichVus",
                newName: "MoTa");

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "KhachHangs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ThuCungs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KhachHangId = table.Column<int>(type: "int", nullable: false),
                    TenThuCung = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LoaiThuCung = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThuCungs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ThuCungs_KhachHangs_KhachHangId",
                        column: x => x.KhachHangId,
                        principalTable: "KhachHangs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ThuCungs_KhachHangId",
                table: "ThuCungs",
                column: "KhachHangId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ThuCungs");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "KhachHangs");

            migrationBuilder.RenameColumn(
                name: "TrangThai",
                table: "DichVus",
                newName: "Trangthai");

            migrationBuilder.RenameColumn(
                name: "TenDichVu",
                table: "DichVus",
                newName: "Tendichvu");

            migrationBuilder.RenameColumn(
                name: "MoTa",
                table: "DichVus",
                newName: "Mota");
        }
    }
}
