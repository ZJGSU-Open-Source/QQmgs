using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Twitter.Data.UnitOfWork;

namespace Twitter.App.Controllers.V2Controllers
{
    public class TwitterApiController : ApiController
    {
        protected TwitterApiController(ITwitterData data)
        {
            this.Data = data;
        }

        protected ITwitterData Data { get; set; }
    }
}