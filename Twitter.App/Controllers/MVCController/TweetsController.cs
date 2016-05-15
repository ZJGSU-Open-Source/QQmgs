using Twitter.App.BusinessLogic;

namespace Twitter.App.Controllers
{
    using Microsoft.AspNet.Identity;
    using PagedList;
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Net;
    using System.Web.Mvc;

    using Twitter.App.Models.BindingModels;
    using Twitter.App.Models.ViewModels;
    using Twitter.Data.UnitOfWork;
    using Twitter.Models;

    using Constants = App.Constants.Constants;

    [Authorize]
    [RoutePrefix("tweets")]
    public class TweetsController : TwitterBaseController
    {
        public TweetsController()
            : base(new TwitterData())
        {
        }

        // GET /tweets/create
        [HttpGet]
        [Route("create")]
        public ActionResult NewTweetForm()
        {
            return this.PartialView("_NewTweetModal");
        }

        // GET /tweets/MyTweets
        [HttpGet]
        [Route("MyTweets")]
        public ActionResult GetTweetBySpecifiedUser(int p = 1)
        {
            var loggedUserId = this.User.Identity.GetUserId();

            var recentTweets = Data.Tweets.All()
                .OrderByDescending(t => t.DatePosted)
                .Where(t => t.Author.Id == loggedUserId)
                .Select(ViewModelsHelper.AsTweetViewModel).ToList();

            var pagedTweets = recentTweets.ToPagedList(pageNumber: p, pageSize: 6);

            return this.View(pagedTweets);
        }

        // GET /tweets/HotTweets
        [HttpGet]
        [Route("HotTweets")]
        public ActionResult GetHotTweets(int p = 1)
        {
            var recentTweets = Data.Tweets.All()
                .OrderByDescending(t => t.UsersFavourite.Count)
                .Select(ViewModelsHelper.AsTweetViewModel)
                .Take(12).ToList();

            var pagedTweets = recentTweets.ToPagedList(pageNumber: p, pageSize: 6);

            return this.View(pagedTweets);
        }

        // GET /tweets?tweetId={tweetId}
        [HttpGet]
        [Route("{tweetId:int}/details")]
        public ActionResult GetTweet(int tweetId)
        {
            var tweet = Data.Tweets.All()
                .OrderByDescending(t => t.DatePosted)
                .Where(t => t.Id == tweetId)
                .Select(ViewModelsHelper.AsTweetViewModel)
                .FirstOrDefault();

            if (tweet == null)
            {
                return HttpNotFound($"Tweet with id {tweetId} not found");
            }

            return this.View(tweet);
        }

        [HttpGet]
        [Route("add")]
        public ActionResult InsertTweet()
        {
            return View();
        }

        [HttpPost]
        [Route("add")]
        public ActionResult InsertTweet(CreateTweetBindingModel model, bool directToHome = false)
        {
            if (!this.ModelState.IsValid)
            {
                this.Response.StatusCode = 400;
                return this.Json(this.ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
            }

            var loggedUserId = this.User.Identity.GetUserId();
            var loggedUserUsername = this.User.Identity.GetUserName();

            model.GroupId = 3; // for test
            var group = Data.Group.Find(model.GroupId);
            if (group == null)
            {
                this.Response.StatusCode = 400;
                return this.Json(this.ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
            }

            var tweet = new Tweet
            {
                Text = model.Text,
                AuthorId = loggedUserId,
                DatePosted = DateTime.Now,
                IsEvent = false,
                GroupId = model.GroupId
            };

            this.Data.Tweets.Add(tweet);
            this.Data.SaveChanges();

            if (directToHome)
            {
                return RedirectToAction("Index", "Home");
            }

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
                    UsersFavouriteCount = tweet.UsersFavourite.Count,
                    GroupId = tweet.GroupId
                });
        }

        [HttpPost]
        [Route("create")]
        public ActionResult CreateTweet(CreateTweetBindingModel model)
        {
            if (!this.ModelState.IsValid)
            {
                this.Response.StatusCode = 400;
                return this.Json(this.ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
            }

            var loggedUserId = this.User.Identity.GetUserId();

            var group = Data.Group.Find(model.GroupId);
            if (group == null)
            {
                this.Response.StatusCode = 400;
                return this.Json(this.ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
            }

            group.LastTweetUpdateTime = DateTime.Now;
            this.Data.Group.Update(group);
            this.Data.SaveChanges();

            var tweet = new Tweet
            {
                Text = model.Text,
                AuthorId = loggedUserId,
                DatePosted = DateTime.Now,
                IsEvent = false,
                GroupId = model.GroupId
            };

            this.Data.Tweets.Add(tweet);
            this.Data.SaveChanges();

            return this.RedirectToAction("Get", "Group", new { groupId = model.GroupId, p = 1 });
        }

        [HttpPost]
        [Route("reply")]
        public ActionResult Reply(ReplyViewModel model)
        {
            if (string.IsNullOrEmpty(model.Text))
            {
                return this.PartialView("_Tweet");
            }

            var tweet = Data.Tweets.Find(model.Id);

            if (tweet == null)
            {
                return this.PartialView("_Tweet");
            }

            var loggedUserId = this.User.Identity.GetUserId();
            var loggedUserUsername = this.User.Identity.GetUserName();

            var reply = new Reply
            {
                AuthorId = loggedUserId,
                Content = model.Text,
                PublishTime = DateTime.Now,
                TweetId = model.Id
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
                    Author = reply.Author.RealName
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
        [Route("{tweetId:int}/edit")]
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
        [Route("{tweetId:int}/edit")]
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

                return RedirectToAction("GetTweet", "Tweets", new { tweetId = tweet.Id });
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