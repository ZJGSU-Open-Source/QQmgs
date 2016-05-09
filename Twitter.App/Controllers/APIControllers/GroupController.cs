using System;
using System.Collections.Generic;
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
    [RoutePrefix("api/group")]
    public class GroupController : TwitterApiController
    {
        public GroupController()
            : base(new TwitterData())
        {
            
        }

        [HttpGet]
        [Route("~/api/queries/group")]
        public HttpResponseMessage GetAll([FromUri] int pageNo = DefaultPageNo, [FromUri] int pageSize = DefaultPageSize)
        {
            if (pageNo <= 0 || pageSize <= 0)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "pageNo and pageSize should be both grater than 0.");
            }

            var recentTweets = this.Data.Group.All()
                .OrderByDescending(t => t.CreatedTime)
                .Select(ViewModelsHelper.AsGroupViewModel).ToList();

            var pagedGroups = recentTweets.GetPagedResult(t => t.Id, pageNo, pageSize, SortDirection.Descending);

            return Request.CreateResponse(HttpStatusCode.OK, new PaginationResult<GroupVieModels>(pagedGroups, pageNo, pageSize, recentTweets.Count));
        }

        [HttpGet]
        [Route("{groupId:int}/details")]
        public HttpResponseMessage Get([FromUri] int groupId, [FromUri] int pageNo = DefaultPageNo, [FromUri] int pageSize = DefaultPageSize)
        {
            var group = Data.Group.Find(groupId);
            if (group == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, $"Group with id {groupId} not found.");
            }

            var tweets = group.Tweets;
            if (tweets == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, $"There's no tweet in the group with id {groupId}.");
            }

            var tweetsViewModel = tweets.Select(t => new TweetViewModel
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
                HasAvatarImage = t.Author.HasAvatarImage,
                AvatarImageName = t.Author.HasAvatarImage ? new Uri(new Uri(Constants.Constants.WebHostPrefix + Constants.Constants.ImageThumbnailsPrefix) + t.Author.AvatarImageName).ToString() : null,
                ReplyList = t.Reply.Select(reply => new ReplyViewModel
                {
                    Text = reply.Content,
                    Id = reply.Id,
                    PublishTime = reply.PublishTime,
                    Author = reply.Author.RealName
                }).ToList()
            }).OrderByDescending(t => t.DatePosted).GetPagedResult(t => t.Id, pageNo, pageSize, SortDirection.Descending);

            return Request.CreateResponse(HttpStatusCode.OK, new PaginationResult<TweetViewModel>(tweetsViewModel, pageNo, pageSize, tweetsViewModel.Count));
        }
    }
}
