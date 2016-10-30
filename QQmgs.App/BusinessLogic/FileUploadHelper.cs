using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using Glimpse.Core.Extensibility;
using Twitter.Models;
using Twitter.Models.Interfaces;

namespace Twitter.App.BusinessLogic
{
    public static class FileUploadHelper
    {
        public static char DirSeparator = Path.DirectorySeparatorChar;

        public static string FilesPath =
            HttpContext.Current.Server.MapPath(Constants.Constants.PhotoLocation);

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
            var size = ResizeImage(file, fileName, Constants.Constants.DefaultResizeSize, Constants.Constants.DefaultResizeSize, photoType);

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

        private static string GetThumbnailDirectory()
        {
            string thumbnailDirectory = $@"{FilesPath}{DirSeparator}Thumbnails";

            // Check if the directory we are saving to exists
            if (!Directory.Exists(thumbnailDirectory))
            {
                // If it doesn't exist, create the directory
                Directory.CreateDirectory(thumbnailDirectory);
            }

            return thumbnailDirectory;
        }

        public static PhotoSize ResizeImage(HttpPostedFileBase file, string fileName, int width, int height, PhotoType photoType)
        {
            // Final path we will save our thumbnail
            var thumbnailDirectory = GetThumbnailDirectory();
            string imagePath = $@"{thumbnailDirectory}{DirSeparator}{fileName}";

            // Create a stream to save the file to when we're done resizing
            FileStream stream = new FileStream(Path.GetFullPath(imagePath), FileMode.OpenOrCreate);

            // Convert our uploaded file to an image
            System.Drawing.Image origImage = System.Drawing.Image.FromStream(file.InputStream);

            // resize algo
            if (photoType == PhotoType.Photo)
            {
                var widthAfter = origImage.Width;
                var heightAfter = origImage.Height;

                while (widthAfter > 800 && heightAfter > 600)
                {
                    widthAfter /= Constants.Constants.DefaultResizeRatio;
                    heightAfter /= Constants.Constants.DefaultResizeRatio;
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

        public static PhotoSize ResizeImage(int newWidth, int newHeight, string stPhotoPath, string photoName, PhotoType photoType)
        {
            // Final path we will save our thumbnail
            var thumbnailDirectory = GetThumbnailDirectory();
            string imagePath = $@"{thumbnailDirectory}{DirSeparator}{photoName}";

            // Create a stream to save the file to when we're done resizing
            FileStream stream = new FileStream(Path.GetFullPath(imagePath), FileMode.OpenOrCreate);

            System.Drawing.Image imgPhoto = System.Drawing.Image.FromFile(stPhotoPath);

            // resize algo
            if (photoType == PhotoType.AvatarImage)
            {
                var widthAfter = imgPhoto.Width;
                var heightAfter = imgPhoto.Height;

                while (widthAfter > 300 && heightAfter > 200)
                {
                    widthAfter /= Constants.Constants.DefaultResizeRatio;
                    heightAfter /= Constants.Constants.DefaultResizeRatio;
                }

                newWidth = widthAfter;
                newHeight = heightAfter;
            }
            else if (photoType == PhotoType.ActivityImage)
            {
                var widthAfter = imgPhoto.Width;
                var heightAfter = imgPhoto.Height;

                // keep 16 : 9
                while (widthAfter > 600 && heightAfter > 338)
                {
                    widthAfter /= Constants.Constants.DefaultResizeRatio;
                    heightAfter /= Constants.Constants.DefaultResizeRatio;
                }

                newWidth = widthAfter;
                newHeight = heightAfter;
            }

            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;

            //Consider vertical pics
            if (sourceWidth < sourceHeight)
            {
                int buff = newWidth;

                newWidth = newHeight;
                newHeight = buff;
            }

            int sourceX = 0, sourceY = 0, destX = 0, destY = 0;
            float nPercent = 0, nPercentW = 0, nPercentH = 0;

            nPercentW = ((float)newWidth / (float)sourceWidth);
            nPercentH = ((float)newHeight / (float)sourceHeight);
            if (nPercentH < nPercentW)
            {
                nPercent = nPercentH;
                destX = System.Convert.ToInt16((newWidth -
                          (sourceWidth * nPercent)) / 2);
            }
            else
            {
                nPercent = nPercentW;
                destY = System.Convert.ToInt16((newHeight -
                          (sourceHeight * nPercent)) / 2);
            }

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);


            Bitmap bmPhoto = new Bitmap(newWidth, newHeight,
                          PixelFormat.Format24bppRgb);

            bmPhoto.SetResolution(imgPhoto.HorizontalResolution,
                         imgPhoto.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.Clear(Color.Black);
            grPhoto.InterpolationMode =
                System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            grPhoto.DrawImage(imgPhoto,
                new Rectangle(destX, destY, destWidth, destHeight),
                new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                GraphicsUnit.Pixel);

            
            // Save the final file
            bmPhoto.Save(stream, imgPhoto.RawFormat);

            // Dispose
            grPhoto.Dispose();
            imgPhoto.Dispose();
            stream.Close();
            stream.Dispose();
            bmPhoto.Dispose();

            return new PhotoSize
            {
                Width = newWidth,
                Height = newHeight
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