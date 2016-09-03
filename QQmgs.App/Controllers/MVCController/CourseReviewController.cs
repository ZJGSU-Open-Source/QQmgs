using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Twitter.App.BusinessLogic;
using Twitter.App.Common;
using Twitter.App.Models.BindingModels;
using Twitter.Data.UnitOfWork;
using Twitter.Models;

namespace Twitter.App.Controllers.MVCController
{
    [Authorize]
    [RoutePrefix("CourseReview")]
    public class CourseReviewController : TwitterBaseController
    {
        public CourseReviewController() 
            : base(new QQmgsData())
        {
        }

        // GET: CourseReview
        [HttpGet]
        [Route]
        public ActionResult Index()
        {
            var reviews = this.Data.CourseReview.All()
                .OrderByDescending(review => review.DatePosted)
                .Select(ViewModelsHelper.AsReviewModel);
            
            return View(reviews);
        }

        // GET: CourseReview/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: CourseReview/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CourseReview/Create
        [HttpPost]
        public ActionResult Create(CreateReviewBindingModel model)
        {
            try
            {
                if (!this.ModelState.IsValid)
                {
                    this.Response.StatusCode = 400;
                    return this.Json(this.ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
                }

                var loggedUserId = this.User.Identity.GetUserId();

                var review = new CourseReview
                {
                    AuthorId = loggedUserId,
                    Comment = model.Comment,
                    Course = model.Course,
                    Teacher = model.Teacher,
                    DatePosted = DateTime.Now
                };

                this.Data.CourseReview.Add(review);
                this.Data.SaveChanges();

                return this.RedirectToAction("Index");
            }
            catch
            {
                return this.RedirectToAction("Index");
            }
        }

        // GET: CourseReview/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: CourseReview/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: CourseReview/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: CourseReview/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: CourseReview/Search/courseName="Math"
        [HttpGet]
        public ActionResult Search(string courseName)
        {
            Guard.ArgumentNotNullOrEmpty(courseName, nameof(courseName));

            var reviews = this.Data.CourseReview.All()
                .Where(r => r.Course.Contains(courseName))
                .OrderByDescending(r => r.DatePosted)
                .Select(ViewModelsHelper.AsReviewModel);

            ViewData["CourseName"] = courseName;

            return View(reviews);
        }
    }
}
