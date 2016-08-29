using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Web;
using Glimpse.Core.Extensibility;
using Twitter.Models;

namespace Twitter.App.BusinessLogic
{
    public static class FileUploadHelper
    {
        public static char DirSeparator = Path.DirectorySeparatorChar;

        public static string FilesPath =
            HttpContext.Current.Server.MapPath("~\\img" + DirSeparator + "Uploads" + DirSeparator);

        public static int DefaultResizeSize { get; } = 200;

        public static int DefaultResizeRatio { get; } = 2;

        public static Photo UploadFile(HttpPostedFileBase file, PhotoType photoType = PhotoType.AvatarImage)
        {
            var photo = new Photo();

            // Check if we have a file
            if (null == file) return photo;
            // Make sure the file has content
            if (!(file.ContentLength > 0)) return photo;

            // escapse invalid charaters and %
            var encodedFileName = CleanFileName(file.FileName);

            string fileName = $"{Guid.NewGuid()}_{encodedFileName}";

            var fileExt = Path.GetExtension(file.FileName);

            // Issues happen when chinese charcters
            //fileName = Base64Coding.Encrption(fileName);
            //fileName += fileExt;

            // Make sure we were able to determine a proper extension
            if (null == fileExt) return photo;

            // Check if the directory we are saving to exists
            if (!Directory.Exists(FilesPath))
            {
                // If it doesn't exist, create the directory
                Directory.CreateDirectory(FilesPath);
            }

            // Set our full path for saving
            string path = FilesPath + DirSeparator + fileName;

            // Save our file
            file.SaveAs(Path.GetFullPath(path));

            // Save our thumbnail as well
            var size = ResizeImage(file, fileName, DefaultResizeSize, DefaultResizeSize, photoType);

            photo.Url = fileName;
            photo.PhotoSize = size;

            return photo;
        }

        public static void DeleteFile(string fileName)
        {
            // Don't do anything if there is no name
            if (fileName.Length == 0) return;

            // Set our full path for deleting
            string path = FilesPath + DirSeparator + fileName;
            string thumbPath = FilesPath + DirSeparator + "Thumbnails" + DirSeparator + fileName;

            RemoveFile(path);
            RemoveFile(thumbPath);
        }

        private static void RemoveFile(string path)
        {
            // Check if our file exists
            if (File.Exists(Path.GetFullPath(path)))
            {
                // Delete our file
                File.Delete(Path.GetFullPath(path));
            }
        }

        public static PhotoSize ResizeImage(HttpPostedFileBase file, string fileName, int width, int height, PhotoType photoType)
        {
            string thumbnailDirectory = $@"{FilesPath}{DirSeparator}Thumbnails";

            // Check if the directory we are saving to exists
            if (!Directory.Exists(thumbnailDirectory))
            {
                // If it doesn't exist, create the directory
                Directory.CreateDirectory(thumbnailDirectory);
            }

            // Final path we will save our thumbnail
            string imagePath = $@"{thumbnailDirectory}{DirSeparator}{fileName}";
            // Create a stream to save the file to when we're done resizing
            FileStream stream = new FileStream(Path.GetFullPath(imagePath), FileMode.OpenOrCreate);

            // Convert our uploaded file to an image
            Image origImage = Image.FromStream(file.InputStream);

            // resize algo
            if (photoType == PhotoType.Photo)
            {
                var widthAfter = origImage.Width;
                var heightAfter = origImage.Height;

                while (widthAfter > 800 && heightAfter > 600)
                {
                    widthAfter /= DefaultResizeRatio;
                    heightAfter /= DefaultResizeRatio;
                }

                width = widthAfter;
                height = heightAfter;
            }

            // Create a new bitmap with the size of our thumbnail
            Bitmap TempBitmap = new Bitmap(width, height);

            // Create a new image that contains are quality information
            Graphics NewImage = Graphics.FromImage(TempBitmap);
            NewImage.CompositingQuality = CompositingQuality.HighQuality;
            NewImage.SmoothingMode = SmoothingMode.HighQuality;
            NewImage.InterpolationMode = InterpolationMode.HighQualityBicubic;

            // Create a rectangle and draw the image
            Rectangle imageRectangle = new Rectangle(0, 0, width, height);
            NewImage.DrawImage(origImage, imageRectangle);

            // Save the final file
            TempBitmap.Save(stream, origImage.RawFormat);

            // Clean up the resources
            NewImage.Dispose();
            TempBitmap.Dispose();
            origImage.Dispose();
            stream.Close();
            stream.Dispose();

            return new PhotoSize
            {
                Width = width,
                Height = height
            };
        }

        private static string CleanFileName(string fileName)
        {
            return Path.GetInvalidFileNameChars().Aggregate(fileName, (current, c) => current.Replace(c.ToString(), string.Empty).Replace("%", ""));
        }

        public class Photo
        {
            public string Url;

            public PhotoSize PhotoSize;
        }

        public class PhotoSize
        {
            public int Width;
            public int Height;
        }
    }
}