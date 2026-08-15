using Abp.Configuration.Startup;
using Abp.Localization.Dictionaries;
using Abp.Localization.Dictionaries.Xml;
using Abp.Reflection.Extensions;

namespace Cuahangchamsocthucung.Localization
{
    public static class CuahangchamsocthucungLocalizationConfigurer
    {
        public static void Configure(ILocalizationConfiguration localizationConfiguration)
        {
            localizationConfiguration.Sources.Add(
                new DictionaryBasedLocalizationSource(CuahangchamsocthucungConsts.LocalizationSourceName,
                    new XmlEmbeddedFileLocalizationDictionaryProvider(
                        typeof(CuahangchamsocthucungLocalizationConfigurer).GetAssembly(),
                        "Cuahangchamsocthucung.Localization.SourceFiles"
                    )
                )
            );
        }
    }
}
