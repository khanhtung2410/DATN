using Abp.Application.Services;
using Abp.UI;
using Cuahangchamsocthucung.DichVu.Dto;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cuahangchamsocthucung.Web.Tests.Controllers
{
    public class DichVuTest : CuahangchamsocthucungWebTestBase
    {
        private readonly IDichVuAppService _dichVuAppService;

        public DichVuTest()
        {
            _dichVuAppService = Resolve<IDichVuAppService>();
        }

        [Fact]
        public async Task CreateDichVu_WithValidInput_ShouldCreate()
        {
            var input = new ThemDichVuDto
            {
                Tendichvu = "Test Dịch Vụ",
                Mota = "Mô tả test",
                Trangthai = true,
                BangGias = new System.Collections.Generic.List<ThemBangGiaDto>
                {
                    new ThemBangGiaDto
                    {
                        Loaithucung = "Chó",
                        Loailong = false,
                        Cannangtu = 0,
                        Cannangden = 5,
                        Giadv = 100000
                    }
                }
            };

            var createdId = await _dichVuAppService.Create(input);

            createdId.ShouldBeGreaterThan(0);

            await UsingDbContextAsync(async context =>
            {
                var dv = await context.DichVus.FindAsync(createdId);
                dv.ShouldNotBeNull();
                dv.TenDichVu.ShouldBe(input.Tendichvu);
            });
        }

        [Fact]
        public async Task CreateDichVu_WithInvalidPrice_ShouldThrow()
        {
            var input = new ThemDichVuDto
            {
                Tendichvu = "Test Dịch Vụ 2",
                Mota = "Mô tả",
                Trangthai = true,
                BangGias = new System.Collections.Generic.List<ThemBangGiaDto>
                {
                    new ThemBangGiaDto
                    {
                        Loaithucung = "Mèo",
                        Loailong = false,
                        Cannangtu = 1,
                        Cannangden = 3,
                        Giadv = 0 // invalid price, should trigger validation in app service
                    }
                }
            };

            await Should.ThrowAsync<UserFriendlyException>(async () => await _dichVuAppService.Create(input));
        }

        [Theory]
        [InlineData(-1, 5)] // Cannangtu < 0
        [InlineData(1, -5)] // Cannangden < 0
        [InlineData(5, 3)]  // Cannangden < Cannangtu
        [InlineData(5, 5)]  // Cannangden = Cannangtu
        public async Task CreateDichVu_WithInvalidWeight_ShouldThrow(
    int cannangtu,
    int cannangden)
        {
            var input = new ThemDichVuDto
            {
                Tendichvu = "Test Dịch Vụ",
                Mota = "Mô tả",
                Trangthai = true,
                BangGias = new List<ThemBangGiaDto>
        {
            new ThemBangGiaDto
            {
                Loaithucung = "Chó",
                Loailong = false,
                Cannangtu = cannangtu,
                Cannangden = cannangden,
                Giadv = 100000m
            }
        }
            };

            await Should.ThrowAsync<UserFriendlyException>(
                () => _dichVuAppService.Create(input));
        }

        [Fact]
        public async Task CreateDichVu_WithoutBangGia_ShouldThrow()
        {
            var input = new ThemDichVuDto
            {
                Tendichvu = "Tắm thú cưng",
                Mota = "Test",
                Trangthai = true,
                BangGias = new List<ThemBangGiaDto>()
            };

            await Should.ThrowAsync<UserFriendlyException>(
                () => _dichVuAppService.Create(input));
        }

        [Fact]
        public async Task GetDichVu_ShouldReturnDichVu()
        {
            var id = await CreateTestDichVu();

            var result = await _dichVuAppService.GetDichVu(id);

            result.ShouldNotBeNull();
            result.Id.ShouldBe(id);
            result.Tendichvu.ShouldBe("Dịch vụ test");
            result.BangGias.Count.ShouldBe(1);
        }


        [Fact]
        public async Task GetAll_ShouldReturnDichVuList()
        {
            await CreateTestDichVu();

            var result = await _dichVuAppService.GetAll();

            result.ShouldNotBeNull();
            result.Count.ShouldBeGreaterThan(0);

            result.Any(x => x.Tendichvu == "Dịch vụ test")
                .ShouldBeTrue();
        }


        [Fact]
        public async Task ChangeTrangThai_ShouldUpdateStatus()
        {
            var id = await CreateTestDichVu();

            await _dichVuAppService.ChangeTrangThai(
                new SuaTrangThaiDichVuDto
                {
                    Id = id,
                    Trangthai = false
                });

            await UsingDbContextAsync(async context =>
            {
                var dichVu = await context.DichVus.FindAsync(id);

                dichVu.ShouldNotBeNull();
                dichVu.TrangThai.ShouldBeFalse();
            });
        }


        [Fact]
        public async Task UpdateDichVu_ShouldUpdateInformation()
        {
            var id = await CreateTestDichVu();

            var input = new SuaDichVuDto
            {
                Id = id,
                Tendichvu = "Dịch vụ đã sửa",
                Mota = "Mô tả mới",
                Trangthai = true,
                BangGias = new List<SuaBangGiaDto>
                {
                    new SuaBangGiaDto
                    {
                        Id = 1,
                        Loaithucung = "Mèo",
                        Loailong = true,
                        Cannangtu = 1,
                        Cannangden = 5,
                        Giadv = 150000
                    }
                }
            };

            await _dichVuAppService.Update(input);

            await UsingDbContextAsync(async context =>
            {
                var dichVu = await context.DichVus.FindAsync(id);

                dichVu.ShouldNotBeNull();
                dichVu.TenDichVu.ShouldBe("Dịch vụ đã sửa");
                dichVu.MoTa.ShouldBe("Mô tả mới");
            });
        }

        [Fact]
        public async Task UpdateDichVu_WithoutBangGia_ShouldThrow()
        {
            var id = await CreateTestDichVu();

            var input = new SuaDichVuDto
            {
                Id = id,
                Tendichvu = "Dịch vụ sửa",
                Mota = "Test",
                Trangthai = true,
                BangGias = new List<SuaBangGiaDto>()
            };

            await Should.ThrowAsync<UserFriendlyException>(
                () => _dichVuAppService.Update(input));
        }

        [Theory]
        [InlineData("", "Mô tả")]
        [InlineData(null, "Mô tả")]
        public async Task UpdateDichVu_WithInvalidName_ShouldThrow(
            string tenDichVu,
            string mota)
        {
            var id = await CreateTestDichVu();

            var input = new SuaDichVuDto
            {
                Id = id,
                Tendichvu = tenDichVu,
                Mota = mota,
                Trangthai = true,
                BangGias = new List<SuaBangGiaDto>()
            };

            await Should.ThrowAsync<UserFriendlyException>(
                () => _dichVuAppService.Update(input));
        }


        [Theory]
        [InlineData(-1, 5)]
        [InlineData(5, 3)]
        [InlineData(5, 5)]
        public async Task UpdateDichVu_WithInvalidWeight_ShouldThrow(
            int cannangtu,
            int cannangden)
        {
            var id = await CreateTestDichVu();

            var input = new SuaDichVuDto
            {
                Id = id,
                Tendichvu = "Dịch vụ test",
                Mota = "Test",
                Trangthai = true,
                BangGias = new List<SuaBangGiaDto>
                {
                    new SuaBangGiaDto
                    {
                        Id = 1,
                        Loaithucung = "Chó",
                        Loailong = false,
                        Cannangtu = cannangtu,
                        Cannangden = cannangden,
                        Giadv = 100000
                    }
                }
            };

            await Should.ThrowAsync<UserFriendlyException>(
                () => _dichVuAppService.Update(input));
        }

        private async Task<int> CreateTestDichVu()
        {
            var input = new ThemDichVuDto
            {
                Tendichvu = "Dịch vụ test",
                Mota = "Test",
                Trangthai = true,
                BangGias = new List<ThemBangGiaDto>
                {
                    new ThemBangGiaDto
                    {
                        Loaithucung = "Chó",
                        Loailong = false,
                        Cannangtu = 1,
                        Cannangden = 5,
                        Giadv = 100000
                    }
                }
            };

            return await _dichVuAppService.Create(input);
        }
    }

}
