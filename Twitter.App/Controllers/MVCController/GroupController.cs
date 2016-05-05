using System;
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
    [Authorize]
    [RoutePrefix("Group")]
    public class GroupController : TwitterBaseController
    {
        public GroupController()
            : base(new TwitterData())
        {
        }

        [HttpGet]
        [Route("")]
        public ActionResult Index()
        {
            var recentLogs = Data.Group.All()
                .OrderByDescending(t => t.CreatedTime)
                .Select(ViewModelsHelper.AsGroupViewModel)
                .ToList();

            return this.View(recentLogs);
        }

        [HttpGet]
        [Route("{groupId:int}/details/{p:int}")]
        public ActionResult Get(int groupId, int p = 1)
        {
            var group = Data.Group.Find(groupId);
            if (group == null)
            {
                return HttpNotFound($"Group with id {groupId} not found");
            }

            var tweets = group.Tweets;
            if (tweets == null)
            {
                return HttpNotFound($"There's no tweet in the group with id {groupId}");
            }

            var tweetsViewModel = tweets.Select(t => new TweetViewModel
            {
                Id = t.Id,
                Author = t.Author.UserName,
                AuthorStatus = t.Author.Status,
                IsEvent = t.IsEvent,
                Text = t.Text,
                UsersFavouriteCount = t.UsersFavourite.Count,
                RepliesCount = t.Reply.Count,
                RetweetsCount = t.Retweets.Count,
                DatePosted = t.DatePosted,
                GroupId = t.GroupId,
                HasAvatarImage = t.Author.HasAvatarImage,
                AvatarImageName = t.Author.AvatarImageName,
                ReplyList = t.Reply.Select(reply => new ReplyViewModel
                {
                    Text = reply.Content,
                    Id = reply.Id,
                    PublishTime = reply.PublishTime,
                    Author = reply.AuthorName
                }).ToList()
            }).OrderByDescending(t => t.DatePosted).ToPagedList(pageNumber: p, pageSize: Constants.Constants.PageTweetsNumber);

            // pass Group info to view
            ViewData["GroupName"] = group.Name;
            ViewData["GroupId"] = groupId;

            return this.View(tweetsViewModel);
        }

        public ActionResult AddTweet(int groupId)
        {
            var group = Data.Group.Find(groupId);
            if (group == null)
            {
                return HttpNotFound($"Group with id {groupId} not found");
            }

            // pass Group info to view
            ViewData["GroupName"] = group.Name;
            ViewData["GroupId"] = groupId;

            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(CreateGroupBindingModel model)
        {
            var loggedUserId = this.User.Identity.GetUserId();

            try
            {
                var group = new Group
                {
                    Name = model.Name,
                    CreatedTime = DateTime.Now,
                    CreaterId = loggedUserId
                };

                Data.Group.Add(group);
                Data.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Edit(int id)
        {
            return View();
        }

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

        public ActionResult Delete(int id)
        {
            return View();
        }

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

        [HttpGet]
        public ActionResult UploadGroupImage(int groupId)
        {
            ViewData["GroupId"] = groupId;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadGroupImage(HttpPostedFileBase file, int groupId)
        {
            var uploadedFile = FileUploadHelper.UploadFile(file);
            var loggedUserId = this.User.Identity.GetUserId();

            var photo = new Photo
            {
                AuthorId = loggedUserId,
                DatePosted = DateTime.Now,
                Name = uploadedFile,
                PhotoType = PhotoType.GroupImage
            };

            this.Data.Photo.Add(photo);
            this.Data.SaveChanges();

            var group = this.Data.Group.Find(groupId);
            group.HasImageOverview = true;
            group.ImageOverview = photo.Name;
            this.Data.Group.Update(group);
            this.Data.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
