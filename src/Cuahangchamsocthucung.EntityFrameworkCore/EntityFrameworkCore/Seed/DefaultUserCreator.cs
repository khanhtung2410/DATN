using Abp.Domain.Uow;
using Cuahangchamsocthucung.Authorization.Users;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Cuahangchamsocthucung.EntityFrameworkCore.Seed
{
    public class DefaultUserCreator
    {
        private readonly CuahangchamsocthucungDbContext _context;
        private readonly UserManager _userManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public DefaultUserCreator(
            CuahangchamsocthucungDbContext context,
            UserManager userManager,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _context = context;
            _userManager = userManager;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public void Create()
        {
            using (_unitOfWorkManager.Current.SetTenantId(1))
            {
                CreateUsers();
            }
        }

        private void CreateUsers()
        {
            var users = new[]
            {
                new { UserName = "0912345678", Name = "Nguyễn", Surname = "Văn Toàn", Email = "khachhang01@gmail.com" },
                new { UserName = "0912345679", Name = "Trần", Surname = "Thị Lan", Email = "khachhang02@gmail.com" },
                new { UserName = "0912345680", Name = "Lê", Surname = "Hoàng Nam", Email = "khachhang03@gmail.com" },
                new { UserName = "0912345681", Name = "Phạm", Surname = "Minh Anh", Email = "khachhang04@gmail.com" },
                new { UserName = "0912345682", Name = "Vũ", Surname = "Đức Minh", Email = "khachhang05@gmail.com" },
                new { UserName = "0912345683", Name = "Đặng", Surname = "Thu Hà", Email = "khachhang06@gmail.com" },
                new { UserName = "0912345684", Name = "Bùi", Surname = "Quang Huy", Email = "khachhang07@gmail.com" },
                new { UserName = "0912345685", Name = "Đỗ", Surname = "Ngọc Mai", Email = "khachhang08@gmail.com" },
                new { UserName = "0912345686", Name = "Hồ", Surname = "Gia Bảo", Email = "khachhang09@gmail.com" },
                new { UserName = "0912345687", Name = "Ngô", Surname = "Phương Thảo", Email = "khachhang10@gmail.com" },
                new { UserName = "0912345688", Name = "Dương", Surname = "Tuấn Anh", Email = "khachhang11@gmail.com" },
                new { UserName = "0912345689", Name = "Mai", Surname = "Khánh Linh", Email = "khachhang12@gmail.com" },
                new { UserName = "0912345690", Name = "Phan", Surname = "Hữu Phước", Email = "khachhang13@gmail.com" },
                new { UserName = "0912345691", Name = "Tạ", Surname = "Thanh Tâm", Email = "khachhang14@gmail.com" },
                new { UserName = "0912345692", Name = "Cao", Surname = "Minh Khang", Email = "khachhang15@gmail.com" }
            };

            foreach (var item in users)
            {
                var existingUser = _context.Users
                    .IgnoreQueryFilters()
                    .FirstOrDefault(x => x.UserName == item.UserName && x.TenantId == 1);

                if (existingUser != null)
                    continue;

                var user = new User
                {
                    TenantId = 1,
                    UserName = item.UserName,
                    Name = item.Name,
                    Surname = item.Surname,
                    EmailAddress = item.Email,
                    IsActive = true,
                    IsEmailConfirmed = true
                };

                var result = _userManager.CreateAsync(user, "Abc@123456").GetAwaiter().GetResult();

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(x => x.Description));
                    throw new Exception("Không thể tạo User " + item.UserName + ": " + errors);
                }
            }
        }
    }
}