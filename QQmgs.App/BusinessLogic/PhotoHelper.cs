using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Twitter.App.BusinessLogic
{
    public static class PhotoHelper
    {
        public static string RetrievePhotoThumnails(this string fileName, bool isExisted = true)
        {
            return (isExisted && fileName != null)
                ? $"{HTTPHelper.GetUrlPrefix()}/{Constants.Constants.ImageThumbnailsPrefix}/{fileName}"
                : string.Empty;
        }
    }
}