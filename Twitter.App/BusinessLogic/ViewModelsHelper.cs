using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using Twitter.App.Models.ViewModels;
using Twitter.Models;

namespace Twitter.App.BusinessLogic
{
    public class ViewModelsHelper
    {
        public static readonly Expression<Func<Group, GroupVieModels>> AsGroupViewModel =
            g => new GroupVieModels
            {
                Id = g.Id,
                CreatedTime = g.CreatedTime,
                Name = g.Name,
                HasImageOverview = g.HasImageOverview,
                ImageOverview = g.ImageOverview,
                Description = g.Description,
                TweetsCount = g.Tweets.Count,
                LastTweetUpdateTime = g.LastTweetUpdateTime,
                IsDisplay = g.IsDisplay
            };

        public static readonly Expression<Func<Tweet, TweetViewModel>> AsTweetViewModel =
            t => new TweetViewModel
            {
                Id = t.Id,
                Author = t.Author.RealName,
                AuthorStatus = t.Author.Status,
                AuthorPhoneNumber = t.Author.UserName,
                IsEvent = t.IsEvent,
                Text = t.Text,
                UsersFavouriteCount = t.UsersFavourite.Count,
                RepliesCount = t.Reply.Count,
                RetweetsCount = t.Retweets.Count,
                DatePosted = t.DatePosted,
                GroupId = t.GroupId,
                HasAvatarImage = t.Author.HasAvatarImage,
                AvatarImageName = t.Author.HasAvatarImage ? Constants.Constants.WebHostPrefix + "/" + Constants.Constants.ImageThumbnailsPrefix + "/" + t.Author.AvatarImageName : null,
                ReplyList = t.Reply.Select(reply => new ReplyViewModel
                {
                    Text = reply.Content,
                    Id = reply.Id,
                    PublishTime = reply.PublishTime,
                    Author = reply.Author.RealName
                }).ToList()
            };

        public static readonly Expression<Func<CourseReview, CourseReviewVideModel>> AsReviewModel =
            r => new CourseReviewVideModel
            {
                Id = r.Id,
                Comment = r.Comment,
                Course = r.Course,
                Teacher = r.Teacher,
                DatePosted = r.DatePosted,
                Author = r.Author.RealName
            };

        public static readonly Expression<Func<User, UserViewModel>> AsUserViewModel =
            u => new UserViewModel
            {
                RealName = u.RealName,
                Class = u.Class,
                Status = u.Status
            };

        public static readonly Expression<Func<Photo, PhotoViewModel>> AsPhotoViewModel =
            p => new PhotoViewModel
            {
                Description = p.Descrption,
                Author = p.Author.RealName,
                Name = p.Name,
                DatePosted = p.DatePosted,
                Height = p.OriginalHeight,
                Width = p.OriginalWidth
            };
    }
}