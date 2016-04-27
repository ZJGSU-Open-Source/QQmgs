using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using PagedList;
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
            //for (var i = 0; i < 10; ++i)
            //{
            //    var group = this.Data.Group.Find(i);
            //    if (group == null)
            //    {
            //        var testGroup = new Group
            //        {
            //            Name = "TEST" + DateTime.Now.ToString("O"),
            //            CreatedTime = DateTime.Now,
            //            CreaterId = this.User.Identity.GetUserId()
            //        };

            //        Data.Group.Add(testGroup);
            //        Data.SaveChanges();
            //    }
            //}

            var recentLogs = Data.Group.All()
                .OrderByDescending(t => t.CreatedTime)
                .Select(AsGroupViewModel)
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

        public ActionResult Add(int groupId)
        {
            // pass Group info to view
            ViewData["GroupId"] = groupId;

            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

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

        private static readonly Expression<Func<Group, GroupVieModels>> AsGroupViewModel =
            t => new GroupVieModels
            {
                Id = t.Id,
                CreatedTime = t.CreatedTime,
                Name = t.Name,
                TweetsCount = t.Tweets.Count
            };
    }
}
