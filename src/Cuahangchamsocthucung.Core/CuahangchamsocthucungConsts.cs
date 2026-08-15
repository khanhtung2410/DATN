using Cuahangchamsocthucung.Debugging;

namespace Cuahangchamsocthucung
{
    public class CuahangchamsocthucungConsts
    {
        public const string LocalizationSourceName = "Cuahangchamsocthucung";

        public const string ConnectionStringName = "Default";

        public const bool MultiTenancyEnabled = true;


        /// <summary>
        /// Default pass phrase for SimpleStringCipher decrypt/encrypt operations
        /// </summary>
        public static readonly string DefaultPassPhrase =
            DebugHelper.IsDebug ? "gsKxGZ012HLL3MI5" : "c48d9f8ccf1f4513bab1a98b0b044d81";
    }
}
