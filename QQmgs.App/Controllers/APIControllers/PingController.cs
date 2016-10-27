using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Twitter.App.BusinessLogic;

namespace Twitter.App.Controllers.APIControllers
{
    [AllowAnonymous]
    [RoutePrefix("api/ping")]
    public class PingController : ApiController
    {

        [Route("")]
        [HttpGet]
        public HttpResponseMessage Get()
        {
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [Route("Admin")]
        [HttpGet]
        public HttpResponseMessage CheckAdmin()
        {
            var isAdmin = RoleHelper.IsAdmin();

            return Request.CreateResponse(!isAdmin ? HttpStatusCode.Unauthorized : HttpStatusCode.OK);
        }

        [Route("Admin")]
        [HttpPost]
        public HttpResponseMessage JoinAdmin()
        {
            // Check current user
            var isAuthenticated = User.Identity.IsAuthenticated;
            if (isAuthenticated == false)
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized);
            }

            // Join Admin role
            var isAdmin = RoleHelper.IsAdmin();
            if (isAdmin)
            {
                return Request.CreateResponse(HttpStatusCode.OK);

            }
            else
            {
                RoleHelper.JoinAdmin(User.Identity.GetUserName());
                return Request.CreateResponse(HttpStatusCode.Accepted);
            }
        }
    }
}
