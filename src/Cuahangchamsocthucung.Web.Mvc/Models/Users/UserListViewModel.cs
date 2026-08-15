using System.Collections.Generic;
using Cuahangchamsocthucung.Roles.Dto;

namespace Cuahangchamsocthucung.Web.Models.Users
{
    public class UserListViewModel
    {
        public IReadOnlyList<RoleDto> Roles { get; set; }
    }
}
