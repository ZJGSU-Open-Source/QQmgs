using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Twitter.App.BusinessLogic;
using Twitter.App.EmotionAnalysis;

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
            //var res =
            //    EmotionAnalysis.EmotionAnalysis.Go(
            //        "http://www.qqmgs.com/img/Uploads/Thumbnails/cf24872d-5854-479e-b629-31e84e84d0fb_IMG_20160903_135159.jpg");

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
