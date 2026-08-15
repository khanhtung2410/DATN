using Abp.Authorization;
using Cuahangchamsocthucung.Authorization.Roles;
using Cuahangchamsocthucung.Authorization.Users;

namespace Cuahangchamsocthucung.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {
        }
    }
}
