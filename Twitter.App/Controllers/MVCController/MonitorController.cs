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
        public ActionResult Index()
        {
            var items = this.Data.UserLogTrace.All()
                .OrderByDescending(trace => trace.DatePosted)
                .Select(ViewModelsHelper.AsUserLoginTraceVideModel)
                .ToList();
            
            foreach (var item in items)
            {
                var phoneNumber = item.LoggedUserPhoneNumber;
                if (phoneNumber != null && phoneNumber != "00000000000")
                {
                    var user = this.Data.Users.All().FirstOrDefault(u => u.UserName == phoneNumber);

                    item.LoggedUserName = user?.RealName;
                }

                var result = HighAccIpLocationClient.Query(item.IpAddress);

                item.HighAccIpLocation = result;
            }

            return View(items);
        }
    }
}