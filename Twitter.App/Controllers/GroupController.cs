using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Twitter.App.Models.BindingModels;
using Twitter.App.Models.ViewModels;
using Twitter.Data.UnitOfWork;
using Twitter.Models;

namespace Twitter.App.Controllers
{
    [Authorize]
    [RoutePrefix("Group")]
    public class GroupController : TwitterBaseController
    {
        public GroupController() 
            : base(new TwitterData())
        {
        }
    
        public ActionResult Index()
        {
            for (var i = 0; i < 10; ++i)
            {
                var group = this.Data.Group.Find(i);
                if (group == null)
                {
                    var testGroup = new Group
                    {
                        Name = "TEST" + DateTime.Now.ToString("O"),
                        CreatedTime = DateTime.Now,
                        CreaterId = this.User.Identity.GetUserId()
                    };

                    Data.Group.Add(testGroup);
                    Data.SaveChanges();
                }
            }
           
            var recentLogs = Data.Group.All()
                .OrderByDescending(t => t.CreatedTime)
                .Select(AsGroupViewModel)
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

        private static readonly Expression<Func<Group, GroupVieModels>> AsGroupViewModel =
            t => new GroupVieModels
            {
                Id = t.Id,
                CreatedTime = t.CreatedTime,
                Name = t.Name
            };
    }
}
