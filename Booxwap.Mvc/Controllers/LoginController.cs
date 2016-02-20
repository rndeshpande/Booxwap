namespace Booxwap.Mvc.Controllers
{
    using System;
    using System.Web.Mvc;
    using Providers;
    using Utilities;

    public class LoginController : Controller
    {
        private readonly DataProvider _provider;

        public LoginController()
        {
            _provider = new DataProvider();
        }

        public ActionResult Index(string login)
        {
            try
            {
                if (!string.IsNullOrEmpty(login))
                {
                    Response.Redirect(_provider.GetRedirectionUrl());
                }

                if (Request.QueryString["code"] != null)
                {
                    _provider.InitializeSession(Request.QueryString["code"]);
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex.Message, ex.StackTrace);
            }

            return View();
        }
    }
}