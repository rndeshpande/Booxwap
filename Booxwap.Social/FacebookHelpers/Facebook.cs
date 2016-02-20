namespace Booxwap.Social.FacebookHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using Interfaces;
    using Newtonsoft.Json;
    using Utilities;

    public class Facebook : ISocial
    {
        #region Private Variables

        private static string _consumerKey = "";
        private static string _consumerSecret = "";
        private static string _redirectUri = "";

        #endregion Private Variables

        #region Properties

        private static string ConsumerKey
        {
            get
            {
                if (_consumerKey.Length == 0)
                {
                    _consumerKey = ConfigurationManager.AppSettings["AppID"];
                }
                return _consumerKey;
            }
        }

        private static string ConsumerSecret
        {
            get
            {
                if (_consumerSecret.Length == 0)
                {
                    _consumerSecret = ConfigurationManager.AppSettings["AppSecret"];
                }
                return _consumerSecret;
            }
        }

        private static string RedirectUri
        {
            get
            {
                if (_redirectUri.Length == 0)
                {
                    _redirectUri = ConfigurationManager.AppSettings["FBRedirectURI"];
                }
                return _redirectUri;
            }
        }

        private static FriendListSqlModel FacebookQuery(string fbQuery, string fbAccessToken)
        {
            var url = string.Format("{0}?q={1}&access_token={2}", Constants.FqlApiUrl, fbQuery, fbAccessToken);
            var jsonResponse = HttpHelper.CallUrl(url);
            return JsonConvert.DeserializeObject<FriendListSqlModel>(jsonResponse);
        }

        #endregion Properties

        #region Public Methods

        public string GetAccessToken(string code)
        {
            var accessTokenUrl = string.Format("{0}?client_id={1}&redirect_uri={2}&client_secret={3}&code={4}&scope={5}", Constants.AccessTokenUrl, ConsumerKey, RedirectUri, ConsumerSecret, code, Constants.Permissions);
            return HttpHelper.CallUrl(accessTokenUrl).Replace("access_token=", "");
        }

        public string GetOAuthUrl()
        {
            return string.Format("{0}?client_id={1}&redirect_uri={2}&scope={3}", Constants.OAuthUrl, ConsumerKey, RedirectUri, Constants.Permissions);
        }

        public IDictionary<string, string> GetFriendList(string accessToken, string userId)
        {
            try
            {
                var url = string.Format("{0}/me/friends?access_token={1}", Constants.GraphApiUrl, accessToken);

                var jsonResponse = HttpHelper.CallUrl(url);

                var friendList = JsonConvert.DeserializeObject<FriendListModel>(jsonResponse);

                var htFriendList = new Dictionary<string, string>();

                foreach (var item in friendList.List.Where(item => !htFriendList.ContainsKey(item.Id)))
                {
                    htFriendList.Add(item.Id, item.Name);
                }

                var friends = FacebookQuery(string.Format(Constants.FqlQuery, userId), accessToken);
                foreach (var item in friends.List.Where(item => !htFriendList.ContainsKey(item.Id)))
                {
                    htFriendList.Add(item.Id, item.Name);
                }

                return htFriendList;
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex.Message, ex.StackTrace);
                return null;
            }
        }

        public string GetUserLocation(string fbid, string fbAccessToken)
        {
            var url = string.Format("{0}{1}?access_token={2}", Constants.GraphApiUrl, fbid, fbAccessToken);
            var jsonResponse = HttpHelper.CallUrl(url);
            var userProps = JsonConvert.DeserializeObject<UserModel>(jsonResponse);
            return userProps.Location.Name;
        }

        public string SetUserProperties(string userFacebookAccessToken)
        {
            var url = string.Format("{0}/me?access_token={1}", Constants.GraphApiUrl, userFacebookAccessToken);
            var jsonResponse = HttpHelper.CallUrl(url);
            return jsonResponse;
        }

        #endregion Public Methods
    }
}