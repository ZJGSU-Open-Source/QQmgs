using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Twitter.App.BusinessLogic;
using Twitter.App.DataContracts;
using Twitter.Data.UnitOfWork;

namespace Twitter.App.Controllers.MVCController
{
    public class MonitorController : TwitterBaseController
    {
        public MonitorController()
            : base(new TwitterData())
        {
        }

        // GET: Monitar
        //public ActionResult Index()
        //{
        //    var items = this.Data.UserLoginTrace.All()
        //        .OrderByDescending(trace => trace.DatePosted)
        //        .Select(ViewModelsHelper.AsUserLoginTraceVideModel)
        //        .ToList();

        //    foreach (var item in items)
        //    {
        //        item.HighAccIpLocation = HighAccIpLocationClient.Query(item.IpAddress);
        //    }

        //    return View(items);
        //}
    }
}