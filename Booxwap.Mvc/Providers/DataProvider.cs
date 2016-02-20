namespace Booxwap.Mvc.Providers
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Core.Providers;
    using Models;
    using Social.AmazonHelpers;
    using Social.FacebookHelpers;
    using Social.FacebookHelpers.Interfaces;

    public class DataProvider
    {
        #region Private variables

        private readonly CoreProvider _provider;
        private static readonly ISocial Social;

        #endregion Private variables

        #region Private Methods

        private void SetUserProperties()
        {
            SessionProvider.UserInfo = _provider.SetUserProperties(SessionProvider.FacebookAccessToken);
        }

        private void UpdateUserInfo()
        {
            SessionProvider.UserId = _provider.UpdateUserInfo(SessionProvider.UserInfo.Id,
                SessionProvider.UserInfo.FirstName, SessionProvider.UserInfo.LastName, SessionProvider.UserInfo.FbLink);
        }

        private void GetFriendList()
        {
            SessionProvider.FriendList = _provider.GetFriendList(SessionProvider.FacebookAccessToken,
                SessionProvider.UserInfo.Id);
        }

        private static void SetLoginSession(string authCode)
        {
            SessionProvider.FacebookAccessToken = Social.GetAccessToken(authCode);
        }

        #endregion Private Methods

        #region Public Methods

        static DataProvider()
        {
            Social = new Facebook();
        }

        public DataProvider()
        {
            _provider = new CoreProvider();
        }

        public IDictionary<string, string> GetWishList()
        {
            return _provider.GetWishList(SessionProvider.UserId);
        }

        public IList<FoundBooks> GetMatchingBooks()
        {
            var data = _provider.GetMatchingBooks(SessionProvider.UserId, SessionProvider.FriendList);
            return data.Select(item => new FoundBooks
            {
                Title = item.Title,
                FirstName = item.UserFirstName,
                LastName = item.UserLastName
            }).ToList();
        }

        public IDictionary<string, string> GetShareList()
        {
            return _provider.GetShareList(SessionProvider.UserId);
        }

        public DataTable SearchBook(string searchBy, string name)
        {
            var amazonQuery = new AmazonQuery();
            return amazonQuery.Search(searchBy, name);
        }

        public bool SaveList(string bbid, string bookTitle, string mode, string author, string amazonUrl, string bookType, string asin)
        {
            return _provider.SaveList(bbid, bookTitle, mode, author, amazonUrl, bookType, asin);
        }

        public IDictionary<string, string> GetFoundBooks(string bookName)
        {
            return _provider.GetFoundBooks(SessionProvider.UserId, bookName);
        }

        public void InitializeSession(string authCode)
        {
            SetLoginSession(authCode);
            SetUserProperties();
            GetFriendList();
            UpdateUserInfo();
        }

        public IList<Models.NewsModel> GetNewsStream()
        {
            var data = _provider.GetNewsStream(SessionProvider.UserId, SessionProvider.FriendList);
            return data.Select(item => new Models.NewsModel
            {
                BookName = item.BookName,
                BookAsin = item.BookAsin,
                UserFacebookId = item.UserFacebookId,
                UserFacebookUrl = item.UserFacebookUrl,
                UserFullName = item.UserFullName,
                ActionType = item.ActionType,
                ActionDate = item.ActionDate,
                ListItemId = item.ListItemId
            }).ToList();
        }

        public bool SaveWishList(string asin)
        {
            return _provider.SaveWishList(SessionProvider.UserId, asin);
        }

        public bool SaveShareList(string asin)
        {
            return _provider.SaveShareList(SessionProvider.UserId, asin);
        }

        public bool AddBookToList(string bbid, string bookTitle, string mode, string author, string amazonUrl, string bookType, string asin)
        {
            return _provider.AddBookToList(bbid, bookTitle, mode, author, amazonUrl, bookType, asin);
        }

        public bool SaveReview(string bbid, string bookTitle, string author, string amazonUrl, string bookType, string content)
        {
            return _provider.SaveReview(bbid, bookTitle, author, amazonUrl, bookType, content);
        }

        public string GetRedirectionUrl()
        {
            return Social.GetOAuthUrl();
        }

        public bool IsLoggedIn()
        {
            return !string.IsNullOrEmpty(SessionProvider.FacebookAccessToken);
        }

        #endregion Public Methods
    }
}