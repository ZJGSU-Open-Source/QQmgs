using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Twitter.App.BusinessLogic;
using Twitter.App.Common;
using Twitter.App.Models.BindingModel.WebApiModel;
using Twitter.App.Models.BindingModels.WebApiModels;
using Twitter.App.Models.ViewModels;
using Twitter.App.Provider;
using Twitter.Data.UnitOfWork;
using Twitter.Models;
using Twitter.App.Constants;

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
        [Route("{userId:guid}/createdGroup")]
        public HttpResponseMessage UserCreatedGroup()
        {
            var userId = this.User.Identity.GetUserId();
            var user = this.Data.Users.Find(userId);

            if (user == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, $"User with ID {userId} is not found");
            }

            var userViewModel = user.ToUserCreatedGroupsViewModel();

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

        [HttpPut]
        [Route("status")]
        public HttpResponseMessage UpdateStatus([FromBody] UpdateUserStatusBindingModel model)
        {
            Guard.ArgumentNotNull(model.Status, $"{model}.{model.Status}");

            var userId = this.User.Identity.GetUserId();
            var user = this.Data.Users.Find(userId);

            if (user == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, $"User with ID {userId} is not found");
            }

            user.Status = model.Status;
            this.Data.Users.Update(user);
            this.Data.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK, model);
        }

        [HttpPost]
        [Route("avatarImage")]
        public async Task<HttpResponseMessage> AvatarImage()
        {
            var loggedUserId = this.User.Identity.GetUserId();

            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            var root = HttpContext.Current.Server.MapPath(Constants.Constants.PhotoLocation);
            var provider = new CustomMultipartFormDataStreamProvider(root);

            try
            {
                // Read the form data.
                await Request.Content.ReadAsMultipartAsync(provider);

                // This illustrates how to get the file names.
                foreach (var file in provider.FileData)
                {
                    var fileName = file.LocalFileName.Split(Path.DirectorySeparatorChar).Last();

                    var uploadedFile = FileUploadHelper.ResizeImage(Constants.Constants.DefaultResizeSize, Constants.Constants.DefaultResizeSize, file.LocalFileName, fileName, PhotoType.AvatarImage);

                    var photo = new Photo
                    {
                        AuthorId = loggedUserId,
                        DatePosted = DateTime.Now,
                        Name = fileName,
                        PhotoType = PhotoType.AvatarImage,
                        PhotoClasscification = PhotoClasscification.Ohter,
                        Descrption = string.Empty,
                        IsSoftDelete = false,
                        OriginalHeight = uploadedFile.Height,
                        OriginalWidth = uploadedFile.Width
                    };

                    this.Data.Photo.Add(photo);
                    this.Data.SaveChanges();

                    var user = this.Data.Users.Find(loggedUserId);
                    user.AvatarImageName = fileName;
                    user.HasAvatarImage = true;
                    this.Data.Users.Update(user);
                    this.Data.SaveChanges();
                }

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }
    }
}
