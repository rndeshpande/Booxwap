namespace Booxwap.Mvc.Controllers
{
    using System.Web.Mvc;
    using Models;
    using Providers;

    public class HomeController : Controller
    {
        private readonly DataProvider _provider;
        private readonly HomeModel _model;

        public HomeController()
        {
            _provider = new DataProvider();
            _model = new HomeModel();
        }

        public ActionResult Index()
        {
            _model.FoundBooks = _provider.GetMatchingBooks();
            _model.News = _provider.GetNewsStream();

            return View(_model);
        }
    }
}