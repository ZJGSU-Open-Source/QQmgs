using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Twitter.App.Models.BindingModel;
using Twitter.App.Models.ViewModels;
using Twitter.Data.UnitOfWork;
using Twitter.Models;

namespace Twitter.App.Controllers
{
    [RoutePrefix("dev")]
    public class DevController : TwitterBaseController
    {
        public DevController() 
            : base(new QQmgsData())
        {
        }
    
        // GET: DevLog
        public ActionResult Index()
        {
            var recentLogs = Data.DevLog.All()
                .OrderByDescending(t => t.PublishedTime)
                .Select(AsDevLogViewModel)
                .ToList();

            return this.View(recentLogs);
        }
        
        // GET: DevLog/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DevLog/Create
        [HttpPost]
        public ActionResult Create(CreateLogBindingModel model)
        {
            try
            {
                var devLog = new DevLog
                {
                    Log = model.Log,
                    PublishedTime = DateTime.Now
                };

                Data.DevLog.Add(devLog);
                Data.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: DevLog/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: DevLog/Edit/5
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

        // GET: DevLog/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: DevLog/Delete/5
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

        private static readonly Expression<Func<DevLog, DevLogViewModel>> AsDevLogViewModel =
            t => new DevLogViewModel
            {
                Id = t.Id,
                PublishedTime = t.PublishedTime,
                Log = t.Log
            };
    }
}
