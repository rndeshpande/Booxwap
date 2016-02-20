using System.Linq;
using System.Web.Mvc;

namespace Booxwap.Mvc.Controllers
{
    using Models;
    using Providers;

    public class ListController : Controller
    {
        private readonly DataProvider _provider;

        public ListController()
        {
            _provider = new DataProvider();
        }

        public ActionResult Share()
        {
            var data = _provider.GetShareList();

            var result = data.Select(item => new ListModel
            {
                Id = item.Key,
                Title = item.Value
            }).ToList();

            return View(result);
        }

        public ActionResult Wish()
        {
            var data = _provider.GetWishList();

            var result = data.Select(item => new ListModel
            {
                Id = item.Key,
                Title = item.Value
            }).ToList();

            return View(result);
        }
    }
}