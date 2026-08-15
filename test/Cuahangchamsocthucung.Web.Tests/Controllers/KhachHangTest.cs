using System.Threading.Tasks;
using Abp.Domain.Uow;
using Xunit;
using Cuahangchamsocthucung.Authorization.Users;
using Cuahangchamsocthucung.KhachHang;
using Cuahangchamsocthucung.KhachHang.Dto;

namespace Cuahangchamsocthucung.Web.Tests.Controllers
{
    public class KhachHangTest : CuahangchamsocthucungWebTestBase
    {
        private readonly IKhachHangAppService _khachHangAppService;
        private readonly UserManager _userManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public KhachHangTest()
        {
            _khachHangAppService = Resolve<IKhachHangAppService>();
            _userManager = Resolve<UserManager>();
            _unitOfWorkManager = Resolve<IUnitOfWorkManager>();
        }

        [Fact]
        public async Task DangKy_Should_Create_KhachHang_User_And_CustomerRole()
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                // Arrange
                var input = new DangKyDto
                {
                    HoTen = "Nguyen Van A",
                    SDT = "0912345678",
                    Email = "nguyenvana@gmail.com",
                    MatKhau = "Abc@12345",
                    XacNhanMatKhau = "Abc@12345"
                };

                // Act
                var result = await _khachHangAppService.DangKy(input);

                // Assert - KhachHang
                Assert.NotNull(result);
                Assert.True(result.Id > 0);
                Assert.Equal(input.HoTen, result.Hoten);
                Assert.Equal(input.SDT, result.SDT);
                Assert.Equal(input.Email, result.Email);

                // Assert - User
                var user = await _userManager.FindByNameAsync(input.SDT);

                Assert.NotNull(user);
                Assert.Equal(input.SDT, user.UserName);
                Assert.Equal(input.HoTen, user.Name);
                Assert.Equal(input.Email, user.EmailAddress);
                Assert.True(user.IsActive);

                // Assert - Role
                var roles = await _userManager.GetRolesAsync(user);

                Assert.Contains("Customer", roles);

                await uow.CompleteAsync();
            }
        }
    }
}