using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using Twitter.App.Models.ViewModels;
using Twitter.Data.UnitOfWork;
using Twitter.Models;

namespace Twitter.App.Controllers.V2Controllers
{
    [RoutePrefix("v2tweets")]
    public class V2TweetsController : TwitterApiController
    {
        public V2TweetsController()
            : base(new TwitterData())
        {
        }

        [HttpGet]
        [Route("index")]
        public HttpResponseMessage GetHotTweets()
        {
            var recentTweets =
                this.Data.Tweets.All()
                    .OrderByDescending(t => t.DatePosted)
                    .Select(AsTweetViewModel).ToList();

            //const string testJsonString = "{ 'firstname' : 'Jason', 'lastname' : 'Voorhees' }";
            //JToken json = JObject.Parse(testJsonString);

            return Request.CreateResponse(HttpStatusCode.OK, recentTweets);
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
                ReplyList = t.Reply.Select(reply => new ReplyViewModel
                {
                    Text = reply.Content,
                    Id = reply.Id,
                    PublishTime = reply.PublishTime,
                    Author = t.Author.UserName
                }).ToList()
            };
    }
}