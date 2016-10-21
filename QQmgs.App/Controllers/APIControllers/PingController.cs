using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Security;

namespace Twitter.App.Controllers.APIControllers
{
    [RoutePrefix("api/ping")]
    public class PingController : ApiController
    {
        // GET api/ping
        [Route("")]
        [HttpGet]
        public HttpResponseMessage Get()
        {
            if (!User.IsInRole("QQmgsAdmin"))
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized);
            }

            //var s = Constants.Constants.UserRoles.QQmgsAdmin.ToString();
            //Roles.CreateRole(s);
            //Roles.AddUserToRole(User.Identity.Name, "QQmgsAdmin");

            //var users = Roles.GetUsersInRole("Admin");
            //var sss = Roles.GetRolesForUser(User.Identity.Name);

            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
