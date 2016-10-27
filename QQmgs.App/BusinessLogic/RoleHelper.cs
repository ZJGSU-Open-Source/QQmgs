using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using Twitter.App.Common;

namespace Twitter.App.BusinessLogic
{
    public static class RoleHelper
    {
        private static readonly string AdminRole = Constants.Constants.UserRoles.QQmgsAdmin.ToString();

        public static bool IsAdmin()
        {
            bool result;
            try
            {
                result = Roles.IsUserInRole(AdminRole);
            }
            catch (Exception)
            {
                return false;
            }

            return result;
        }

        public static bool JoinAdmin(string userName)
        {
            Guard.ArgumentNotNullOrEmpty(userName, nameof(userName));

            try
            {
                // Create Admin role if not existed
                if (!Roles.RoleExists(AdminRole))
                {
                    Roles.CreateRole(AdminRole);
                }

                Roles.AddUserToRole(userName, AdminRole);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}