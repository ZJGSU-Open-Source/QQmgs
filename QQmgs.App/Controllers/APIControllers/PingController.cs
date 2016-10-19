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
        //[Authorize(Roles = "Admin")]
        [Route("")]
        public string Get()
        {
            var roles = Roles.GetAllRoles();

            var p = Roles.Provider;
            var n = p.ApplicationName;

            //Roles.CreateRole("Admin");
            //Roles.AddUserToRole(User.Identity.Name, "Admin");

            //var users = Roles.GetUsersInRole("Admin");
            //var sss = Roles.GetRolesForUser(User.Identity.Name);

            return $"Hello, QQmgs.";
        }
    }
}
