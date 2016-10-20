using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Security;

namespace Twitter.App.Controllers.APIControllers
{
    [Authorize]
    [RoutePrefix("api/ping")]
    public class PingController : ApiController
    {
        // GET api/ping
        //[Authorize(Roles = "QQmgsAdmin")]
        [Route("")]
        public string Get()
        {
            var s = Constants.Constants.UserRoles.QQmgsAdmin.ToString();
            Roles.CreateRole(s);
            Roles.AddUserToRole("13500000007", s);

            //var s = Roles.IsUserInRole("QQmgs_admin");

            //var users = Roles.GetUsersInRole("Admin");
            //var sss = Roles.GetRolesForUser(User.Identity.Name);

            return $"Hello, QQmgs.";
        }
    }
}
