using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Twitter.App.BusinessLogic;
using Twitter.App.DataContracts;
using Twitter.Data.UnitOfWork;
using Twitter.Models.Trace;

namespace Twitter.App.Controllers.MVCController
{
    [RoutePrefix("monitor")]
    public class MonitorController : TwitterBaseController
    {
        public MonitorController()
            : base(new QQmgsData())
        {
        }

        // GET: Monitar
        [Route("")]
        public ActionResult Index()
        {
            // permision check
            var userName = this.User.Identity.GetUserName();
            if (userName != "13588201467")
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Permision denied");
            }

            var items = this.Data.UserLogTrace.All()
                .Where(trace => !trace.IsCached)
                .OrderByDescending(trace => trace.DatePosted);

            var traces = items.Select(ViewModelsHelper.AsUserLoginTraceVideModel).ToList();

            var tarceItems = items.ToList();

            // flag cache
            foreach (var item in tarceItems)
            {
                var trace = this.Data.UserLogTrace.Find(item.TraceId);
                trace.IsCached = true;
                this.Data.UserLogTrace.Update(trace);
                this.Data.SaveChanges();
            }

            // cache data
            foreach (var item in traces)
            {
                var phoneNumber = item.LoggedUserPhoneNumber;
                if (phoneNumber != null && phoneNumber != "00000000000")
                {
                    var user = this.Data.Users.All().FirstOrDefault(u => u.UserName == phoneNumber);

                    item.LoggedUserName = user?.RealName;
                }

                if (item.LoggedUserName == null)
                {
                    item.LoggedUserName = "Failed Login Try";
                }

                var queryResult = HighAccIpLocationClient.Query(item.IpAddress);
                item.HighAccIpLocation = queryResult;

                var highAccLocationByIpResult = new HighAccLocationByIpResult
                {
                    FormattedAddress = item.HighAccIpLocation?.Content?.FormattedAddress,
                    Confidence = item.HighAccIpLocation?.Content?.Confidence,
                    AdminAreaCode = item.HighAccIpLocation?.Content?.AddressComponent?.AdminAreaCode,
                    City = item.HighAccIpLocation?.Content?.AddressComponent?.City,
                    Country = item.HighAccIpLocation?.Content?.AddressComponent?.Country,
                    District = item.HighAccIpLocation?.Content?.AddressComponent?.District,
                    Province = item.HighAccIpLocation?.Content?.AddressComponent?.Province,
                    Street = item.HighAccIpLocation?.Content?.AddressComponent?.Street,
                    StreetNumber = item.HighAccIpLocation?.Content?.AddressComponent?.StreetNumber,
                    Business = item.HighAccIpLocation?.Content?.Business,
                    LocId = item.HighAccIpLocation?.Content?.LocId,
                    Lat = item.HighAccIpLocation?.Content?.Location?.Lat,
                    Lng = item.HighAccIpLocation?.Content?.Location?.Lng,
                    Radius = item.HighAccIpLocation?.Content?.Radius,
                    Error = item.HighAccIpLocation?.Result?.Error,
                    LocalTime = item.HighAccIpLocation?.Result?.LocalTime,
                    LoggedUserName = item.LoggedUserName,
                    DatePosted = item.DatePosted,
                    IpAddress = item.IpAddress,
                    LoggedUserPhoneNumber = item.LoggedUserPhoneNumber,
                    Id = item.Id,
                    IsLoggedSucceeded = item.IsLoggedSucceeded
                };

                this.Data.HighAccLocationByIpResult.Add(highAccLocationByIpResult);
                this.Data.SaveChanges();
            }

            var cachedData = this.Data.HighAccLocationByIpResult.All()
                .OrderByDescending(trace => trace.DatePosted)
                .Select(ViewModelsHelper.AsUserLoginTraceViewModel)
                .ToList();

            return View(cachedData);
        }
    }
}