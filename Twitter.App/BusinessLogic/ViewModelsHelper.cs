using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using Twitter.App.DataContracts;
using Twitter.App.Models.ViewModels;
using Twitter.Models;
using Twitter.Models.Trace;

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
                AvatarImageName =
                    t.Author.HasAvatarImage
                        ? Constants.Constants.WebHostPrefix + "/" + Constants.Constants.ImageThumbnailsPrefix + "/" +
                          t.Author.AvatarImageName
                        : null,
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
                Height = p.OriginalHeight,
                Width = p.OriginalWidth
            };

        public static readonly Expression<Func<UserLogTrace, UserLoginTraceViewModel>> AsUserLoginTraceVideModel =
            u => new UserLoginTraceViewModel
            {
                DatePosted = u.DatePosted,
                Id = u.TraceId,
                IpAddress = u.IpAddress,
                IsLoggedSucceeded = u.IsLoggedSucceeded,
                LoggedUserPhoneNumber = u.PhoneNumber
            };

        public static readonly Expression<Func<HighAccLocationByIpResult, UserLoginTraceViewModel>>
            AsUserLoginTraceViewModel = u => new UserLoginTraceViewModel
            {
                DatePosted = u.DatePosted,
                Id = u.Id,
                IpAddress = u.IpAddress,
                IsLoggedSucceeded = u.IsLoggedSucceeded,
                LoggedUserName = u.LoggedUserName,
                LoggedUserPhoneNumber = u.LoggedUserPhoneNumber,
                HighAccIpLocation = new HighAccIpLocation
                {
                    Content = new Content
                    {
                        FormattedAddress = u.FormattedAddress,
                        Confidence = u.Confidence ?? 0.0,
                        AddressComponent = new AddressComponent
                        {
                            AdminAreaCode = u.AdminAreaCode ?? 0,
                            City = u.City,
                            Country = u.Country,
                            District = u.District,
                            Province = u.Province,
                            Street = u.Street,
                            StreetNumber = u.StreetNumber
                        },
                        Business = u.Business,
                        LocId = u.LocId,
                        Location = new Location
                        {
                            Lat = u.Lat ?? 0.0,
                            Lng = u.Lng ?? 0.0
                        },
                        Radius = u.Radius ?? 0.0
                    },
                    Result = new Result
                    {
                        Error = u.Error ?? 0,
                        LocalTime = u.LocalTime
                    }
                }
            };
    }
}