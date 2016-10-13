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
using Twitter.App.Models.ViewModels;
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

        [Route("")]
        public ActionResult Index()
        {
            // permision check
            var userName = this.User.Identity.GetUserName();
            if (userName != "13588201467")
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"Permision denied, current user phone number is {userName}");
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

        [Route("bio")]
        public ActionResult Bio()
        {

            var totalActivityNumber = this.Data.Activity.All().Count();
            var totalPhotoNumber = this.Data.Photo.All().Count();
            var totalReplyNumber = this.Data.Reply.All().Count();
            var totalReviewNumber = this.Data.CourseReview.All().Count();
            var totalTweetNumber = this.Data.Tweets.All().Count();
            var totalUserNumber = this.Data.Users.All().Count();
            var totalGroupNumber = this.Data.Group.All().Count();

            var result = new UserBioStatisticsViewModel
            {
                TotalActivityNumber = totalActivityNumber,
                TotalPhotoNumber =  totalPhotoNumber,
                TotalReplyNumber =  totalReplyNumber,
                TotalReviewNumber =  totalReviewNumber,
                TotalTweetNumber =  totalTweetNumber,
                TotalUserNumber =  totalUserNumber,
                TotalGroupNumber = totalGroupNumber
            };

            return PartialView(result);
        }

        [Route("user")]
        public ActionResult Statistics()
        {
            var result = new List<UserStatisticsViewModel>();

            for (var dateTime = DateTime.Now; dateTime.Month != DateTime.Now.AddMonths(-3).Month; dateTime = dateTime.AddDays(-1))
            {
                var newUserNumber =
                this.Data.Users.All()
                    .Count(
                        user => user.RegisteredTime.Month == dateTime.Month && user.RegisteredTime.Day == dateTime.Day);

                var dateTimeLastDay = dateTime.AddDays(-1);

                var lastDayUserNumber =
                    (double)
                        this.Data.Users.All()
                            .Count(
                                user =>
                                    user.RegisteredTime.Month == dateTimeLastDay.Month &&
                                    user.RegisteredTime.Day == dateTimeLastDay.Day);

                double userIncreasingRatio;
                if (Math.Abs(lastDayUserNumber) < 1e-6)
                {
                    userIncreasingRatio = 0;
                }
                else
                {
                    userIncreasingRatio = (double)newUserNumber /
                                          (double)
                                              this.Data.Users.All()
                                                  .Count(
                                                      user =>
                                                          user.RegisteredTime.Month == dateTimeLastDay.Month &&
                                                          user.RegisteredTime.Day == dateTimeLastDay.Day);

                    userIncreasingRatio = (userIncreasingRatio - 1.0) * 100;
                }

                var activeUserNumber = 1;

                var newActivities =
                    this.Data.Activity.All()
                        .Count(
                            activity =>
                                activity.PublishTime.Month == dateTime.Month && activity.PublishTime.Day == dateTime.Day);

                var newPosts =
                    this.Data.Tweets.All()
                        .Count(tweet => tweet.DatePosted.Month == dateTime.Month && tweet.DatePosted.Day == dateTime.Day);

                var newReplies =
                    this.Data.Reply.All()
                        .Count(reply => reply.PublishTime.Month == dateTime.Month && reply.PublishTime.Day == dateTime.Day);

                var newPhotos =
                    this.Data.Photo.All()
                        .Count(photo => photo.DatePosted.Month == dateTime.Month && photo.DatePosted.Day == dateTime.Day);

                var userStatisticsViewModel = new UserStatisticsViewModel
                {
                    Date = dateTime,
                    ActiveUserNumber = activeUserNumber,
                    NewActivities = newActivities,
                    NewPhotos = newPhotos,
                    NewPosts = newPosts,
                    NewReplies = newReplies,
                    NewUserNumber = newUserNumber,
                    UserIncreasingRatio = userIncreasingRatio
                };

                result.Add(userStatisticsViewModel);
            }

            return View(result);
        }
    }
}