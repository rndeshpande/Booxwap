namespace Booxwap.Mvc.Providers
{
    using Core.Models;
    using System.Collections.Generic;

    public static class SessionProvider
    {
        public static string FacebookAccessToken
        {
            get;
            set;
        }

        public static UserModel UserInfo
        {
            get;
            set;
        }

        public static IDictionary<string, string> FriendList
        {
            get;
            set;
        }

        public static string UserId
        {
            get;
            set;
        }
    }
}