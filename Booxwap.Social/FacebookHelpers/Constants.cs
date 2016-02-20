namespace Booxwap.Social.FacebookHelpers
{
    internal static class Constants
    {
        public const string GraphApiUrl = "https://graph.facebook.com/";
        public const string FqlApiUrl = "https://graph.facebook.com//fql";
        public const string AuthorizationUrl = "https://graph.facebook.com/oauth/authorize";
        public const string AccessTokenUrl = "https://graph.facebook.com/oauth/access_token";
        public const string CallbackUrl = "http://localhost/BookBridge/FBCallBack.aspx";
        public const string OAuthUrl = "https://www.facebook.com/dialog/oauth";

        public const string AccessToken = "access_token";
        public const string Permissions = "public_profile,user_friends,email";

        public const string FqlQuery =
            "SELECT+uid,name,pic_small_with_logo+FROM+user+WHERE+uid+IN(SELECT+uid2+FROM+friend+WHERE+uid1+IN(SELECT+uid+FROM+user+WHERE+uid+IN(SELECT+uid2+FROM+friend+WHERE+uid1={0})+and+is_app_user=1) )+and+is_app_user=1";
    }
}