using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using Glimpse.Core.Extensibility;
using Twitter.App.Common;

namespace Twitter.App.BusinessLogic
{
    public static class Base64Coding
    {
        public static string Encrption(string uid1, string uid2)
        {
            Guard.ArgumentNotNullOrEmpty(uid1, nameof(uid1));
            Guard.ArgumentNotNullOrEmpty(uid2, nameof(uid2));

            var stringValue = uid1 + "_" + uid2;
            var bytes = Encoding.UTF8.GetBytes(stringValue);
            var cypherString = Convert.ToBase64String(bytes);

            return cypherString;
        }

        public static string Encrption(string uid)
        {
            Guard.ArgumentNotNullOrEmpty(uid, nameof(uid));
        
            var stringValue = uid;
            var bytes = Encoding.UTF8.GetBytes(stringValue);
            var cypherString = Convert.ToBase64String(bytes);

            return cypherString;
        }

        public static string Decrption(string cid)
        {
            Guard.ArgumentNotNullOrEmpty(cid, nameof(cid));

            var bytes = Convert.FromBase64String(cid);
            var users = Encoding.UTF8.GetString(bytes);

            return users;
        }
    }
}