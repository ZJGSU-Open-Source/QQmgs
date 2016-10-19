using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Twitter.App.Controllers.MobileController
{
    [RoutePrefix("m")]
    public class MController : Controller
    {
        // GET: M
        [Route("")]
        public ActionResult Redirect()
        {
            return RedirectToAction("Activity");
        }

        // GET: Activity
        [Route("activity")]
        public ActionResult Activity()
        {
            return View();
        }
    }
}