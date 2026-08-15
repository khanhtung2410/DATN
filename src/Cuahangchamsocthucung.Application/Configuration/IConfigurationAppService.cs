using System.Threading.Tasks;
using Cuahangchamsocthucung.Configuration.Dto;

namespace Cuahangchamsocthucung.Configuration
{
    public interface IConfigurationAppService
    {
        Task ChangeUiTheme(ChangeUiThemeInput input);
    }
}
