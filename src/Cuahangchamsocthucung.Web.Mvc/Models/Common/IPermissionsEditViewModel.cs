using System.Collections.Generic;
using Cuahangchamsocthucung.Roles.Dto;

namespace Cuahangchamsocthucung.Web.Models.Common
{
    public interface IPermissionsEditViewModel
    {
        List<FlatPermissionDto> Permissions { get; set; }
    }
}