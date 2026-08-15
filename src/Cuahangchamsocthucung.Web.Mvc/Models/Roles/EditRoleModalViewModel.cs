using Abp.AutoMapper;
using Cuahangchamsocthucung.Roles.Dto;
using Cuahangchamsocthucung.Web.Models.Common;

namespace Cuahangchamsocthucung.Web.Models.Roles
{
    [AutoMapFrom(typeof(GetRoleForEditOutput))]
    public class EditRoleModalViewModel : GetRoleForEditOutput, IPermissionsEditViewModel
    {
        public bool HasPermission(FlatPermissionDto permission)
        {
            return GrantedPermissionNames.Contains(permission.Name);
        }
    }
}
