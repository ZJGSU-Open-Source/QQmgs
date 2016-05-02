using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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
            : base(new TwitterData())
        {
        }

        public ActionResult Index(int p = 1)
        {
            return RedirectToAction("Index", "Group");

            var recentTweets =
                this.Data.Tweets.All()
                    .OrderByDescending(t => t.DatePosted)
                    .Select(AsTweetViewModel);

            var pagedTweets = recentTweets.ToPagedList(pageNumber: p, pageSize: Constants.Constants.PageTweetsNumber);

            return this.View(pagedTweets);
        }

        [AllowAnonymous]
        public ActionResult About()
        {
            return View();
        }

        private static readonly Expression<Func<Tweet, TweetViewModel>> AsTweetViewModel =
            t => new TweetViewModel
            {
                Id = t.Id,
                Author = t.Author.UserName,
                AuthorStatus = t.Author.Status,
                IsEvent = t.IsEvent,
                Text = t.Text,
                UsersFavouriteCount = t.UsersFavourite.Count,
                RepliesCount = t.Reply.Count,
                RetweetsCount = t.Retweets.Count,
                DatePosted = t.DatePosted,
                GroupId = t.GroupId,
                ReplyList = t.Reply.Select(reply => new ReplyViewModel
                {
                    Text = reply.Content,
                    Id = reply.Id,
                    PublishTime = reply.PublishTime,
                    Author = reply.AuthorName
                }).ToList()
            };
    }
}