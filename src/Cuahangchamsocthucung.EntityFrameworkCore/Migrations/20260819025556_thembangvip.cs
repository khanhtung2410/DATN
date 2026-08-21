using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cuahangchamsocthucung.Migrations
{
    /// <inheritdoc />
    public partial class thembangvip : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CapVip",
                table: "KhachHangs");

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayBatDauVip",
                table: "KhachHangs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VipId",
                table: "KhachHangs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Vips",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenVip = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhanTramGiam = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ThoiGianHoatDong = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_Vips", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KhachHangs_VipId",
                table: "KhachHangs",
                column: "VipId");

            migrationBuilder.AddForeignKey(
                name: "FK_KhachHangs_Vips_VipId",
                table: "KhachHangs",
                column: "VipId",
                principalTable: "Vips",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KhachHangs_Vips_VipId",
                table: "KhachHangs");

            migrationBuilder.DropTable(
                name: "Vips");

            migrationBuilder.DropIndex(
                name: "IX_KhachHangs_VipId",
                table: "KhachHangs");

            migrationBuilder.DropColumn(
                name: "NgayBatDauVip",
                table: "KhachHangs");

            migrationBuilder.DropColumn(
                name: "VipId",
                table: "KhachHangs");

            migrationBuilder.AddColumn<int>(
                name: "CapVip",
                table: "KhachHangs",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
