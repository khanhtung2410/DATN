using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cuahangchamsocthucung.Migrations
{
    /// <inheritdoc />
    public partial class ngaysinhnhanvienkhongbatbuoc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateOnly>(
                name: "Ngaysinh",
                table: "NhanViens",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateOnly>(
                name: "Ngaysinh",
                table: "NhanViens",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1),
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);
        }
    }
}
