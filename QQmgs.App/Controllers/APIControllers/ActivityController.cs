using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using Twitter.App.BusinessLogic;
using Twitter.App.Common;
using Twitter.App.DataContracts;
using Twitter.App.Models.ViewModels;
using Twitter.Data.UnitOfWork;
using Twitter.Models;

namespace Twitter.App.Controllers.APIControllers
{
    [RoutePrefix("api/activity")]
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

            activity.Creator = creator.RealName;
            activity.CreatorAvatarImage = $"{HttpContext.Current.Request.Url.Host}/img/Uploads/Thumbnails/{creator.AvatarImageName}";
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

            for (var i = 0; i < activities.Count; ++i)
            {
                activities[i].Creator = creators[i].RealName;
                activities[i].CreatorAvatarImage = $"{HttpContext.Current.Request.Url.Host}/img/Uploads/Thumbnails/{creators[i].AvatarImageName}";
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

            for (var i = 0; i < activities.Count; ++i)
            {
                activities[i].Creator = creators[i].RealName;
                activities[i].CreatorAvatarImage = $"{HttpContext.Current.Request.Url.Host}/img/Uploads/Thumbnails/{creators[i].AvatarImageName}";
                activities[i].HasCreatorAvatarImage = creators[i].HasAvatarImage;
            }

            var pagedActivities = activities.GetPagedResult(t => t.PublishTime, pageNo, pageSize, SortDirection.Descending);

            return pagedActivities.Count == 0
                ? Request.CreateErrorResponse(HttpStatusCode.NotFound, $"Cannot find activities with classcification {classcification}")
                : Request.CreateResponse(HttpStatusCode.OK, new PaginationResult<ActivityViewModel>(pagedActivities, pageNo, pageSize, activities.Count));
        }
    }
}