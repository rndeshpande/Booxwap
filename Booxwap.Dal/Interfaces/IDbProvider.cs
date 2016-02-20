namespace Booxwap.Dal.Interfaces
{
    using System.Collections.Generic;
    using System.Data;
    using Models;

    internal interface IDbProvider
    {
        IDictionary<string, string> GetFoundBooksList(string bookTitle, string userId);

        IDictionary<string, string> GetWishList(string fbId);

        IList<MatchListModel> GetMatchingBooks(string userId);

        IDictionary<string, string> GetShareList(string userId);

        IList<NewsModel> GetNewsStream(string userId);

        bool SaveReview(string bbid, string bookTitle, string author, string amazonUrl, string bookType, string content);

        string UpdateUserInfo(string fbid, string firstName, string lastName, string fbProfileLink);

        bool AddToDatabase(string bbid, string bookAsin, string mode);

        bool AddBookToList(string bbid, string bookTitle, string mode, string author, string amazonUrl, string bookType,
            string asin);

        bool SaveList(string bbid, string bookTitle, string mode, string author, string amazonUrl, string strBookType,
            string strAsin);
    }
}