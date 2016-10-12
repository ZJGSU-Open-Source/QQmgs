using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Twitter.App.BusinessLogic
{
    public static class HTTPHelper
    {
        public static string GetUrlPrefix()
        {
            var url =  $"{HttpContext.Current.Request.Url.Scheme}://{HttpContext.Current.Request.Url.Host}";

            if (HttpContext.Current.Request.Url.Host.Contains("localhost"))
            {
                url += "/QQmgs.App";
            }

            return url;
        }
    }
}