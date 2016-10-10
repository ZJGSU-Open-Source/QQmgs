using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Twitter.App.BusinessLogic;
using Twitter.App.Common;
using Twitter.App.Models.ViewModels;
using Twitter.Data.UnitOfWork;
using Twitter.Models;

namespace Twitter.App.Controllers.APIControllers
{
    [RoutePrefix("api/user")]
    [Authorize]
    public class UserController : TwitterApiController
    {
        public UserController()
            : base(new QQmgsData())
        {
        }

        [HttpGet]
        [Route("me")]
        public HttpResponseMessage Me()
        {
            var userId = this.User.Identity.GetUserId();
            var user = this.Data.Users.Find(userId);

            if (user == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, $"User with ID {userId} is not found");
            }

            var userViewModel = user.ToUserViewModel();

            return Request.CreateResponse(HttpStatusCode.OK, userViewModel);
        }

        [HttpGet]
        [Route("{userId:guid}")]
        public HttpResponseMessage Who([FromUri] Guid userId)
        {
            var user = this.Data.Users.Find(userId.ToString());

            if (user == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, $"User with ID {userId} is not found");
            }

            var userViewModel = user.ToUserViewModel();

            return Request.CreateResponse(HttpStatusCode.OK, userViewModel);
        }

        [HttpGet]
        [Route("{userId:guid}/tweet")]
        public HttpResponseMessage UserPostedTweet([FromUri] Guid userId)
        {
            var user = this.Data.Users.Find(userId.ToString());

            if (user == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, $"User with ID {userId} is not found");
            }

            var userViewModel = user.ToUserPostedTweetViewModel();

            return Request.CreateResponse(HttpStatusCode.OK, userViewModel);
        }

        [HttpGet]
        [Route("{userId:guid}/photo")]
        public HttpResponseMessage UserPostedPhoto([FromUri] Guid userId)
        {
            var user = this.Data.Users.Find(userId.ToString());

            if (user == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, $"User with ID {userId} is not found");
            }

            var userViewModel = user.ToUserPostedPhotosViewModel();

            return Request.CreateResponse(HttpStatusCode.OK, userViewModel);
        }

        [HttpGet]
        [Route("{userId:guid}/group")]
        public HttpResponseMessage UserJoinedGroup([FromUri] Guid userId)
        {
            var user = this.Data.Users.Find(userId.ToString());

            if (user == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, $"User with ID {userId} is not found");
            }

            var userViewModel = user.ToUserJoinedGroupsViewModel();

            return Request.CreateResponse(HttpStatusCode.OK, userViewModel);
        }

        [HttpGet]
        [Route("{userId:guid}/activity")]
        public HttpResponseMessage UserJoinedActivity([FromUri] Guid userId)
        {
            var user = this.Data.Users.Find(userId.ToString());

            if (user == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, $"User with ID {userId} is not found");
            }

            var userViewModel = user.ToUserJoinedActivitiesViewModel();

            return Request.CreateResponse(HttpStatusCode.OK, userViewModel);
        }

        [HttpGet]
        [Route("{userId:guid}/bio")]
        public HttpResponseMessage Bio([FromUri] Guid userId)
        {
            var user = this.Data.Users.Find(userId.ToString());

            if (user == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, $"User with ID {userId} is not found");
            }

            var userViewModel = user.ToUserBioViewModel();

            return Request.CreateResponse(HttpStatusCode.OK, userViewModel);
        }
    }
}
