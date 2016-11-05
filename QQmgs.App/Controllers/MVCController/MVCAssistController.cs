using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Twitter.App.Ambient;
using Twitter.Data.UnitOfWork;

namespace Twitter.App.Controllers.MVCController
{
    public class MVCAssistController : TwitterBaseController
    {
        public MVCAssistController() : base(new QQmgsData())
        {
        }

        [HttpGet]
        public ActionResult Footer()
        {
            var currentYear = DateTime.Now.Year;
            var currentMonth = DateTime.Now.Month;
            var currentDay = DateTime.Now.Day;

            // Get current assembly version
            var version = AssemblyHelper.GetCurrentVersion();

            // Get total registered user number
            var userNumber = this.Data.Users.All().Count();

            // Get post&reply number today
            var tweetNumber =
                this.Data.Tweets.All()
                    .Count(
                        tweet =>
                            tweet.DatePosted.Year == currentYear && tweet.DatePosted.Month == currentMonth &&
                            tweet.DatePosted.Day == currentDay);

            var replyNumber =
                this.Data.Reply.All()
                    .Count(
                        tweet =>
                            tweet.PublishTime.Year == currentYear && tweet.PublishTime.Month == currentMonth &&
                            tweet.PublishTime.Day == currentDay);

            var postNumber = tweetNumber + replyNumber;

            //var loginCounts =
            //    this.Data.UserLogTrace.All()
            //        .Count(
            //            login =>
            //                login.DatePosted.Year == currentYear && login.DatePosted.Month == currentMonth &&
            //                login.DatePosted.Day == currentDay);

            var display = $"全球某工商 Build V{version} · 共有 {userNumber} 位浙小商 · 今日Post共: {postNumber} 条";

            return PartialView((object)display);
        }
    }
}