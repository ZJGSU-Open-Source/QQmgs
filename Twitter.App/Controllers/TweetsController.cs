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
                        RepliesCount = t.Replies.Count,
                        RetweetsCount = t.Retweets.Count,
                        DatePosted = t.DatePosted
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
                        RepliesCount = t.Replies.Count,
                        RetweetsCount = t.Retweets.Count,
                        DatePosted = t.DatePosted
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
                        RepliesCount = tweet.Replies.Count,
                        Text = tweet.Text,
                        UsersFavouriteCount = tweet.UsersFavourite.Count
                    });
        }

        //[HttpGet]
        //[Route("reply")]
        //public ActionResult Reply(int tweetId)
        //{
        //    var tweet = Data.Tweets.Find(tweetId);

        //    if (tweet != null)
        //    {
                
        //    }
        //}

        public int Favourite(int tweetId)
        {
            var loggedUserId = this.User.Identity.GetUserId();
            var tweet = this.Data.Tweets.Find(tweetId);

            this.Data.Users.Find(loggedUserId).FavouriteTweets.Add(tweet);

            this.Data.SaveChanges();

            return tweet.UsersFavourite.Count();
        }

        [Route("delete")]
        public bool Delete(int tweetId)
        {
            var loggedUserId = this.User.Identity.GetUserId();
            var tweet = this.Data.Tweets.Find(tweetId);

            var removedTweet = this.Data.Tweets.Remove(tweetId);

            this.Data.SaveChanges();

            return true;
        }
    }
}