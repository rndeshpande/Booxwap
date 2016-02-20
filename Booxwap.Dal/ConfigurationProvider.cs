namespace Booxwap.Dal
{
    using System.Configuration;

    internal static class ConfigurationProvider
    {
        public static string ConnectionString
        {
            get
            {
                return ConfigurationManager.AppSettings["ConnectionString"];
            }
        }
    }
}