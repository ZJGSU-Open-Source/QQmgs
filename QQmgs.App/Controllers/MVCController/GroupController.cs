using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using PagedList;
using Twitter.App.BusinessLogic;
using Twitter.App.Common;
using Twitter.App.Models.BindingModel;
using Twitter.App.Models.ViewModels;
using Twitter.Data.UnitOfWork;
using Twitter.Models;
using WebGrease.Css.Extensions;

namespace Twitter.App.Controllers
{
    [Authorize]
    [RoutePrefix("Group")]
    public class GroupController : TwitterBaseController
    {
        public GroupController()
            : base(new QQmgsData())
        {
        }

        [HttpGet]
        [Route]
        public ActionResult Index()
        {
            var groups = this.Data.Group.All()
                .OrderByDescending(t => t.LastTweetUpdateTime)
                .Select(ViewModelsHelper.AsGroupViewModel)
                .Where(models => models.IsDisplay)
                .ToList();

            return this.View(groups);
        }

        public ActionResult GetClassificatedGroup(Classification classification)
        {
            Guard.ArgumentNotNull(classification, nameof(classification));

            var groups = this.Data.Group.All()
                .Where(group => group.Classification == classification)
                .OrderByDescending(group => group.CreatedTime)
                .Select(ViewModelsHelper.AsGroupViewModel)
                .Where(models => models.IsDisplay)
                .ToList();

            ViewData["Classification"] = classification;

            return PartialView(groups);
        }

        [HttpGet]
        [Route("Recommand")]
        public ActionResult GetUserRecommand()
        {
            //var loggedUserId = this.User.Identity.GetUserId();

            //var tweets = this.Data.Users.Find(loggedUserId).Tweets.ToList();
            //var recordDictionary = new Dictionary<int, int>();
            //foreach (var tweet in tweets)
            //{
            //    recordDictionary[tweet.GroupId]++;
            //}
            //var result = recordDictionary.Where(pair => pair.Key != 0)
            //    .OrderBy(pair => pair.Value).Select(pair => pair.Key);

            //foreach (var groupId in result)
            //{
            //    this.Data.Group.Find(groupId)
            //}


            var groupQuard = this.Data.Group.All()
                .OrderByDescending(group => group.Tweets.Count)
                .Select(ViewModelsHelper.AsGroupViewModel)
                .Take(4).ToList();

            return PartialView(groupQuard);
        }

        [HttpGet]
        [Route("ParticipatedGroups")]
        public ActionResult GetParticipatedGroups()
        {
            var loggedUserId = this.User.Identity.GetUserId();

            // get posted tweets
            var tweets = Data.Users.Find(loggedUserId).Tweets;
            var groupIds = tweets.Select(tweet => tweet.GroupId).Distinct().ToList();
            
            // get posted replies
            var replies = Data.Users.Find(loggedUserId).Replies;
            var tweetIds = replies.Select(reply => reply.TweetId).Distinct();
            groupIds.AddRange(tweetIds.Select(id => Data.Tweets.Find(id).GroupId).Distinct());

            groupIds = groupIds.Distinct().ToList();

            // get detailed groups
            var groups =
                groupIds.Select(id => Data.Group.Find(id).ToGroupVieModel())
                    .OrderByDescending(models => models.LastTweetUpdateTime).ToList();

            return PartialView(groups);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("{groupId:int}/details/{p:int}")]
        public ActionResult Get(int groupId, int p = 1)
        {
            var group = Data.Group.Find(groupId);
            if (group == null)
            {
                return HttpNotFound($"Group with id {groupId} not found");
            }

            // check private group
            if (group.IsPrivate)
            {
                var loggedUserId = this.User.Identity.GetUserId();

                var foundUser = group.Users.Any(user => user.Id == loggedUserId);
                if (foundUser == false)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }

            var tweets = group.Tweets;
            if (tweets == null)
            {
                return HttpNotFound($"There's no tweet in the group with id {groupId}");
            }

            var tweetsViewModel = tweets.Select(t => new TweetViewModel
            {
                Id = t.Id,
                Author = t.Author.RealName,
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
                    Author = reply.Author.RealName,
                    AvatarImageName = reply.Author.AvatarImageName,
                    HasAvatarImage = reply.Author.HasAvatarImage
                }).ToList()
            }).OrderByDescending(t => t.DatePosted).ToPagedList(pageNumber: p, pageSize: Constants.Constants.PageTweetsNumber);

            // pass Group info to view
            ViewData["GroupName"] = group.Name;
            ViewData["GroupId"] = groupId;
            ViewData["PageNumber"] = p;
            ViewData["IsPrivate"] = group.IsPrivate ? "true" : "false";

            return this.View(tweetsViewModel);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(CreateGroupBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return this.View(model);
            }

            var loggedUserId = this.User.Identity.GetUserId();
            var currentTime = DateTime.Now;

            // transform string value to bool value
            var isPrivate = string.Compare(model.IsPrivate, "on", StringComparison.OrdinalIgnoreCase) == 0;

            var group = new Group
            {
                Name = model.Name,
                Description = model.Description,
                CreatedTime = currentTime,
                CreaterId = loggedUserId,
                LastTweetUpdateTime = currentTime,
                IsDisplay = true,
                Classification = Classification.未分类,
                IsPrivate = isPrivate
            };

            if (isPrivate)
            {
                var user = this.Data.Users.Find(loggedUserId);
                if (user == null)
                {
                    return HttpNotFound($"User with id {loggedUserId} not found");
                }

                group.Users.Add(user);
            }

            this.Data.Group.Add(group);
            this.Data.SaveChanges();

            return RedirectToAction("Get", new {groupId = group.Id, p = 1});
        }

        [HttpGet]
        [Route("{groupId:int}/edit")]
        public ActionResult Edit(int? groupId)
        {
            if (groupId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var group = this.Data.Group.Find(groupId);
            if (group == null)
            {
                return HttpNotFound();
            }

            //// temp delete
            //if (group.CreaterId != User.Identity.GetUserId())
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}

            return View(group);
        }

        [HttpPost]
        public ActionResult Edit(EditGroupBindingModel group)
        {
            try
            {
                var existedGroup = this.Data.Group.Find(group.Id);
                if (existedGroup == null)
                {
                    return HttpNotFound();
                }

                existedGroup.Name = group.Name;
                existedGroup.Description = group.Description;

                Data.Group.Update(existedGroup);
                Data.SaveChanges();

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
            var uploadedFile = FileUploadHelper.UploadFile(file, PhotoType.GroupImage);
            var loggedUserId = this.User.Identity.GetUserId();

            var photo = new Photo
            {
                AuthorId = loggedUserId,
                Name = uploadedFile.Url,
                PhotoType = PhotoType.GroupImage,
                DatePosted = DateTime.Now
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

        [HttpGet]
        public ActionResult Search(CreateSearchBindingModel model)
        {
            var users = this.Data.Users.All()
                .Where(user => user.RealName.Contains(model.SerachWords))
                .OrderByDescending(user => user.RegisteredTime)
                .Select(ViewModelsHelper.AsUserViewModel)
                .ToList().Take(3);

            var groups = this.Data.Group.All()
                .Where(group => group.Name.Contains(model.SerachWords))
                .OrderByDescending(group => group.CreatedTime)
                .Select(ViewModelsHelper.AsGroupViewModel)
                .Where(models => models.IsDisplay)
                .ToList().Take(5);

            var tweets = this.Data.Tweets.All()
                .Where(tweet => tweet.Text.Contains(model.SerachWords))
                .OrderByDescending(tweet => tweet.DatePosted)
                .Select(ViewModelsHelper.AsTweetViewModel)
                .ToList().Take(10);

            var searchResult = new SearchResultViewModel
            {
                Groups = groups,
                Users = users,
                Tweets = tweets
            };

            ViewData["SearchWords"] = model.SerachWords;

            return this.View(searchResult);
        }

        [HttpGet]
        public ActionResult GetSearchGroup()
        {
            return this.PartialView();
        }

        [HttpPost]
        public ActionResult SearchGroup(CreateSearchBindingModel model)
        {
            var groups = this.Data.Group.All()
                .Where(group => group.Name.Contains(model.SerachWords))
                .OrderByDescending(group => group.CreatedTime)
                .Select(ViewModelsHelper.AsGroupViewModel)
                .Where(models => models.IsDisplay)
                .ToList().Take(5);

            var searchResult = new SearchResultViewModel
            {
                Groups = groups,
            };

            ViewData["SearchWords"] = model.SerachWords;

            return this.View(searchResult);
        }

        [HttpGet]
        public ActionResult GetAllGroups()
        {
            var loggedUserId = this.User.Identity.GetUserId();
            var user = this.Data.Users.Find(loggedUserId);
            if (user == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var groups = user.Groups.Select(group => new GroupVieModels
            {
                Id = group.Id,
                Description = group.Description,
                Name = group.Name,
                CreatedTime = group.CreatedTime,
                CreaterId = group.CreaterId,
                HasImageOverview = group.HasImageOverview,
                ImageOverview = group.ImageOverview,
                IsDisplay = group.IsDisplay,
                LastTweetUpdateTime = group.LastTweetUpdateTime,
                TweetsCount = group.Tweets.Count,
                IsPrivate = group.IsPrivate
            }).OrderByDescending(models => models.LastTweetUpdateTime);

            return PartialView(groups);
        }

        [HttpGet]
        [Route("{groupId:int}/members")]
        public ActionResult GetGroupMembers(int? groupId)
        {
            // check groupId
            if (groupId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // check group
            var group = this.Data.Group.Find(groupId);
            if (group == null)
            {
                return HttpNotFound();
            }

            // check user permission
            var loggedUserId = this.User.Identity.GetUserId();
            if (IsNotGroupMember(group, loggedUserId))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var users = group.Users.Select(user => new UserViewModel
            {
                RealName = user.RealName,
                Class = user.Class,
                Status = user.Status,
                AvatarImageName = user.AvatarImageName,
                HasAvatarImage = user.HasAvatarImage,
                UserId = user.Id
            });

            var isGroupOwner = !IswNotGroupOwner(group, loggedUserId);

            ViewData["GroupId"] = groupId;
            ViewData["IsGroupOwner"] = isGroupOwner;

            return PartialView(users);
        }

        [HttpGet]
        public ActionResult InviteUserToGroup(int? groupId)
        {
            // check groupId
            if (groupId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // check group
            var group = this.Data.Group.Find(groupId);
            if (group == null)
            {
                return HttpNotFound();
            }

            ViewData["GroupId"] = groupId;

            return PartialView();
        }

        [HttpPost]
        public ActionResult InviteUserToGroup(InviteUserToGroupBindingModel model)
        {
            // check user permission
            var loggedUserId = this.User.Identity.GetUserId();
            var group = this.Data.Group.Find(model.GroupId);

            if (group == null)
            {
                return HttpNotFound($"Group with Id:{model.GroupId} not found");
            }

            if (group.Users.All(u => u.Id != loggedUserId))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"User with id {loggedUserId} don't have the permission to invite others");
            }

            // check invite user
            var users =
                this.Data.Users.All()
                    .Where(u => u.RealName == model.UserName && u.Groups.All(g => g.Id != model.GroupId));

            User user;
            switch (users.Count())
            {
                case 0:
                    return HttpNotFound($"User with name:{model.UserName} not found");
                case 1:
                    user = users.FirstOrDefault();

                    if (user == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
                    }

                    break;
                default:
                    var usersViewModel = users.Select(ViewModelsHelper.AsUserViewModel).ToList();

                    ViewData["GroupId"] = model.GroupId;
                    ViewData["GroupName"] = group.Name;
                    ViewData["UserName"] = model.UserName;

                    return View("InviteFriendView", usersViewModel);
            }

            // check target user group
            if (user.Groups.Any(g => g.Id == model.GroupId))
            {
                return RedirectToAction("Get", new {groupId = model.GroupId, p = 1});
            }

            user.Groups.Add(group);
            this.Data.Users.Update(user);
            this.Data.SaveChanges();

            return RedirectToAction("Get", new { groupId = model.GroupId, p = 1 });
        }

        [HttpGet]
        public ActionResult SetGroupVisibility(int groupId)
        {
            var group = this.Data.Group.Find(groupId);
            if (group == null)
            {
                return HttpNotFound($"Group with Id:{groupId} not found");
            }

            // check user permission
            var loggedUserId = this.User.Identity.GetUserId();
            if (IsNotGroupMember(group, loggedUserId))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"User with id {loggedUserId} is not the owner of the group with id {groupId}");
            }

            var isOwner = !IswNotGroupOwner(group, loggedUserId);

            var isDisplay = group.IsDisplay;

            ViewData["GroupId"] = groupId;
            ViewData["IsOwner"] = isOwner;

            return PartialView(isDisplay);
        }

        [HttpPost]
        public ActionResult SetGroupVisibility(SetGroupVisibilityBindingModel model)
        {
            var isDisplay = string.Compare(model.IsDisplay, "on", StringComparison.OrdinalIgnoreCase) == 0;
            var group = this.Data.Group.Find(model.GroupId);
            if (group == null)
            {
                return HttpNotFound($"Group with Id:{model.GroupId} not found");
            }

            group.IsDisplay = isDisplay;
            this.Data.Group.Update(group);
            this.Data.SaveChanges();

            return RedirectToAction("Get", new { groupId = model.GroupId, p = 1 });
        }

        public ActionResult InviteFriendToGroup(int groupId, string userId)
        {
            // check user permission
            var loggedUserId = this.User.Identity.GetUserId();
            var group = this.Data.Group.Find(groupId);

            if (group == null)
            {
                return HttpNotFound($"Group with Id:{groupId} not found");
            }

            if (group.Users.All(u => u.Id != loggedUserId))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"User with id {loggedUserId} don't have the permission to invite others");
            }

            // check invite user
            var user = this.Data.Users.Find(userId);
            if (user == null)
            {
                return HttpNotFound($"User with id:{userId} not found");
            }

            // check target user group
            if (user.Groups.Any(g => g.Id == groupId))
            {
                return RedirectToAction("Get", new { groupId = groupId, p = 1 });
            }

            user.Groups.Add(group);
            this.Data.Users.Update(user);
            this.Data.SaveChanges();

            return RedirectToAction("Get", new { groupId = groupId, p = 1 });
        }

        public ActionResult DeleteGroupmember(string userId, int groupId)
        {
            // check user permission
            var loggedUserId = this.User.Identity.GetUserId();
            var group = this.Data.Group.Find(groupId);

            if (group == null)
            {
                return HttpNotFound($"Group with Id:{groupId} not found");
            }

            if (group.Users.All(u => u.Id != loggedUserId) || IswNotGroupOwner(group, loggedUserId))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"User with id {loggedUserId} don't have the permission to delete member in group {groupId}");
            }

            // check user to delete
            var user = this.Data.Users.Find(userId);
            if (user == null)
            {
                return HttpNotFound($"User with id:{userId} not found");
            }

            group.Users.Remove(user);
            this.Data.Group.Update(group);
            this.Data.SaveChanges();

            return RedirectToAction("Get", new { groupId = groupId, p = 1 });
        }

        private static bool IsNotGroupMember(Group group, string userId)
        {
            return group.Users.All(user => user.Id != userId);
        }

        private static bool IswNotGroupOwner(Group group, string userId)
        {
            return group.CreaterId != userId;
        }
    }
}
