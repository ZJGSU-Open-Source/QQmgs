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
                            RepliesCount = t.Replies.Count,
                            RetweetsCount = t.Retweets.Count,
                            DatePosted = t.DatePosted
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
                        RepliesCount = t.Replies.Count,
                        RetweetsCount = t.Retweets.Count,
                        DatePosted = t.DatePosted
                    })
                .Skip(10 * (page - 1))
                .Take(10);

            return this.PartialView("_Tweets", tweets);
        }
    }
}