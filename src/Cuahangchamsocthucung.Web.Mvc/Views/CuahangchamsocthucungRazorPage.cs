using Abp.AspNetCore.Mvc.Views;
using Abp.Runtime.Session;
using Microsoft.AspNetCore.Mvc.Razor.Internal;

namespace Cuahangchamsocthucung.Web.Views
{
    public abstract class CuahangchamsocthucungRazorPage<TModel> : AbpRazorPage<TModel>
    {
        [RazorInject]
        public IAbpSession AbpSession { get; set; }

        protected CuahangchamsocthucungRazorPage()
        {
            LocalizationSourceName = CuahangchamsocthucungConsts.LocalizationSourceName;
        }
    }
}
