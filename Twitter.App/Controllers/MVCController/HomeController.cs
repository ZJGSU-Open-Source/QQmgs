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
            : base(new QQmgsData())
        {
        }

        [Route("")]
        public ActionResult Index(int p = 1)
        {
            return RedirectToAction("Index", "Group");
        }

        [AllowAnonymous]
        public ActionResult Question(int q = 0)
        {
            if (q == 0)
            {
                var userLoginTrace = new UserLogTrace
                {
                    DatePosted = DateTime.Now,
                    IpAddress = IPAddressHelper.GetIpAddress(),
                    IsLoggedSucceeded = false,
                    PhoneNumber = "00000000000"
                };

                this.Data.UserLogTrace.Add(userLoginTrace);
                this.Data.SaveChanges();
            }

            return View(q);
        }

        [AllowAnonymous]
        public ActionResult NextQuestion(int cur)
        {
            var nxt = 0;
            if (cur >= 0 && cur < 4)
            {
                nxt = cur + 1;
            }

            return RedirectToAction("Question", new {q = nxt});
        }

        [AllowAnonymous]
        public ActionResult QuestionSubmit()
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