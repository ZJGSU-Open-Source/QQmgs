using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Twitter.App.BusinessLogic;
using Twitter.Models;

namespace Twitter.App.Controllers
{
    using System.Linq;
    using System.Web.Mvc;

    using Twitter.App.Helper;
    using Twitter.App.Models.ViewModels;
    using Twitter.Data.UnitOfWork;

    using PagedList;

    [Authorize]
    [RoutePrefix("home")]
    public class HomeController : TwitterBaseController
    {
        public HomeController()
            : base(new TwitterData())
        {
        }

        [Route("")]
        public ActionResult Index(int p = 1)
        {
            return RedirectToAction("Index", "Group");
        }

        public ActionResult Question()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult About()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Job()
        {
            return View();
        }
    }
}