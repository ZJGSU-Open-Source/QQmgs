using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Twitter.App.BusinessLogic;
using Twitter.App.Common;
using Twitter.App.DataContracts;
using Twitter.App.Models.BindingModel;
using Twitter.App.Models.ViewModels;
using Twitter.App.Provider;
using Twitter.Data.UnitOfWork;
using Twitter.Models;

namespace Twitter.App.Controllers.APIControllers
{
    [RoutePrefix("api/activity")]
    [Authorize]
    public class ActivityController : TwitterApiController
    {
        public ActivityController()
            : base(new QQmgsData())
        {
        }

        [HttpGet]
        [Route("{activityId:int}")]
        public HttpResponseMessage Get([FromUri] int activityId)
        {
            Guard.ArgumentNotNull(activityId, nameof(activityId));

            var activity = Data.Activity.All()
                .Where(t => t.Id == activityId)
                .Select(ViewModelsHelper.AsActivictyViewModel)
                .FirstOrDefault();

            if (activity == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, $"Cannot find activity for activity ID {activityId}");
            }

            // assign avatar image full url
            var urlPrefix = HTTPHelper.GetUrlPrefix();

            activity.CreatorAvatarImage = $"{urlPrefix}/img/Uploads/Thumbnails/{activity.CreatorAvatarImage}";
            foreach (var participation in activity.Participations)
            {
                participation.AvatarImage = $"{urlPrefix}/img/Uploads/Thumbnails/{participation.AvatarImage}";
            }

            return Request.CreateResponse(HttpStatusCode.OK, activity);
        }

        [HttpGet]
        [Route("~/api/queries/activity")]
        public HttpResponseMessage GetAll([FromUri] string classcification = null, [FromUri] int pageNo = DefaultPageNo, [FromUri] int pageSize = DefaultPageSize)
        {
            if (pageNo <= 0 || pageSize <= 0)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "pageNo and pageSize should be both grater than 0.");
            }

            var activities = this.Data.Activity.All()
                .Select(ViewModelsHelper.AsActivictyViewModel)
                .Where(
                    model =>
                        model.Classficiation == classcification && classcification != null || classcification == null)
                .ToList();

            // retrieve creators data
            var creators = activities.Select(activity => this.Data.Users.Find(activity.CreatorId)).ToList();
            var urlPrefix = HTTPHelper.GetUrlPrefix();

            for (var i = 0; i < activities.Count; ++i)
            {
                activities[i].Creator = creators[i].RealName;
                activities[i].CreatorAvatarImage = $"{urlPrefix}/img/Uploads/Thumbnails/{creators[i].AvatarImageName}";
                activities[i].HasCreatorAvatarImage = creators[i].HasAvatarImage;
            }

            var pagedActivities = activities.GetPagedResult(t => t.PublishTime, pageNo, pageSize, SortDirection.Descending);

            return pagedActivities.Count == 0
                ? Request.CreateErrorResponse(HttpStatusCode.NoContent, $"No activity was found under classcification {classcification}")
                : Request.CreateResponse(HttpStatusCode.OK, new PaginationResult<ActivityViewModel>(pagedActivities, pageNo, pageSize, activities.Count));
        }

        [HttpGet]
        [Route("~/api/queries/hotActivity")]
        public HttpResponseMessage GetHotActivities([FromUri] string classcification = null, [FromUri] int pageNo = DefaultPageNo, [FromUri] int pageSize = DefaultPageSize)
        {
            if (pageNo <= 0 || pageSize <= 0)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "pageNo and pageSize should be both grater than 0.");
            }

            var activities = this.Data.Activity.All()
                .Select(ViewModelsHelper.AsActivictyViewModel)
                .Where(
                    model =>
                        model.Classficiation == classcification && classcification != null || classcification == null)
                .Take(2) // TODO: take top 2 activities if available
                .ToList();

            // retrieve creators data
            var creators = activities.Select(activity => this.Data.Users.Find(activity.CreatorId)).ToList();
            var urlPrefix = HTTPHelper.GetUrlPrefix();

            for (var i = 0; i < activities.Count; ++i)
            {
                activities[i].Creator = creators[i].RealName;
                activities[i].CreatorAvatarImage = $"{urlPrefix}/img/Uploads/Thumbnails/{creators[i].AvatarImageName}";
                activities[i].HasCreatorAvatarImage = creators[i].HasAvatarImage;
            }

            var pagedActivities = activities.GetPagedResult(t => t.PublishTime, pageNo, pageSize, SortDirection.Descending);

            return pagedActivities.Count == 0
                ? Request.CreateErrorResponse(HttpStatusCode.NoContent, $"No activity was found under classcification {classcification}")
                : Request.CreateResponse(HttpStatusCode.OK, new PaginationResult<ActivityViewModel>(pagedActivities, pageNo, pageSize, activities.Count));
        }

        [HttpPut]
        [Route("{activityId:int}")]
        public HttpResponseMessage Update([FromUri] int activityId, [FromBody] CreateActivityBindingModel model)
        {
            Guard.ArgumentNotNull(activityId, nameof(activityId));
            Guard.ArgumentNotNullOrEmpty(model.Name, $"{model}.{model.Name}");
            Guard.ArgumentNotNullOrEmpty(model.Description, $"{model}.{model.Description}");
            Guard.ArgumentNotNullOrEmpty(model.Place, $"{model}.{model.Place}");
            Guard.ArgumentNotNullOrEmpty(model.StartTime, $"{model}.{model.StartTime}");
            Guard.ArgumentNotNullOrEmpty(model.EndTime, $"{model}.{model.EndTime}");
            Guard.ArgumentNotNullOrEmpty(model.Classfication, $"{model}.{model.Classfication}");

            var activity = Data.Activity
                .All()
                .FirstOrDefault(t => t.Id == activityId);

            if (activity == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, $"Cannot find activity for activity ID {activityId}");
            }

            // Check ownership
            if (activity.CreatorId != this.User.Identity.GetUserId())
            {
                return Request.CreateErrorResponse(HttpStatusCode.Forbidden, $"Only creator {activity.CreatorId} can update the activity");
            }

            activity.Name = model.Name;
            activity.Description = model.Description;
            activity.Place = model.Place;
            activity.Classficiation = EnumUtils.Parse<ActivityClassficiation>(model.Classfication);
            activity.StartTime = DateTime.Parse(model.StartTime);
            activity.EndTime = DateTime.Parse(model.EndTime);

            this.Data.Activity.Update(activity);
            this.Data.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK, activity);
        }

        [HttpPost]
        [Route("")]
        public HttpResponseMessage Create([FromBody] CreateActivityBindingModel model)
        {
            Guard.ArgumentNotNullOrEmpty(model.Name, $"{model}.{model.Name}");
            Guard.ArgumentNotNullOrEmpty(model.Description, $"{model}.{model.Description}");
            Guard.ArgumentNotNullOrEmpty(model.Place, $"{model}.{model.Place}");
            Guard.ArgumentNotNullOrEmpty(model.StartTime, $"{model}.{model.StartTime}");
            Guard.ArgumentNotNullOrEmpty(model.EndTime, $"{model}.{model.EndTime}");
            Guard.ArgumentNotNullOrEmpty(model.Classfication, $"{model}.{model.Classfication}");

            var activity = new Activity
            {
                CreatorId = this.User.Identity.GetUserId(),
                Name = model.Name,
                Description = model.Description,
                PublishTime = DateTime.Now,
                Classficiation =
                    EnumUtils.Parse<ActivityClassficiation>(model.Classfication ??
                                                            ActivityClassficiation.Other.ToString()),
                Place = model.Place,
                StartTime = DateTime.Parse(model.StartTime),
                EndTime = DateTime.Parse(model.EndTime)
            };

            this.Data.Activity.Add(activity);
            this.Data.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK, activity);
        }

        [HttpDelete]
        [Route("{activityId:int}")]
        public HttpResponseMessage Delete([FromUri] int activityId)
        {
            Guard.ArgumentNotNull(activityId, nameof(activityId));

            var activity = Data.Activity
                .All()
                .FirstOrDefault(t => t.Id == activityId);

            if (activity == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, $"Cannot find activity for activity ID {activityId}");
            }

            // Check ownership
            if (activity.CreatorId != this.User.Identity.GetUserId())
            {
                return Request.CreateErrorResponse(HttpStatusCode.Forbidden, $"Only creator {activity.CreatorId} can update the activity");
            }

            this.Data.Activity.Remove(activity);
            this.Data.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK, activity);
        }

        [HttpPost]
        [Route("{activityId:int}/activityImage")]
        public async Task<HttpResponseMessage> ActivityImage([FromUri] int activityId)
        {
            Guard.ArgumentNotNull(activityId, nameof(activityId));

            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            var activity = Data.Activity
                .All()
                .FirstOrDefault(t => t.Id == activityId);

            if (activity == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, $"Cannot find activity for activity ID {activityId}");
            }

            // Check ownership
            var loggedUserId = this.User.Identity.GetUserId();
            if (activity.CreatorId != loggedUserId)
            {
                return Request.CreateErrorResponse(HttpStatusCode.Forbidden, $"Only creator {activity.CreatorId} can update the activity");
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
                        PhotoType = PhotoType.ActivityImage,
                        PhotoClasscification = PhotoClasscification.Ohter,
                        Descrption = string.Empty,
                        IsSoftDelete = false,
                        OriginalHeight = uploadedFile.Height,
                        OriginalWidth = uploadedFile.Width
                    };

                    this.Data.Photo.Add(photo);
                    this.Data.SaveChanges();

                    activity.ActivityImage = fileName;
                    this.Data.Activity.Update(activity);
                    this.Data.SaveChanges();
                }

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        [HttpPost]
        [Route("{activityId:int}/Join")]
        public HttpResponseMessage JoinActivity([FromUri] int activityId)
        {
            Guard.ArgumentNotNull(activityId, nameof(activityId));

            var activity = Data.Activity
                .All()
                .FirstOrDefault(t => t.Id == activityId);

            if (activity == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, $"Cannot find activity for activity ID {activityId}");
            }

            var loggedUserId = this.User.Identity.GetUserId();
            var user = this.Data.Users.Find(loggedUserId);

            if (activity.Participents.Contains(user))
            {
                return Request.CreateResponse(HttpStatusCode.Created,
                    $"User {loggedUserId} already joined the activity {activityId}");
            }

            activity.Participents.Add(user);
            this.Data.Activity.Update(activity);
            this.Data.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpDelete]
        [Route("{activityId:int}/Join")]
        public HttpResponseMessage LeaveActivity([FromUri] int activityId)
        {
            Guard.ArgumentNotNull(activityId, nameof(activityId));

            var activity = Data.Activity
                .All()
                .FirstOrDefault(t => t.Id == activityId);

            if (activity == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, $"Cannot find activity for activity ID {activityId}");
            }

            var loggedUserId = this.User.Identity.GetUserId();
            var user = this.Data.Users.Find(loggedUserId);

            if (!activity.Participents.Contains(user))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest,
                    $"User {loggedUserId} haven't joined the activity {activityId}");
            }

            activity.Participents.Remove(user);
            this.Data.Activity.Update(activity);
            this.Data.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}