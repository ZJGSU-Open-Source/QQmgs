using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using PagedList;
using Twitter.App.BusinessLogic;
using Twitter.App.Models.BindingModels;
using Twitter.App.Models.ViewModels;
using Twitter.Data.UnitOfWork;
using Twitter.Models;

namespace Twitter.App.Controllers
{
    [RoutePrefix("photo")]
    public class PhotoController : TwitterBaseController
    {
        public PhotoController()
            : base(new QQmgsData())
        {
        }

        [AllowAnonymous]
        [Route("")]
        public ActionResult Index(int pageNumber = 1)
        {
            //var photos =
            //    this.Data.Photo.All()
            //        .OrderByDescending(p => p.DatePosted)
            //        .Where(p => p.PhotoType == PhotoType.Photo && p.IsSoftDelete != true)
            //        .Select(AsPhotoViewModel)
            //        .ToPagedList(pageNumber: pageNumber, pageSize: Constants.Constants.PagePhotosNumber);

            return View();
        }

        [HttpGet]
        public ActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Upload(UploadPhotoBindingModel model)
        {
            var uploadedFile = FileUploadHelper.UploadFile(model.File, PhotoType.Photo);
            var loggedUserId = this.User.Identity.GetUserId();

            try
            {
                var photo = new Photo
                {
                    AuthorId = loggedUserId,
                    DatePosted = DateTime.Now,
                    Name = uploadedFile.Url,
                    PhotoType = PhotoType.Photo,
                    Descrption = model.Description,
                    IsSoftDelete = false,
                    OriginalHeight = uploadedFile.PhotoSize.Height,
                    OriginalWidth = uploadedFile.PhotoSize.Width
                };

                this.Data.Photo.Add(photo);
                this.Data.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                this.Response.StatusCode = 400;
                return this.Json(this.ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
            }
        }

        [HttpPost]
        public ActionResult Favourite(int photoId)
        {
            var loggedUserId = this.User.Identity.GetUserId();
            var photo = this.Data.Photo.Find(photoId);

            if (photo == null)
            {
                return HttpNotFound();
            }

            if (Data.Users.Find(loggedUserId).FavouritePhotos.Contains(photo))
            {
                this.Data.Users.Find(loggedUserId).FavouritePhotos.Remove(photo);
                this.Data.SaveChanges();
            }
            else
            {
                this.Data.Users.Find(loggedUserId).FavouritePhotos.Add(photo);
                this.Data.SaveChanges();
            }

            return View();
        }


        private static readonly Expression<Func<Photo, PhotoViewModel>> AsPhotoViewModel =
            t => new PhotoViewModel
            {
                Author = t.Author.RealName,
                Name = t.Name,
                Description = t.Descrption
            };
    }
}