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
            : base(new QQmgsData())
        {
        }

        [HttpGet]
        [Route("~/api/queries/Tweets")]
        public HttpResponseMessage GetAll([FromUri] int pageNo = DefaultPageNo, [FromUri] int pageSize = DefaultPageSize)
        {
            if (pageNo <= 0 || pageSize <= 0)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "pageNo and pageSize should be both grater than 0.");
            }

            var recentTweets = this.Data.Tweets.All()
                .OrderByDescending(t => t.DatePosted)
                .Select(ViewModelsHelper.AsTweetViewModel).ToList();

            var pagedTweets = recentTweets.GetPagedResult(t => t.Id, pageNo, pageSize, SortDirection.Descending);

            return Request.CreateResponse(HttpStatusCode.OK, new PaginationResult<TweetViewModel>(pagedTweets, pageNo, pageSize, recentTweets.Count));
        }

        [Route("{tweetId}")]
        [HttpGet]
        public HttpResponseMessage Get([FromUri] int tweetId)
        {
            var tweet = Data.Tweets.All()
                .Where(t => t.Id == tweetId)
                .Select(ViewModelsHelper.AsTweetViewModel)
                .FirstOrDefault();

            return tweet == null
                ? Request.CreateResponse(HttpStatusCode.NotFound, $"Cannot find tweet for tweet ID {tweetId}")
                : Request.CreateResponse(HttpStatusCode.OK, tweet);
        }

    }
}