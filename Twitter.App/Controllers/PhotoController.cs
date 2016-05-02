using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Twitter.App.BusinessLogic;
using Twitter.App.Models.ViewModels;
using Twitter.Data.UnitOfWork;
using Twitter.Models;

namespace Twitter.App.Controllers
{
    public class PhotoController : TwitterBaseController
    {
        public PhotoController()
            : base(new TwitterData())
        {
        }

        [AllowAnonymous]
        public ActionResult Index()
        {
            var photos =
                this.Data.Photo.All()
                    .OrderByDescending(t => t.DatePosted)
                    .Select(AsPhotoViewModel);

            return View(photos);
        }
        
        [HttpGet]
        public ActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            var uploadedFile = FileUpload.UploadFile(file);
            var loggedUserId = this.User.Identity.GetUserId();

            var photo = new Photo
            {
                AuthorId = loggedUserId,
                DatePosted = DateTime.Now,
                Name = uploadedFile
            };

            this.Data.Photo.Add(photo);
            this.Data.SaveChanges();

            ViewData["uploadedFile"] = uploadedFile;

            return RedirectToAction("Index");
        }

        private static readonly Expression<Func<Photo, PhotoViewModel>> AsPhotoViewModel =
            t => new PhotoViewModel
            {
                Author = t.Author.UserName,
                Name = t.Name
            };
    }
}