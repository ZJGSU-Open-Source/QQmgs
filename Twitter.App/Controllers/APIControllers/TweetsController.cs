using System;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Web.Helpers;
using System.Web.Http;
using Twitter.App.BusinessLogic;
using Twitter.App.DataContracts;
using Twitter.App.Models.ViewModels;
using Twitter.Data.UnitOfWork;
using Twitter.Models;

namespace Twitter.App.Controllers.APIControllers
{
    [RoutePrefix("api/tweets")]
    public class TweetsController : TwitterApiController
    {
        public TweetsController()
            : base(new TwitterData())
        {
        }

        [HttpGet]
        [Route("~/api/Queries/Tweets")]
        public HttpResponseMessage GetAll([FromUri] int pageNo = DefaultPageNo, [FromUri] int pageSize = DefaultPageSize)
        {
            if (pageNo <= 0 || pageSize <= 0)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "pageNo and pageSize should be both grater than 0.");
            }

            var recentTweets = this.Data.Tweets.All()
                .OrderByDescending(t => t.DatePosted)
                .Select(AsTweetViewModel).ToList();

            var pagedTweets = recentTweets.GetPagedResult(t => t.Id, pageNo, pageSize, SortDirection.Descending);

            return Request.CreateResponse(HttpStatusCode.OK, new PaginationResult<TweetViewModel>(pagedTweets, pageNo, pageSize, recentTweets.Count));
        }

        [Route("{tweetId}")]
        [HttpGet]
        public HttpResponseMessage Get([FromUri] int tweetId)
        {
            var tweet = Data.Tweets.All()
                .OrderByDescending(t => t.DatePosted)
                .Where(t => t.Id == tweetId)
                .Select(AsTweetViewModel)
                .FirstOrDefault();

            return tweet == null
                ? Request.CreateResponse(HttpStatusCode.NotFound, $"Cannot find tweet for tweet ID {tweetId}")
                : Request.CreateResponse(HttpStatusCode.OK, tweet);
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