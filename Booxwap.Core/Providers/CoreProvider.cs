namespace Booxwap.Core.Providers
{
    using System.Collections.Generic;
    using System.Linq;
    using Dal;
    using Models;
    using Newtonsoft.Json;
    using Social.FacebookHelpers;

    public class CoreProvider
    {
        private Facebook _provider;
        private readonly DbProvider _dbProvider;

        public CoreProvider()
        {
            _dbProvider = new DbProvider();
        }

        #region List Operations
        public IDictionary<string, string> GetWishList(string userId)
        {
            return _dbProvider.GetWishList(userId);
        }

        public IDictionary<string, string> GetShareList(string userId)
        {
            return _dbProvider.GetShareList(userId);
        }

        public bool SaveList(string bbid, string bookTitle, string mode, string author, string amazonUrl, string bookType, string asin)
        {
            return _dbProvider.SaveList(bbid, bookTitle, mode, author, amazonUrl, bookType, asin);
        }

        public bool SaveWishList(string userId, string asin)
        {
            return _dbProvider.AddToDatabase(userId, asin, "Wish");
        }

        public bool SaveShareList(string userId, string asin)
        {
            return _dbProvider.AddToDatabase(userId, asin, "Share");
        }

        public bool AddBookToList(string bbid, string bookTitle, string mode, string author, string amazonUrl, string bookType, string asin)
        {
            return _dbProvider.AddBookToList(bbid, bookTitle, mode, author, amazonUrl, bookType, asin);
        }
        #endregion List Operations

        #region User Operations
        public UserModel SetUserProperties(string userFacebookAccessToken)
        {
            _provider = new Facebook();
            return JsonConvert.DeserializeObject<UserModel>(_provider.SetUserProperties(userFacebookAccessToken));
        }

        public string UpdateUserInfo(string userId, string userFirstName, string userLastName, string userFacebookAccnt)
        {
            return _dbProvider.UpdateUserInfo(userId, userFirstName, userLastName, userFacebookAccnt);
        }
        #endregion User Operations


        public IList<Core.Models.MatchListModel> GetMatchingBooks(string userId, IDictionary<string, string> friendList)
        {
            var matchList = _dbProvider.GetMatchingBooks(userId);

            //Only return books that belong to the logged-in user's friend list
            var result = matchList.Where(m => friendList.ContainsKey(m.UserFacebookId)).ToList<Dal.Models.MatchListModel>();

            return result.Select(item => new Models.MatchListModel
            {
                BookId = item.BookId,
                Title = item.Title,
                UserId = item.UserId,
                UserFacebookId = item.UserFacebookId,
                UserFirstName = item.UserFirstName,
                UserLastName = item.UserLastName
            }).ToList();
        }

        public IList<Models.NewsModel> GetNewsStream(string userId, IDictionary<string, string> friendList)
        {
            var news = _dbProvider.GetNewsStream(userId);
            var result = news.Where(m => friendList.ContainsKey(m.UserFacebookId)).ToList<Dal.Models.NewsModel>();

            return result.Select(item => new Models.NewsModel
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

        public IDictionary<string, string> GetFoundBooks(string userId, string bookName)
        {
            return _dbProvider.GetFoundBooksList(bookName, userId);
        }

        public IDictionary<string, string> GetFriendList(string accessToken, string userId)
        {
            var provider = new FriendsProvider();
            return provider.GetFriendList(accessToken, userId);
        }

        public bool SaveReview(string bbid, string bookTitle, string author, string amazonUrl, string bookType, string content)
        {
            return _dbProvider.SaveReview(bbid, bookTitle, author, amazonUrl, bookType, content);
        }
    }
}