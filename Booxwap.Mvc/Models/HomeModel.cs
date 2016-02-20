namespace Booxwap.Mvc.Models
{
    using System.Collections.Generic;

    public class HomeModel
    {
        public IList<FoundBooks> FoundBooks { get; set; }

        public IList<NewsModel> News { get; set; }
    }

    public class FoundBooks
    {
        public string Title { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}