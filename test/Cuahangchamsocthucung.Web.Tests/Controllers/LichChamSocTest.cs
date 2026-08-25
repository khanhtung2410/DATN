//using Abp.Authorization.Users;
//using Abp.UI;
//using Cuahangchamsocthucung.Entities;
//using Cuahangchamsocthucung.Enum;
//using Cuahangchamsocthucung.LichChamSoc.Dto;
//using Shouldly;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Xunit;

//namespace Cuahangchamsocthucung.Web.Tests.Controllers
//{
//    public class LichChamSocTest : CuahangchamsocthucungWebTestBase
//    {
//        private readonly ILichChamSocAppService _lichChamSocAppService;

//        public LichChamSocTest()
//        {
//            _lichChamSocAppService = Resolve<ILichChamSocAppService>();
//            LoginAsDefaultTenantAdmin();
//        }

//        #region CREATE - USER ĐẶT LỊCH

//        [Fact]
//        public async Task CreateLichChamSoc_WithValidInput_ShouldCreate()
//        {
//            var data = await CreateTestDataAsync();
//            var thoiGian = FutureTime();

//            var id = await _lichChamSocAppService.Create(new ThemLichChamSocDto
//            {
//                ThuCungId = data.ThuCungId,
//                DichVuId = data.DichVuId,
//                BangGiaId = data.BangGiaId,
//                CanNang = 3,
//                ThoiGian = thoiGian
//            });

//            id.ShouldBeGreaterThan(0);

//            await UsingDbContextAsync(async context =>
//            {
//                var lich = await context.LichChamSocs.FindAsync(id);

//                lich.ShouldNotBeNull();
//                lich.ThuCungId.ShouldBe(data.ThuCungId);
//                lich.DichVuId.ShouldBe(data.DichVuId);
//                lich.BangGiaId.ShouldBe(data.BangGiaId);
//                lich.KhachHangId.ShouldBe(data.KhachHangId);
//                lich.NhanVienId.ShouldBeNull();
//                lich.ThoiGian.ShouldBe(thoiGian);
//                lich.TrangThai.ShouldBe(TrangThaiLichChamSoc.ChoXacNhan);
//            });
//        }

//        [Fact]
//        public async Task CreateLichChamSoc_WithNullInput_ShouldThrow()
//        {
//            await Should.ThrowAsync<UserFriendlyException>(
//                () => _lichChamSocAppService.Create(null));
//        }

//        [Fact]
//        public async Task CreateLichChamSoc_WithoutThuCung_ShouldThrow()
//        {
//            var data = await CreateTestDataAsync();

//            await Should.ThrowAsync<UserFriendlyException>(() =>
//                _lichChamSocAppService.Create(new ThemLichChamSocDto
//                {
//                    ThuCungId = 0,
//                    DichVuId = data.DichVuId,
//                    BangGiaId = data.BangGiaId,
//                    ThoiGian = FutureTime()
//                }));
//        }

//        [Fact]
//        public async Task CreateLichChamSoc_WithoutDichVu_ShouldThrow()
//        {
//            var data = await CreateTestDataAsync();

//            await Should.ThrowAsync<UserFriendlyException>(() =>
//                _lichChamSocAppService.Create(new ThemLichChamSocDto
//                {
//                    ThuCungId = data.ThuCungId,
//                    DichVuId = 0,
//                    BangGiaId = data.BangGiaId,
//                    ThoiGian = FutureTime()
//                }));
//        }

//        [Fact]
//        public async Task CreateLichChamSoc_WithoutBangGia_ShouldThrow()
//        {
//            var data = await CreateTestDataAsync();

//            await Should.ThrowAsync<UserFriendlyException>(() =>
//                _lichChamSocAppService.Create(new ThemLichChamSocDto
//                {
//                    ThuCungId = data.ThuCungId,
//                    DichVuId = data.DichVuId,
//                    BangGiaId = 0,
//                    ThoiGian = FutureTime()
//                }));
//        }

//        [Fact]
//        public async Task CreateLichChamSoc_WithoutTime_ShouldThrow()
//        {
//            var data = await CreateTestDataAsync();

//            await Should.ThrowAsync<UserFriendlyException>(() =>
//                _lichChamSocAppService.Create(new ThemLichChamSocDto
//                {
//                    ThuCungId = data.ThuCungId,
//                    DichVuId = data.DichVuId,
//                    BangGiaId = data.BangGiaId,
//                    ThoiGian = default
//                }));
//        }

//        [Fact]
//        public async Task CreateLichChamSoc_WithPastTime_ShouldThrow()
//        {
//            var data = await CreateTestDataAsync();

//            await Should.ThrowAsync<UserFriendlyException>(() =>
//                _lichChamSocAppService.Create(new ThemLichChamSocDto
//                {
//                    ThuCungId = data.ThuCungId,
//                    DichVuId = data.DichVuId,
//                    BangGiaId = data.BangGiaId,
//                    ThoiGian = DateTime.Now.AddHours(-1)
//                }));
//        }

//        [Fact]
//        public async Task CreateLichChamSoc_WithPetOfAnotherCustomer_ShouldThrow()
//        {
//            var data = await CreateTestDataAsync();
//            var anotherPet = await CreateAnotherCustomerPetAsync();

//            await Should.ThrowAsync<UserFriendlyException>(() =>
//                _lichChamSocAppService.Create(new ThemLichChamSocDto
//                {
//                    ThuCungId = anotherPet,
//                    DichVuId = data.DichVuId,
//                    BangGiaId = data.BangGiaId,
//                    ThoiGian = FutureTime()
//                }));
//        }

//        [Fact]
//        public async Task CreateLichChamSoc_WithInactivePet_ShouldThrow()
//        {
//            var data = await CreateTestDataAsync();
//            var petId = await CreateInactivePetAsync(data.KhachHangId);

//            await Should.ThrowAsync<UserFriendlyException>(() =>
//                _lichChamSocAppService.Create(new ThemLichChamSocDto
//                {
//                    ThuCungId = petId,
//                    DichVuId = data.DichVuId,
//                    BangGiaId = data.BangGiaId,
//                    ThoiGian = FutureTime()
//                }));
//        }

//        [Fact]
//        public async Task CreateLichChamSoc_WithInactiveService_ShouldThrow()
//        {
//            var data = await CreateTestDataAsync();
//            var serviceId = await CreateInactiveDichVuAsync();
//            var priceId = await CreateBangGiaAsync(serviceId);

//            await Should.ThrowAsync<UserFriendlyException>(() =>
//                _lichChamSocAppService.Create(new ThemLichChamSocDto
//                {
//                    ThuCungId = data.ThuCungId,
//                    DichVuId = serviceId,
//                    BangGiaId = priceId,
//                    ThoiGian = FutureTime()
//                }));
//        }

//        [Fact]
//        public async Task CreateLichChamSoc_WithBangGiaOfAnotherService_ShouldThrow()
//        {
//            var data = await CreateTestDataAsync();
//            var anotherServiceId = await CreateDichVuAsync("Dịch vụ khác");
//            var anotherPriceId = await CreateBangGiaAsync(anotherServiceId);

//            await Should.ThrowAsync<UserFriendlyException>(() =>
//                _lichChamSocAppService.Create(new ThemLichChamSocDto
//                {
//                    ThuCungId = data.ThuCungId,
//                    DichVuId = data.DichVuId,
//                    BangGiaId = anotherPriceId,
//                    ThoiGian = FutureTime()
//                }));
//        }

//        [Fact]
//        public async Task CreateLichChamSoc_WithDuplicatePetTime_ShouldThrow()
//        {
//            var data = await CreateTestDataAsync();
//            var time = FutureTime();

//            await CreateLichAsync(data, time);

//            await Should.ThrowAsync<UserFriendlyException>(() =>
//                _lichChamSocAppService.Create(new ThemLichChamSocDto
//                {
//                    ThuCungId = data.ThuCungId,
//                    DichVuId = data.DichVuId,
//                    BangGiaId = data.BangGiaId,
//                    ThoiGian = time
//                }));
//        }

//        [Fact]
//        public async Task CreateLichChamSoc_WhenPreviousScheduleCancelled_ShouldAllowCreate()
//        {
//            var data = await CreateTestDataAsync();
//            var time = FutureTime();

//            var oldId = await CreateLichAsync(data, time);
//            await _lichChamSocAppService.HuyLichChamSoc(oldId);

//            var newId = await CreateLichAsync(data, time);

//            newId.ShouldBeGreaterThan(0);
//        }

//        #endregion

//        #region HỦY LỊCH - USER

//        [Fact]
//        public async Task HuyLichChamSoc_WhenWaiting_ShouldCancel()
//        {
//            var data = await CreateTestDataAsync();
//            var id = await CreateLichAsync(data, FutureTime());

//            await _lichChamSocAppService.HuyLichChamSoc(id);

//            await AssertStatusAsync(id, TrangThaiLichChamSoc.DaHuy);
//        }

//        [Fact]
//        public async Task HuyLichChamSoc_WhenConfirmed_ShouldCancel()
//        {
//            var data = await CreateTestDataAsync();
//            var id = await CreateLichAsync(data, FutureTime());

//            await UsingDbContextAsync(async context =>
//            {
//                var lich = await context.LichChamSocs.FindAsync(id);
//                lich.TrangThai = TrangThaiLichChamSoc.DaXacNhan;
//            });

//            await _lichChamSocAppService.HuyLichChamSoc(id);

//            await AssertStatusAsync(id, TrangThaiLichChamSoc.DaHuy);
//        }

//        [Theory]
//        [InlineData(TrangThaiLichChamSoc.HoanThanh)]
//        [InlineData(TrangThaiLichChamSoc.DaHuy)]
//        [InlineData(TrangThaiLichChamSoc.BiTuChoi)]
//        public async Task HuyLichChamSoc_WithInvalidStatus_ShouldThrow(TrangThaiLichChamSoc status)
//        {
//            var data = await CreateTestDataAsync();
//            var id = await CreateLichAsync(data, FutureTime());

//            await UsingDbContextAsync(async context =>
//            {
//                var lich = await context.LichChamSocs.FindAsync(id);
//                lich.TrangThai = status;
//            });

//            await Should.ThrowAsync<UserFriendlyException>(
//                () => _lichChamSocAppService.HuyLichChamSoc(id));
//        }

//        [Fact]
//        public async Task HuyLichChamSoc_WithNonExistingId_ShouldThrow()
//        {
//            await Should.ThrowAsync<UserFriendlyException>(
//                () => _lichChamSocAppService.HuyLichChamSoc(int.MaxValue));
//        }

//        [Fact]
//        public async Task HuyLichChamSoc_WithAnotherCustomerSchedule_ShouldThrow()
//        {
//            var data = await CreateTestDataAsync();
//            var anotherCustomerId = await CreateAnotherCustomerAsync();
//            var petId = await CreatePetAsync(anotherCustomerId);
//            var id = await CreateLichDirectAsync(anotherCustomerId, petId, data.DichVuId, data.BangGiaId, FutureTime());

//            await Should.ThrowAsync<UserFriendlyException>(
//                () => _lichChamSocAppService.HuyLichChamSoc(id));
//        }

//        #endregion

//        #region TỪ CHỐI - ADMIN

//        [Fact]
//        public async Task TuChoiLichChamSoc_WhenWaiting_ShouldReject()
//        {
//            var data = await CreateTestDataAsync();
//            var id = await CreateLichAsync(data, FutureTime());

//            await _lichChamSocAppService.TuChoiLichChamSoc(id);

//            await AssertStatusAsync(id, TrangThaiLichChamSoc.BiTuChoi);
//        }

//        [Fact]
//        public async Task TuChoiLichChamSoc_WhenConfirmed_ShouldReject()
//        {
//            var data = await CreateTestDataAsync();
//            var id = await CreateLichAsync(data, FutureTime());

//            await UsingDbContextAsync(async context =>
//            {
//                var lich = await context.LichChamSocs.FindAsync(id);
//                lich.TrangThai = TrangThaiLichChamSoc.DaXacNhan;
//            });

//            await _lichChamSocAppService.TuChoiLichChamSoc(id);

//            await AssertStatusAsync(id, TrangThaiLichChamSoc.BiTuChoi);
//        }

//        [Theory]
//        [InlineData(TrangThaiLichChamSoc.HoanThanh)]
//        [InlineData(TrangThaiLichChamSoc.DaHuy)]
//        [InlineData(TrangThaiLichChamSoc.BiTuChoi)]
//        public async Task TuChoiLichChamSoc_WithInvalidStatus_ShouldThrow(TrangThaiLichChamSoc status)
//        {
//            var data = await CreateTestDataAsync();
//            var id = await CreateLichAsync(data, FutureTime());

//            await UsingDbContextAsync(async context =>
//            {
//                var lich = await context.LichChamSocs.FindAsync(id);
//                lich.TrangThai = status;
//            });

//            await Should.ThrowAsync<UserFriendlyException>(
//                () => _lichChamSocAppService.TuChoiLichChamSoc(id));
//        }

//        [Fact]
//        public async Task TuChoiLichChamSoc_WithNonExistingId_ShouldThrow()
//        {
//            await Should.ThrowAsync<UserFriendlyException>(
//                () => _lichChamSocAppService.TuChoiLichChamSoc(int.MaxValue));
//        }

//        #endregion

//        #region PHÂN CÔNG NHÂN VIÊN - ADMIN

//        [Fact]
//        public async Task PhanCongNhanVien_WithValidInput_ShouldAssign()
//        {
//            var data = await CreateTestDataAsync();
//            var nhanVienId = await CreateNhanVienAsync();
//            var id = await CreateLichAsync(data, FutureTime());

//            await _lichChamSocAppService.PhanCongNhanVien(id, nhanVienId);

//            await UsingDbContextAsync(async context =>
//            {
//                var lich = await context.LichChamSocs.FindAsync(id);

//                lich.NhanVienId.ShouldBe(nhanVienId);
//                lich.TrangThai.ShouldBe(TrangThaiLichChamSoc.DaXacNhan);
//            });
//        }

//        [Fact]
//        public async Task PhanCongNhanVien_WithNonExistingSchedule_ShouldThrow()
//        {
//            var nhanVienId = await CreateNhanVienAsync();

//            await Should.ThrowAsync<UserFriendlyException>(
//                () => _lichChamSocAppService.PhanCongNhanVien(int.MaxValue, nhanVienId));
//        }

//        [Fact]
//        public async Task PhanCongNhanVien_WithNonExistingEmployee_ShouldThrow()
//        {
//            var data = await CreateTestDataAsync();
//            var id = await CreateLichAsync(data, FutureTime());

//            await Should.ThrowAsync<UserFriendlyException>(
//                () => _lichChamSocAppService.PhanCongNhanVien(id, int.MaxValue));
//        }

//        [Fact]
//        public async Task PhanCongNhanVien_WithInactiveEmployee_ShouldThrow()
//        {
//            var data = await CreateTestDataAsync();
//            var nhanVienId = await CreateNhanVienAsync(false);
//            var id = await CreateLichAsync(data, FutureTime());

//            await Should.ThrowAsync<UserFriendlyException>(
//                () => _lichChamSocAppService.PhanCongNhanVien(id, nhanVienId));
//        }

//        [Theory]
//        [InlineData(TrangThaiLichChamSoc.DaXacNhan)]
//        [InlineData(TrangThaiLichChamSoc.DaHuy)]
//        [InlineData(TrangThaiLichChamSoc.BiTuChoi)]
//        [InlineData(TrangThaiLichChamSoc.HoanThanh)]
//        public async Task PhanCongNhanVien_WithInvalidScheduleStatus_ShouldThrow(TrangThaiLichChamSoc status)
//        {
//            var data = await CreateTestDataAsync();
//            var nhanVienId = await CreateNhanVienAsync();
//            var id = await CreateLichAsync(data, FutureTime());

//            await UsingDbContextAsync(async context =>
//            {
//                var lich = await context.LichChamSocs.FindAsync(id);
//                lich.TrangThai = status;
//            });

//            await Should.ThrowAsync<UserFriendlyException>(
//                () => _lichChamSocAppService.PhanCongNhanVien(id, nhanVienId));
//        }

//        [Fact]
//        public async Task PhanCongNhanVien_WhenEmployeeHasOverlappingSchedule_ShouldThrow()
//        {
//            var data = await CreateTestDataAsync();
//            var nhanVienId = await CreateNhanVienAsync();
//            var firstTime = FutureTime();
//            var secondTime = firstTime.AddMinutes(30);

//            var firstId = await CreateLichAsync(data, firstTime);

//            await UsingDbContextAsync(async context =>
//            {
//                var lich = await context.LichChamSocs.FindAsync(firstId);
//                lich.NhanVienId = nhanVienId;
//                lich.TrangThai = TrangThaiLichChamSoc.DaXacNhan;
//            });

//            var secondData = await CreateTestDataAsync();
//            var secondId = await CreateLichAsync(secondData, secondTime);

//            await Should.ThrowAsync<UserFriendlyException>(
//                () => _lichChamSocAppService.PhanCongNhanVien(secondId, nhanVienId));
//        }

//        [Fact]
//        public async Task PhanCongNhanVien_WhenEmployeeScheduleEndsExactlyAtStart_ShouldAllow()
//        {
//            var data = await CreateTestDataAsync();
//            var nhanVienId = await CreateNhanVienAsync();
//            var firstTime = FutureTime();
//            var secondTime = firstTime.AddMinutes(30);

//            var firstId = await CreateLichAsync(data, firstTime);

//            await UsingDbContextAsync(async context =>
//            {
//                var lich = await context.LichChamSocs.FindAsync(firstId);
//                lich.NhanVienId = nhanVienId;
//                lich.TrangThai = TrangThaiLichChamSoc.DaXacNhan;
//            });

//            var secondData = await CreateTestDataAsync();
//            var secondId = await CreateLichAsync(secondData, secondTime);

//            await _lichChamSocAppService.PhanCongNhanVien(secondId, nhanVienId);

//            await AssertEmployeeAsync(secondId, nhanVienId);
//        }

//        [Fact]
//        public async Task PhanCongNhanVien_WhenOtherScheduleCancelled_ShouldAllow()
//        {
//            var data = await CreateTestDataAsync();
//            var nhanVienId = await CreateNhanVienAsync();
//            var firstTime = FutureTime();

//            var firstId = await CreateLichAsync(data, firstTime);

//            await UsingDbContextAsync(async context =>
//            {
//                var lich = await context.LichChamSocs.FindAsync(firstId);
//                lich.NhanVienId = nhanVienId;
//                lich.TrangThai = TrangThaiLichChamSoc.DaHuy;
//            });

//            var secondData = await CreateTestDataAsync();
//            var secondId = await CreateLichAsync(secondData, firstTime.AddMinutes(15));

//            await _lichChamSocAppService.PhanCongNhanVien(secondId, nhanVienId);

//            await AssertEmployeeAsync(secondId, nhanVienId);
//        }

//        #endregion

//        #region UPDATE - ADMIN

//        [Fact]
//        public async Task UpdateLichChamSoc_WithValidInput_ShouldUpdate()
//        {
//            var data = await CreateTestDataAsync();
//            var id = await CreateLichAsync(data, FutureTime());

//            var newServiceId = await CreateDichVuAsync("Dịch vụ sửa");
//            var newPriceId = await CreateBangGiaAsync(newServiceId);
//            var newTime = FutureTime().AddHours(2);

//            await _lichChamSocAppService.Update(new SuaLichChamSocDto
//            {
//                Id = id,
//                DichVuId = newServiceId,
//                BangGiaId = newPriceId,
//                KhachHangId = data.KhachHangId,
//                NhanVienId = null,
//                ThoiGian = newTime,
//                TrangThai = TrangThaiLichChamSoc.ChoXacNhan
//            });

//            await UsingDbContextAsync(async context =>
//            {
//                var lich = await context.LichChamSocs.FindAsync(id);

//                lich.DichVuId.ShouldBe(newServiceId);
//                lich.BangGiaId.ShouldBe(newPriceId);
//                lich.ThoiGian.ShouldBe(newTime);
//            });
//        }

//        [Fact]
//        public async Task UpdateLichChamSoc_WithNonExistingId_ShouldThrow()
//        {
//            var data = await CreateTestDataAsync();

//            await Should.ThrowAsync<UserFriendlyException>(() =>
//                _lichChamSocAppService.Update(new SuaLichChamSocDto
//                {
//                    Id = int.MaxValue,
//                    DichVuId = data.DichVuId,
//                    BangGiaId = data.BangGiaId,
//                    ThoiGian = FutureTime()
//                }));
//        }

//        [Fact]
//        public async Task UpdateLichChamSoc_WithoutDichVu_ShouldThrow()
//        {
//            var data = await CreateTestDataAsync();
//            var id = await CreateLichAsync(data, FutureTime());

//            await Should.ThrowAsync<UserFriendlyException>(() =>
//                _lichChamSocAppService.Update(new SuaLichChamSocDto
//                {
//                    Id = id,
//                    DichVuId = 0,
//                    BangGiaId = data.BangGiaId,
//                    ThoiGian = FutureTime()
//                }));
//        }

//        [Fact]
//        public async Task UpdateLichChamSoc_WithoutBangGia_ShouldThrow()
//        {
//            var data = await CreateTestDataAsync();
//            var id = await CreateLichAsync(data, FutureTime());

//            await Should.ThrowAsync<UserFriendlyException>(() =>
//                _lichChamSocAppService.Update(new SuaLichChamSocDto
//                {
//                    Id = id,
//                    DichVuId = data.DichVuId,
//                    BangGiaId = 0,
//                    ThoiGian = FutureTime()
//                }));
//        }

//        [Fact]
//        public async Task UpdateLichChamSoc_WithBangGiaOfAnotherService_ShouldThrow()
//        {
//            var data = await CreateTestDataAsync();
//            var id = await CreateLichAsync(data, FutureTime());
//            var anotherServiceId = await CreateDichVuAsync("Dịch vụ khác");
//            var anotherPriceId = await CreateBangGiaAsync(anotherServiceId);

//            await Should.ThrowAsync<UserFriendlyException>(() =>
//                _lichChamSocAppService.Update(new SuaLichChamSocDto
//                {
//                    Id = id,
//                    DichVuId = data.DichVuId,
//                    BangGiaId = anotherPriceId,
//                    ThoiGian = FutureTime()
//                }));
//        }

//        [Fact]
//        public async Task UpdateLichChamSoc_WithDefaultTime_ShouldThrow()
//        {
//            var data = await CreateTestDataAsync();
//            var id = await CreateLichAsync(data, FutureTime());

//            await Should.ThrowAsync<UserFriendlyException>(() =>
//                _lichChamSocAppService.Update(new SuaLichChamSocDto
//                {
//                    Id = id,
//                    DichVuId = data.DichVuId,
//                    BangGiaId = data.BangGiaId,
//                    ThoiGian = default
//                }));
//        }

//        [Fact]
//        public async Task UpdateLichChamSoc_WithPastTime_ShouldThrow()
//        {
//            var data = await CreateTestDataAsync();
//            var id = await CreateLichAsync(data, FutureTime());

//            await Should.ThrowAsync<UserFriendlyException>(() =>
//                _lichChamSocAppService.Update(new SuaLichChamSocDto
//                {
//                    Id = id,
//                    DichVuId = data.DichVuId,
//                    BangGiaId = data.BangGiaId,
//                    ThoiGian = DateTime.Now.AddHours(-1)
//                }));
//        }

//        [Fact]
//        public async Task UpdateLichChamSoc_WhenEmployeeHasOverlappingSchedule_ShouldThrow()
//        {
//            var data = await CreateTestDataAsync();
//            var nhanVienId = await CreateNhanVienAsync();
//            var firstTime = FutureTime();

//            var firstId = await CreateLichAsync(data, firstTime);

//            await UsingDbContextAsync(async context =>
//            {
//                var lich = await context.LichChamSocs.FindAsync(firstId);
//                lich.NhanVienId = nhanVienId;
//                lich.TrangThai = TrangThaiLichChamSoc.DaXacNhan;
//            });

//            var secondData = await CreateTestDataAsync();
//            var secondId = await CreateLichAsync(secondData, firstTime.AddHours(2));

//            await Should.ThrowAsync<UserFriendlyException>(() =>
//                _lichChamSocAppService.Update(new SuaLichChamSocDto
//                {
//                    Id = secondId,
//                    DichVuId = secondData.DichVuId,
//                    BangGiaId = secondData.BangGiaId,
//                    NhanVienId = nhanVienId,
//                    ThoiGian = firstTime.AddMinutes(15)
//                }));
//        }

//        [Fact]
//        public async Task UpdateLichChamSoc_ShouldNotConflictWithItself()
//        {
//            var data = await CreateTestDataAsync();
//            var nhanVienId = await CreateNhanVienAsync();
//            var time = FutureTime();

//            var id = await CreateLichAsync(data, time);

//            await UsingDbContextAsync(async context =>
//            {
//                var lich = await context.LichChamSocs.FindAsync(id);
//                lich.NhanVienId = nhanVienId;
//            });

//            await _lichChamSocAppService.Update(new SuaLichChamSocDto
//            {
//                Id = id,
//                DichVuId = data.DichVuId,
//                BangGiaId = data.BangGiaId,
//                NhanVienId = nhanVienId,
//                ThoiGian = time
//            });

//            await AssertEmployeeAsync(id, nhanVienId);
//        }

//        #endregion

//        #region GET

//        [Fact]
//        public async Task GetLichChamSoc_ShouldReturnSchedule()
//        {
//            var data = await CreateTestDataAsync();
//            var id = await CreateLichAsync(data, FutureTime());

//            var result = await _lichChamSocAppService.GetLichChamSoc(id);

//            result.ShouldNotBeNull();
//            result.Id.ShouldBe(id);
//            result.ThuCungId.ShouldBe(data.ThuCungId);
//            result.DichVuId.ShouldBe(data.DichVuId);
//            result.BangGiaId.ShouldBe(data.BangGiaId);
//        }

//        [Fact]
//        public async Task GetLichChamSoc_WithNonExistingId_ShouldThrow()
//        {
//            await Should.ThrowAsync<UserFriendlyException>(
//                () => _lichChamSocAppService.GetLichChamSoc(int.MaxValue));
//        }

//        [Fact]
//        public async Task GetLichChamSocCuaToi_ShouldReturnCurrentCustomerSchedules()
//        {
//            var data = await CreateTestDataAsync();
//            var id = await CreateLichAsync(data, FutureTime());

//            var result = await _lichChamSocAppService.GetLichChamSocCuaToi();

//            result.ShouldNotBeNull();
//            result.Any(x => x.Id == id).ShouldBeTrue();
//        }

//        [Fact]
//        public async Task GetLichSuLichChamSocCuaToi_ShouldReturnCancelledSchedule()
//        {
//            var data = await CreateTestDataAsync();
//            var id = await CreateLichAsync(data, FutureTime());

//            await _lichChamSocAppService.HuyLichChamSoc(id);

//            var result = await _lichChamSocAppService.GetLichSuLichChamSocCuaToi();

//            result.Any(x =>
//                x.Id == id &&
//                x.TrangThai == TrangThaiLichChamSoc.DaHuy).ShouldBeTrue();
//        }

//        #endregion

//        #region HELPERS

//        private async Task<TestData> CreateTestDataAsync()
//        {
//            var userId = AbpSession.UserId.Value;
//            var khachHangId = await GetOrCreateKhachHangAsync(userId);
//            var thuCungId = await CreatePetAsync(khachHangId);
//            var dichVuId = await CreateDichVuAsync("Dịch vụ test");
//            var bangGiaId = await CreateBangGiaAsync(dichVuId);

//            return new TestData
//            {
//                KhachHangId = khachHangId,
//                ThuCungId = thuCungId,
//                DichVuId = dichVuId,
//                BangGiaId = bangGiaId
//            };
//        }

//        private async Task<int> GetOrCreateKhachHangAsync(long userId)
//        {
//            return await UsingDbContextAsync(async context =>
//            {
//                var khachHang = context.KhachHangs.FirstOrDefault(x => x.UserId == userId);

//                if (khachHang != null)
//                    return khachHang.Id;

//                var entity = new Entities.KhachHang
//                {
//                    UserId = userId,
//                    Hoten = "Khách hàng test"
//                };

//                context.KhachHangs.Add(entity);
//                await context.SaveChangesAsync();

//                return entity.Id;
//            });
//        }

//        private async Task<int> CreateAnotherCustomerAsync()
//        {
//            return await UsingDbContextAsync(async context =>
//            {
//                var currentUserId = AbpSession.UserId.Value;
//                var user = context.Users.FirstOrDefault(x => x.Id != currentUserId);

//                if (user == null)
//                    throw new Exception("Không tìm thấy user khác để tạo khách hàng test.");

//                var khachHang = new Entities.KhachHang
//                {
//                    UserId = user.Id,
//                    Hoten = "Khách hàng khác"
//                };

//                context.KhachHangs.Add(khachHang);
//                await context.SaveChangesAsync();

//                return khachHang.Id;
//            });
//        }

//        private async Task<int> CreateAnotherCustomerPetAsync()
//        {
//            var customerId = await CreateAnotherCustomerAsync();
//            return await CreatePetAsync(customerId);
//        }

//        private async Task<int> CreatePetAsync(int khachHangId, bool active = true)
//        {
//            return await UsingDbContextAsync(async context =>
//            {
//                var pet = new Entities.ThuCung
//                {
//                    KhachHangId = khachHangId,
//                    TenThuCung = "Thú cưng test " + Guid.NewGuid().ToString("N").Substring(0, 6),
//                    TrangThai = active
//                };

//                context.ThuCungs.Add(pet);
//                await context.SaveChangesAsync();

//                return pet.Id;
//            });
//        }

//        private async Task<int> CreateInactivePetAsync(int khachHangId)
//        {
//            return await CreatePetAsync(khachHangId, false);
//        }

//        private async Task<int> CreateDichVuAsync(string name)
//        {
//            return await UsingDbContextAsync(async context =>
//            {
//                var dichVu = new Entities.DichVu
//                {
//                    TenDichVu = name + " " + Guid.NewGuid().ToString("N").Substring(0, 6),
//                    MoTa = "Dịch vụ test",
//                    TrangThai = true
//                };

//                context.DichVus.Add(dichVu);
//                await context.SaveChangesAsync();

//                return dichVu.Id;
//            });
//        }

//        private async Task<int> CreateInactiveDichVuAsync()
//        {
//            return await UsingDbContextAsync(async context =>
//            {
//                var dichVu = new Entities.DichVu
//                {
//                    TenDichVu = "Dịch vụ không hoạt động " + Guid.NewGuid().ToString("N").Substring(0, 6),
//                    MoTa = "Test",
//                    TrangThai = false
//                };

//                context.DichVus.Add(dichVu);
//                await context.SaveChangesAsync();

//                return dichVu.Id;
//            });
//        }

//        private async Task<int> CreateBangGiaAsync(int dichVuId)
//        {
//            return await UsingDbContextAsync(async context =>
//            {
//                var bangGia = new BangGia
//                {
//                    DichVuId = dichVuId,
//                    Loaithucung = "Chó",
//                    Loailong = false,
//                    Cannangtu = 1,
//                    Cannangden = 5,
//                    Giadv = 100000,
//                    ThoiGianPhut = 30
//                };

//                context.BangGias.Add(bangGia);
//                await context.SaveChangesAsync();

//                return bangGia.Id;
//            });
//        }

//        private async Task<int> CreateNhanVienAsync(bool active = true)
//        {
//            return await UsingDbContextAsync(async context =>
//            {
//                var nhanVien = new Entities.NhanVien
//                {
//                    Hoten = "Nhân viên test " + Guid.NewGuid().ToString("N").Substring(0, 6),
//                    Trangthai = active
//                };

//                context.NhanViens.Add(nhanVien);
//                await context.SaveChangesAsync();

//                return nhanVien.Id;
//            });
//        }

//        private async Task<int> CreateLichAsync(TestData data, DateTime time)
//        {
//            return await _lichChamSocAppService.Create(new ThemLichChamSocDto
//            {
//                ThuCungId = data.ThuCungId,
//                DichVuId = data.DichVuId,
//                BangGiaId = data.BangGiaId,
//                CanNang = 3,
//                ThoiGian = time
//            });
//        }

//        private async Task<int> CreateLichDirectAsync(
//            int khachHangId,
//            int thuCungId,
//            int dichVuId,
//            int bangGiaId,
//            DateTime time)
//        {
//            return await UsingDbContextAsync(async context =>
//            {
//                var lich = new Entities.LichChamSoc
//                {
//                    KhachHangId = khachHangId,
//                    ThuCungId = thuCungId,
//                    DichVuId = dichVuId,
//                    BangGiaId = bangGiaId,
//                    NhanVienId = null,
//                    ThoiGian = time,
//                    TrangThai = TrangThaiLichChamSoc.ChoXacNhan
//                };

//                context.LichChamSocs.Add(lich);
//                await context.SaveChangesAsync();

//                return lich.Id;
//            });
//        }

//        private async Task AssertStatusAsync(int id, TrangThaiLichChamSoc expected)
//        {
//            await UsingDbContextAsync(async context =>
//            {
//                var lich = await context.LichChamSocs.FindAsync(id);

//                lich.ShouldNotBeNull();
//                lich.TrangThai.ShouldBe(expected);
//            });
//        }

//        private async Task AssertEmployeeAsync(int id, int nhanVienId)
//        {
//            await UsingDbContextAsync(async context =>
//            {
//                var lich = await context.LichChamSocs.FindAsync(id);

//                lich.ShouldNotBeNull();
//                lich.NhanVienId.ShouldBe(nhanVienId);
//                lich.TrangThai.ShouldBe(TrangThaiLichChamSoc.DaXacNhan);
//            });
//        }

//        private static DateTime FutureTime()
//        {
//            var now = DateTime.Now.AddDays(2);
//            return new DateTime(
//                now.Year,
//                now.Month,
//                now.Day,
//                10,
//                0,
//                0);
//        }

//        private class TestData
//        {
//            public int KhachHangId { get; set; }
//            public int ThuCungId { get; set; }
//            public int DichVuId { get; set; }
//            public int BangGiaId { get; set; }
//        }

//        #endregion
//    }
//}