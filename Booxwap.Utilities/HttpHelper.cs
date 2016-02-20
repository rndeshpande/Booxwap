namespace Booxwap.Utilities
{
    using System;
    using System.IO;
    using System.Net;

    public class HttpHelper
    {
        public static string CallUrl(string url)
        {
            try
            {
                var request = WebRequest.Create(url);
                var response = request.GetResponse();
                var reader = new StreamReader(response.GetResponseStream());
                var strResponse = reader.ReadToEnd();

                return strResponse;
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex.Message + "\n" + url, ex.StackTrace);
                return "error";
            }
        }
    }
}