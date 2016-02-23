using System.Data.Entity.Validation;

namespace Twitter.App.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using System.Net;

    using Twitter.Data.UnitOfWork;

    using Microsoft.AspNet.Identity;

    using Twitter.App.Models.BindingModels;
    using Twitter.App.Models.ViewModels;
    using Twitter.Models;
    using PagedList;

    [Authorize]
    [RoutePrefix("tweets")]
    public class TweetsController : BaseController
    {
        public TweetsController()
            : base(new TwitterData())
        {
        }

        [HttpGet]
        [Route("create")]
        public ActionResult NewTweetForm()
        {
            return this.PartialView("_NewTweetModal");
        }

        [HttpGet]
        [Route("get")]
        public ActionResult GetTweetBySpecifiedUser(int p = 1)
        {
            var loggedUserId = this.User.Identity.GetUserId();

            var recentTweets = Data.Tweets.All()
                .OrderByDescending(t => t.DatePosted)
                .Where(t => t.Author.Id == loggedUserId)
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
                .ToList();

            var pagedTweets = recentTweets.ToPagedList(pageNumber: p, pageSize: 6);

            return this.View(pagedTweets);
        }

        [HttpGet]
        [Route("HotTweets")]
        public ActionResult GetHotTweets(int p = 1)
        {
            var loggedUserId = this.User.Identity.GetUserId();

            var recentTweets = Data.Tweets.All()
                .OrderByDescending(t => t.UsersFavourite.Count)
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
                .Take(12)
                .ToList();

            var pagedTweets = recentTweets.ToPagedList(pageNumber: p, pageSize: 6);

            return this.View(pagedTweets);
        }

        [HttpPost]
        [Route("add")]
        public ActionResult InsertTweet(CreateTweetBindingModel model)
        {
            if (!this.ModelState.IsValid)
            {
                this.Response.StatusCode = 400;
                return this.Json(this.ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
            }

            var loggedUserId = this.User.Identity.GetUserId();
            var loggedUserUsername = this.User.Identity.GetUserName();

            var tweet = new Tweet { Text = model.Text, AuthorId = loggedUserId, DatePosted = DateTime.Now };

            this.Data.Tweets.Add(tweet);

            this.Data.SaveChanges();

            return this.PartialView(
                "_Tweet",
                new TweetViewModel
                    {
                        Id = tweet.Id,
                        Author = loggedUserUsername,
                        DatePosted = tweet.DatePosted,
                        RetweetsCount = tweet.Retweets.Count,
                        RepliesCount = tweet.Reply.Count,
                        Text = tweet.Text,
                        UsersFavouriteCount = tweet.UsersFavourite.Count
                    });
        }

        [HttpPost]
        [Route("reply")]
        public ActionResult Reply(string content, int tweetId)
        {
            if (string.IsNullOrEmpty(content))
            {
                return this.PartialView("_Tweet");
            }

            var tweet = Data.Tweets.Find(tweetId);

            if (tweet == null)
            {
                return this.PartialView("_Tweet");
            }

            var loggedUserId = this.User.Identity.GetUserId();
            var loggedUserUsername = this.User.Identity.GetUserName();

            var reply = new Reply()
            {
                AuthorId = loggedUserId,
                AuthorName = loggedUserUsername,
                Content = content,
                PublishTime = DateTime.Now,
                TweetId = tweetId
            };

            try
            {
                this.Data.Reply.Add(reply);
                this.Data.SaveChanges();
            }
            catch (Exception)
            {
                return PartialView(
                    "Reply",
                    new ReplyViewModel
                    {
                        Text = "Reply should not be empty or less than 250 words."
                    });
            }

            return PartialView(
                "Reply",
                new ReplyViewModel
                {
                    PublishTime = reply.PublishTime,
                    Text = reply.Content,
                    Author = loggedUserUsername
                });
        }

        public int Favourite(int tweetId)
        {
            var loggedUserId = this.User.Identity.GetUserId();
            var tweet = this.Data.Tweets.Find(tweetId);

            if (Data.Users.Find(loggedUserId).FavouriteTweets.Contains(tweet))
            {
                this.Data.Users.Find(loggedUserId).FavouriteTweets.Remove(tweet);
                this.Data.SaveChanges();
            }
            else
            {
                this.Data.Users.Find(loggedUserId).FavouriteTweets.Add(tweet);
                this.Data.SaveChanges();
            }

            return tweet.UsersFavourite.Count;
        }

        [HttpGet]
        [Route("edit")]
        public ActionResult Edit(int? tweetId)
        {
            if (tweetId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Tweet tweet = Data.Tweets.Find(tweetId);
            if (tweet == null)
            {
                return HttpNotFound();
            }

            if (tweet.AuthorId != User.Identity.GetUserId())
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return View(tweet);
        }

        [HttpPost]
        [Route("edit")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Tweet tweet)
        {
            if (ModelState.IsValid)
            {
                var loggedUserId = this.User.Identity.GetUserId();

                tweet.AuthorId = loggedUserId;
                tweet.DatePosted = DateTime.Now;

                Data.Tweets.Update(tweet);
                Data.SaveChanges();

                return RedirectToAction("Index", "Home");
            }

            return View(tweet);
        }

        [Route("delete")]
        public string Delete(int tweetId)
        {
            var loggedUserId = this.User.Identity.GetUserId();
            var tweet = this.Data.Tweets.Find(tweetId);

            var removedTweet = this.Data.Tweets.Remove(tweetId);

            this.Data.SaveChanges();

            return "Delete Successfully";
        }
    }
}