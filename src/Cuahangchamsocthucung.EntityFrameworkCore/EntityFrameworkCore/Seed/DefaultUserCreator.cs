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
                CreateUser();
            }
        }

        private void CreateUser()
        {
            var existingUser = _context.Users
                .IgnoreQueryFilters()
                .FirstOrDefault(x =>
                    x.UserName == "0912345678" &&
                    x.TenantId == 1);

            if (existingUser != null)
            {
                return;
            }

            var user = new User
            {
                TenantId = 1,
                UserName = "0912345678",
                Name = "Nguyễn",
                Surname = "Văn Toàn",
                EmailAddress = "khachhang@gmail.com",
                IsActive = true,
                IsEmailConfirmed = true
            };

            var result = _userManager
                .CreateAsync(user, "Abc@123456")
                .GetAwaiter()
                .GetResult();

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    result.Errors.Select(x => x.Description)
                );

                throw new Exception(
                    "Không thể tạo User khách hàng: " + errors);
            }
        }
    }
}