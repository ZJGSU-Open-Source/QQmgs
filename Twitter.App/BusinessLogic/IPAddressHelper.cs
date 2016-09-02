using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Twitter.App.BusinessLogic
{
    public static class IPAddressHelper
    {
        public static string GetIpAddress()
        {
            var context = System.Web.HttpContext.Current;
            var ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipAddress))
            {
                var addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                {
                    return addresses[0];
                }
            }

            return context.Request.ServerVariables["REMOTE_ADDR"];
        }
    }
}