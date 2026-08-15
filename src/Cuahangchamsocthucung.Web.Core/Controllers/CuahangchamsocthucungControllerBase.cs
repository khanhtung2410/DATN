using Abp.AspNetCore.Mvc.Controllers;
using Abp.IdentityFramework;
using Microsoft.AspNetCore.Identity;

namespace Cuahangchamsocthucung.Controllers
{
    public abstract class CuahangchamsocthucungControllerBase: AbpController
    {
        protected CuahangchamsocthucungControllerBase()
        {
            LocalizationSourceName = CuahangchamsocthucungConsts.LocalizationSourceName;
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
