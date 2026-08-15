using System.Collections.Generic;
using Cuahangchamsocthucung.Roles.Dto;

namespace Cuahangchamsocthucung.Web.Models.Roles
{
    public class RoleListViewModel
    {
        public IReadOnlyList<PermissionDto> Permissions { get; set; }
    }
}
