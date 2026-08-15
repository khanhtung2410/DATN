using System.Collections.Generic;

namespace Cuahangchamsocthucung.Authentication.External
{
    public interface IExternalAuthConfiguration
    {
        List<ExternalLoginProviderInfo> Providers { get; }
    }
}
