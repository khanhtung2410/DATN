using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Runtime.Session;
using Cuahangchamsocthucung.Configuration.Dto;

namespace Cuahangchamsocthucung.Configuration
{
    [AbpAuthorize]
    public class ConfigurationAppService : CuahangchamsocthucungAppServiceBase, IConfigurationAppService
    {
        public async Task ChangeUiTheme(ChangeUiThemeInput input)
        {
            await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
        }
    }
}
