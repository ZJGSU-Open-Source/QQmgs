using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Twitter.App.Controllers
{
    public class PhotoController : Controller
    {
        // GET: Photo
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }
    }
}