using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cuahangchamsocthucung.Migrations
{
    /// <inheritdoc />
    public partial class themcauhinhvip : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreationTime",
                table: "Vips");

            migrationBuilder.DropColumn(
                name: "CreatorUserId",
                table: "Vips");

            migrationBuilder.DropColumn(
                name: "DeleterUserId",
                table: "Vips");

            migrationBuilder.DropColumn(
                name: "DeletionTime",
                table: "Vips");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Vips");

            migrationBuilder.DropColumn(
                name: "LastModificationTime",
                table: "Vips");

            migrationBuilder.DropColumn(
                name: "LastModifierUserId",
                table: "Vips");

            migrationBuilder.DropColumn(
                name: "PhanTramGiam",
                table: "Vips");

            migrationBuilder.DropColumn(
                name: "TrangThai",
                table: "Vips");

            migrationBuilder.DropColumn(
                name: "NgayBatDauVip",
                table: "KhachHangs");

            migrationBuilder.RenameColumn(
                name: "ThoiGianHoatDong",
                table: "Vips",
                newName: "TenantId");

            migrationBuilder.AlterColumn<string>(
                name: "TenVip",
                table: "Vips",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CapVip",
                table: "Vips",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CauHinhVips",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    VipId = table.Column<int>(type: "int", nullable: false),
                    PhanTramGiam = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    TuNgay = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DenNgay = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CauHinhVips", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CauHinhVips_Vips_VipId",
                        column: x => x.VipId,
                        principalTable: "Vips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CauHinhVips_VipId",
                table: "CauHinhVips",
                column: "VipId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CauHinhVips");

            migrationBuilder.DropColumn(
                name: "CapVip",
                table: "Vips");

            migrationBuilder.RenameColumn(
                name: "TenantId",
                table: "Vips",
                newName: "ThoiGianHoatDong");

            migrationBuilder.AlterColumn<string>(
                name: "TenVip",
                table: "Vips",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationTime",
                table: "Vips",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "CreatorUserId",
                table: "Vips",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DeleterUserId",
                table: "Vips",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionTime",
                table: "Vips",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Vips",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModificationTime",
                table: "Vips",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "LastModifierUserId",
                table: "Vips",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PhanTramGiam",
                table: "Vips",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "TrangThai",
                table: "Vips",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayBatDauVip",
                table: "KhachHangs",
                type: "datetime2",
                nullable: true);
        }
    }
}
