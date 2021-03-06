﻿using System.Web.Http;
using Twitter.Data.UnitOfWork;

namespace Twitter.App.Controllers.APIControllers
{
    public class TwitterApiController : ApiController
    {
        protected TwitterApiController(IQQmgsData data)
        {
            this.Data = data;
        }

        protected IQQmgsData Data { get; set; }

        protected const int DefaultPageNo = 1;

        protected const int DefaultPageSize = 12;

        protected const int LargetDefaultPageSize = 120;
    }
}