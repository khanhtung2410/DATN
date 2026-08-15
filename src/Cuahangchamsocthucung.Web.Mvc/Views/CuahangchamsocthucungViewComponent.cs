using Abp.AspNetCore.Mvc.ViewComponents;

namespace Cuahangchamsocthucung.Web.Views
{
    public abstract class CuahangchamsocthucungViewComponent : AbpViewComponent
    {
        protected CuahangchamsocthucungViewComponent()
        {
            LocalizationSourceName = CuahangchamsocthucungConsts.LocalizationSourceName;
        }
    }
}
