﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using Twitter.App.BusinessLogic;
using Twitter.App.DataContracts;
using Twitter.App.Models.ViewModels;
using Twitter.Data.UnitOfWork;

namespace Twitter.App.Controllers.APIControllers
{
    [RoutePrefix("api/group")]
    public class PhotoController : TwitterApiController
    {
        public PhotoController()
            : base(new TwitterData())
        {

        }

        [HttpGet]
        [Route("~/api/queries/photo")]
        public HttpResponseMessage GetAll([FromUri] int pageNo = DefaultPageNo, [FromUri] int pageSize = LargetDefaultPageSize)
        {
            if (pageNo <= 0 || pageSize <= 0)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "pageNo and pageSize should be both grater than 0.");
            }

            var recentPhotos = this.Data.Photo.All()
                .OrderByDescending(t => t.DatePosted)
                .Where(photo => photo.OriginalWidth != 0 && photo.OriginalHeight != 0 && !photo.IsSoftDelete)
                .Select(ViewModelsHelper.AsPhotoViewModel).ToList();

            var pagedPhotos = recentPhotos.GetPagedResult(t => t.DatePosted, pageNo, pageSize, SortDirection.Descending);

            return Request.CreateResponse(HttpStatusCode.OK, new PaginationResult<PhotoViewModel>(pagedPhotos, pageNo, pageSize, recentPhotos.Count));
        }
    }
}