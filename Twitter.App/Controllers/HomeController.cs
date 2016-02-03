namespace Twitter.App.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Mvc;

    using Twitter.Data.UnitOfWork;

    using Microsoft.AspNet.Identity;

    using Twitter.App.Models.BindingModels;
    using Twitter.App.Models.ViewModels;
    using Twitter.Models;
    using PagedList;

    [Authorize]
    [RoutePrefix("home")]
    public class HomeController : BaseController
    {
        public HomeController()
            : base(new TwitterData())
        {
        }

        public ActionResult Index(int p = 1)
        {
            var recentTweets =
                this.Data.Tweets.All()
                    .OrderByDescending(t => t.DatePosted)
                    .Select(
                        t => new TweetViewModel
                        {
                            Id = t.Id,
                            Author = t.Author.UserName,
                            Text = t.Text,
                            UsersFavouriteCount = t.UsersFavourite.Count,
                            RepliesCount = t.Reply.Count,
                            RetweetsCount = t.Retweets.Count,
                            DatePosted = t.DatePosted,
                            ReplyList = t.Reply.ToList()
                        });

            var pagedTweets = recentTweets.ToPagedList(pageNumber: p, pageSize: 6);

            return this.View(pagedTweets);
        }

        public ActionResult TweetsByPage(int page)
        {
            var tweets =
                this.Data.Tweets.All()
                .OrderByDescending(t => t.DatePosted)
                .Select(
                    t => new TweetViewModel
                    {
                        Id = t.Id,
                        Author = t.Author.UserName,
                        Text = t.Text,
                        UsersFavouriteCount = t.UsersFavourite.Count,
                        RepliesCount = t.Reply.Count,
                        RetweetsCount = t.Retweets.Count,
                        DatePosted = t.DatePosted,
                        ReplyList = t.Reply.ToList()
                    })
                .Skip(10 * (page - 1))
                .Take(10);

            return this.PartialView("_Tweets", tweets);
        }

        public ActionResult About()
        {
            return View();
        }
    }
}