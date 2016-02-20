namespace Booxwap.Social.FacebookHelpers.Interfaces
{
    using System.Collections.Generic;

    public interface ISocial
    {
        string GetAccessToken(string code);

        string GetOAuthUrl();

        IDictionary<string, string> GetFriendList(string accessToken, string userId);

        string GetUserLocation(string userId, string accessToken);

        string SetUserProperties(string accessToken);
    }
}