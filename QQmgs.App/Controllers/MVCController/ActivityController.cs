using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Twitter.App.BusinessLogic;
using Twitter.App.Models.BindingModel;
using Twitter.App.Models.BindingModels;
using Twitter.Data.UnitOfWork;
using Twitter.Models;
using Twitter.Models.ActivityModels;
using Twitter.Models.GroupModels;
using Twitter.Models.Interfaces;

namespace Twitter.App.Controllers.MVCController
{
    [RoutePrefix("Activity")]
    public class ActivityController : TwitterBaseController
    {
        public ActivityController() 
            : base(new QQmgsData())
        {
        }

        // GET: Activity
        public ActionResult Index()
        {
            var activities = this.Data.Activity.All()
                .OrderByDescending(activity => activity.PublishTime)
                .Select(ViewModelsHelper.AsActivictyViewModel)
                .ToList();

            // get each creator name from DB
            activities.ForEach(model =>
            {
                model.Creator = this.Data.Users.Find(model.CreatorId).RealName;
            });

            return View(activities);
        }

        // GET: Activity/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Activity/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Activity/Create
        [HttpPost]
        public ActionResult Create(CreateActivityBindingModel model)
        {
            try
            {
                var activity = new Activity
                {
                    CreatorId = this.User.Identity.GetUserId(),
                    Name = model.Name,
                    Description = model.Description,
                    PublishTime = DateTime.Now,
                    Classficiation = EnumUtils.Parse<ActivityClassficiation>(model.Classfication ?? ActivityClassficiation.Other.ToString()),
                    Place = model.Place,
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now
                };

                this.Data.Activity.Add(activity);
                this.Data.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Activity/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Activity/Edit/5
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

        // GET: Activity/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Activity/Delete/5
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
    }
}
