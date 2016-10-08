using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Twitter.App.BusinessLogic;
using Twitter.App.Common;
using Twitter.App.DataContracts;
using Twitter.App.Models.BindingModels;
using Twitter.App.Models.ViewModels;
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

            // retrieve creator data
            var creator = this.Data.Users.Find(activity.CreatorId);
            var urlPrefix = HTTPHelper.GetUrlPrefix();

            activity.Creator = creator.RealName;
            activity.CreatorAvatarImage = $"{urlPrefix}/img/Uploads/Thumbnails/{creator.AvatarImageName}";
            activity.HasCreatorAvatarImage = creator.HasAvatarImage;

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
                ? Request.CreateErrorResponse(HttpStatusCode.NotFound, $"Cannot find activities with classcification {classcification}")
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
                ? Request.CreateErrorResponse(HttpStatusCode.NotFound, $"Cannot find activities with classcification {classcification}")
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
    }
}