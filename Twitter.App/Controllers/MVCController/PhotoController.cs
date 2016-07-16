using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
    public class PhotoController : TwitterBaseController
    {
        public PhotoController()
            : base(new TwitterData())
        {
        }

        [AllowAnonymous]
        [Route("")]
        public ActionResult Index(int pageNumber = 1)
        {
            var photos =
                this.Data.Photo.All()
                    .OrderByDescending(p => p.DatePosted)
                    .Where(p => p.PhotoType == PhotoType.Photo && p.IsSoftDelete != true)
                    .Select(AsPhotoViewModel)
                    .ToPagedList(pageNumber: pageNumber, pageSize: Constants.Constants.PagePhotosNumber);

            return View(photos);
        }

        [HttpGet]
        public ActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Upload(UploadPhotoBindingModel model)
        {
            var uploadedFile = FileUploadHelper.UploadFile(model.File);
            var loggedUserId = this.User.Identity.GetUserId();

            try
            {
                var photo = new Photo
                {
                    AuthorId = loggedUserId,
                    DatePosted = DateTime.Now,
                    Name = uploadedFile,
                    PhotoType = PhotoType.Photo,
                    Descrption = model.Description,
                    IsSoftDelete = false
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

        private static readonly Expression<Func<Photo, PhotoViewModel>> AsPhotoViewModel =
            t => new PhotoViewModel
            {
                Author = t.Author.RealName,
                Name = t.Name,
                Description = t.Descrption
            };
    }
}