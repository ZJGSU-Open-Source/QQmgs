using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Twitter.App.BusinessLogic;
using Twitter.App.Common;

namespace Twitter.App.Controllers.MVCController
{
    [Authorize]
    [RoutePrefix("conversation")]
    public class ConversationController : Controller
    {
        // GET: Chatting
        public ActionResult Chatting(string cid)
        {
            var userPair = Base64Coding.Decrption(cid);
            var users = userPair.Split('_');

            Guard.Argument(() => users.Length == 2, nameof(users));

            var sender = users[0];
            var receiver = users[1];

            Guard.ArgumentNotNullOrEmpty(sender, nameof(sender));
            Guard.ArgumentNotNullOrEmpty(receiver, nameof(receiver));

            ViewData["sender"] = sender;
            ViewData["receiver"] = receiver;

            return View();
        }

        // GET: Chatting
        public ActionResult GenerateChatting(string user1, string user2)
        {
            var cypherString = Base64Coding.Encrption(user1, user2);
            ViewData["string"] = cypherString;

            return View();
        }

        // GET: Conversation
        public ActionResult Index()
        {
            return View();
        }

        // GET: Conversation/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Conversation/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Conversation/Create
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

        // GET: Conversation/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Conversation/Edit/5
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

        // GET: Conversation/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Conversation/Delete/5
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
