namespace Booxwap.Utilities
{
    using System;
    using System.Configuration;
    using System.Globalization;
    using System.IO;

    public static class LogHelper
    {
        public static void LogException(string message, string stackTrace)
        {
            string strFileName = DateTime.Now.ToShortDateString().Replace("/", "") + "_Log.txt";

            TextWriter tw = new StreamWriter(ConfigurationManager.AppSettings["ExceptionLog"] + strFileName, true);
            tw.WriteLine("\n\n" + DateTime.Now.ToString(CultureInfo.InvariantCulture) + " " + message + "\n" + stackTrace + "\n\n");

            tw.Close();
        }
    }
}