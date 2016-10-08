using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Twitter.App.Controllers.APIControllers
{
    [RoutePrefix("api/ping")]
    [AllowAnonymous]
    public class PingController : ApiController
    {
        // GET api/ping
        [Route("")]
        public string Get()
        {
            return $"Hello, QQmgs.";
        }
    }
}
