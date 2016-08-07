using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using Twitter.App.Models.ViewModels;
using Twitter.Data.UnitOfWork;

namespace Twitter.App.Controllers.MVCController
{
    [AllowAnonymous]
    [RoutePrefix("topic")]
    public class CustomizedNameController : TwitterBaseController
    {
        public CustomizedNameController()
            : base(new TwitterData())
        {
        }

        // temp hot topic
        [HttpGet]
        [Route("ShangKeyuan")]
        public ActionResult Redirect()
        {
            return RedirectToAction("Get", "Group", new {groupId = 42, p = 1});
        }

        // GET: CustomizedName
        public ActionResult Index()
        {
            return View();
        }

        // GET: CustomizedName/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: CustomizedName/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CustomizedName/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: CustomizedName/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: CustomizedName/Edit/5
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

        // GET: CustomizedName/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: CustomizedName/Delete/5
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
