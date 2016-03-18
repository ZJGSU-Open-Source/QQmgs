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
            var recentTweets =
                this.Data.Tweets.All()
                    .OrderByDescending(t => t.DatePosted)
                    .Select(
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
                            ReplyList = t.Reply.ToList()
                        });

            var pagedTweets = recentTweets.ToPagedList(pageNumber: p, pageSize: Constants.Constants.PageTweetsNumber);

            return this.View(pagedTweets);
        }

        public ActionResult About()
        {
            return View();
        }
    }
}