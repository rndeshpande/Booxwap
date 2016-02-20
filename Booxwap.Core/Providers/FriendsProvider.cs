namespace Booxwap.Core.Providers
{
    using System.Collections.Generic;
    using Social.FacebookHelpers;

    internal class FriendsProvider
    {
        private readonly Facebook _provider;

        public FriendsProvider()
        {
            _provider = new Facebook();
        }

        public IDictionary<string, string> GetFriendList(string accessToken, string userId)
        {
            return _provider.GetFriendList(accessToken, userId);
        }

        public string GetUserLocation(string fbid, string fbAccessToken)
        {
            return _provider.GetUserLocation(fbid, fbAccessToken);
        }
    }
}