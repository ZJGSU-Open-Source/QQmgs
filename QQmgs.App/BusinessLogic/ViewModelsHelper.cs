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
    public static class ViewModelsHelper
    {
        #region Select Expression

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
                IsDisplay = g.IsDisplay,
                IsPrivate = g.IsPrivate
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
                Status = u.Status,
                UserId = u.Id,
                HasAvatarImage = u.HasAvatarImage,
                AvatarImageName = u.AvatarImageName
            };

        public static readonly Expression<Func<Photo, PhotoViewModel>> AsPhotoViewModel =
            p => new PhotoViewModel
            {
                Description = p.Descrption,
                Author = p.Author.RealName,
                Name = p.Name,
                Height = p.OriginalHeight,
                Width = p.OriginalWidth,
                Id = p.Id,
                HasAvatarImage = p.Author.HasAvatarImage,
                AvatarImageName = p.Author.AvatarImageName
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

        public static readonly Expression<Func<Activity, ActivityViewModel>> AsActivictyViewModel =
            a => new ActivityViewModel
            {
                Id = a.Id.ToString(),
                Name = a.Name,
                Classficiation = a.Classficiation.ToString(),
                AvatarImage = a.AvatarImage ?? string.Empty,
                CreatorId = a.CreatorId,
                Description = a.Description,
                EndTime = a.EndTime,
                StartTime = a.StartTime,
                Place = a.Place,
                PublishTime = a.PublishTime,
                Creator = string.Empty,
                Participations = a.Participents.Select(participant => new ParticipationViewModel
                {
                    Id = participant.Id,
                    Name = participant.RealName,
                    AvatarImage = participant.AvatarImageName,
                    HasAvatarImage = participant.HasAvatarImage
                }).ToList()
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

        #endregion

        #region To View Model
        public static UserViewModel ToUserViewModel(this User user)
        {
            return new UserViewModel
            {
                RealName = user.RealName,
                Class = user.Class,
                Status = user.Status,
                UserId = user.Id,
                PhoneNumber = user.UserName,
                HasAvatarImage = user.HasAvatarImage,
                AvatarImageName =
                    user.HasAvatarImage
                        ? $"{HTTPHelper.GetUrlPrefix()}/img/Uploads/Thumbnails/{user.AvatarImageName}"
                        : string.Empty,
                JoinedGroups = user.Groups.Select(group => new GroupVieModels
                {
                    Id = group.Id,
                    CreatedTime = group.CreatedTime,
                    Name = group.Name,
                    HasImageOverview = group.HasImageOverview,
                    ImageOverview = group.ImageOverview,
                    Description = group.Description,
                    TweetsCount = group.Tweets.Count,
                    LastTweetUpdateTime = group.LastTweetUpdateTime,
                    IsDisplay = group.IsDisplay,
                    IsPrivate = group.IsPrivate
                }),
                PostedTweets = user.Tweets.Select(tweet => new TweetViewModel
                {
                    Id = tweet.Id,
                    Author = tweet.Author.RealName,
                    AuthorStatus = tweet.Author.Status,
                    AuthorPhoneNumber = tweet.Author.UserName,
                    IsEvent = tweet.IsEvent,
                    Text = tweet.Text,
                    UsersFavouriteCount = tweet.UsersFavourite.Count,
                    RepliesCount = tweet.Reply.Count,
                    RetweetsCount = tweet.Retweets.Count,
                    DatePosted = tweet.DatePosted,
                    GroupId = tweet.GroupId,
                    HasAvatarImage = tweet.Author.HasAvatarImage,
                    AvatarImageName =
                        tweet.Author.HasAvatarImage
                            ? Constants.Constants.WebHostPrefix + "/" + Constants.Constants.ImageThumbnailsPrefix + "/" +
                              tweet.Author.AvatarImageName
                            : null
                }),
                JoinedActivities = user.Activities.Select(activity => new ActivityViewModel
                {
                    Id = activity.Id.ToString(),
                    Name = activity.Name,
                    Classficiation = activity.Classficiation.ToString(),
                    AvatarImage = activity.AvatarImage ?? string.Empty,
                    CreatorId = activity.CreatorId,
                    Description = activity.Description,
                    EndTime = activity.EndTime,
                    StartTime = activity.StartTime,
                    Place = activity.Place,
                    PublishTime = activity.PublishTime,
                    Creator = string.Empty,
                    Participations = activity.Participents.Select(participant => new ParticipationViewModel
                    {
                        Id = participant.Id,
                        Name = participant.RealName,
                        AvatarImage = participant.AvatarImageName,
                        HasAvatarImage = participant.HasAvatarImage
                    }).ToList()
                }),
                PostedPhotos = user.Photos.Select(photo => new PhotoViewModel
                {
                    Description = photo.Descrption,
                    Author = photo.Author.RealName,
                    Name = photo.Name,
                    Height = photo.OriginalHeight,
                    Width = photo.OriginalWidth,
                    Id = photo.Id,
                    HasAvatarImage = photo.Author.HasAvatarImage,
                    AvatarImageName = photo.Author.AvatarImageName
                })
            };
        }

        public static UserViewModel ToUserPostedTweetViewModel(this User user)
        {
            return new UserViewModel
            {
                RealName = user.RealName,
                Class = user.Class,
                Status = user.Status,
                UserId = user.Id,
                PhoneNumber = user.UserName,
                HasAvatarImage = user.HasAvatarImage,
                AvatarImageName =
                    user.HasAvatarImage
                        ? $"{HTTPHelper.GetUrlPrefix()}/img/Uploads/Thumbnails/{user.AvatarImageName}"
                        : string.Empty,
                PostedTweets = user.Tweets.Select(tweet => new TweetViewModel
                {
                    Id = tweet.Id,
                    Author = tweet.Author.RealName,
                    AuthorStatus = tweet.Author.Status,
                    AuthorPhoneNumber = tweet.Author.UserName,
                    IsEvent = tweet.IsEvent,
                    Text = tweet.Text,
                    UsersFavouriteCount = tweet.UsersFavourite.Count,
                    RepliesCount = tweet.Reply.Count,
                    RetweetsCount = tweet.Retweets.Count,
                    DatePosted = tweet.DatePosted,
                    GroupId = tweet.GroupId,
                    HasAvatarImage = tweet.Author.HasAvatarImage,
                    AvatarImageName =
                        tweet.Author.HasAvatarImage
                            ? Constants.Constants.WebHostPrefix + "/" + Constants.Constants.ImageThumbnailsPrefix + "/" +
                              tweet.Author.AvatarImageName
                            : null
                }),
                JoinedActivities = new List<ActivityViewModel>(),
                PostedPhotos = new List<PhotoViewModel>(),
                JoinedGroups = new List<GroupVieModels>()
            };
        }

        public static UserViewModel ToUserPostedPhotosViewModel(this User user)
        {
            return new UserViewModel
            {
                RealName = user.RealName,
                Class = user.Class,
                Status = user.Status,
                UserId = user.Id,
                PhoneNumber = user.UserName,
                HasAvatarImage = user.HasAvatarImage,
                AvatarImageName =
                    user.HasAvatarImage
                        ? $"{HTTPHelper.GetUrlPrefix()}/img/Uploads/Thumbnails/{user.AvatarImageName}"
                        : string.Empty,
                PostedPhotos = user.Photos.Select(photo => new PhotoViewModel
                {
                    Description = photo.Descrption,
                    Author = photo.Author.RealName,
                    Name = photo.Name,
                    Height = photo.OriginalHeight,
                    Width = photo.OriginalWidth,
                    Id = photo.Id,
                    HasAvatarImage = photo.Author.HasAvatarImage,
                    AvatarImageName = photo.Author.AvatarImageName
                }),
                JoinedActivities = new List<ActivityViewModel>(),
                JoinedGroups = new List<GroupVieModels>(),
                PostedTweets = new List<TweetViewModel>()
            };
        }

        public static UserViewModel ToUserJoinedGroupsViewModel(this User user)
        {
            return new UserViewModel
            {
                RealName = user.RealName,
                Class = user.Class,
                Status = user.Status,
                UserId = user.Id,
                PhoneNumber = user.UserName,
                HasAvatarImage = user.HasAvatarImage,
                AvatarImageName =
                    user.HasAvatarImage
                        ? $"{HTTPHelper.GetUrlPrefix()}/img/Uploads/Thumbnails/{user.AvatarImageName}"
                        : string.Empty,
                JoinedGroups = user.Groups.Select(group => new GroupVieModels
                {
                    Id = group.Id,
                    CreatedTime = group.CreatedTime,
                    Name = group.Name,
                    HasImageOverview = group.HasImageOverview,
                    ImageOverview = group.ImageOverview,
                    Description = group.Description,
                    TweetsCount = group.Tweets.Count,
                    LastTweetUpdateTime = group.LastTweetUpdateTime,
                    IsDisplay = group.IsDisplay,
                    IsPrivate = group.IsPrivate
                }),
                JoinedActivities = new List<ActivityViewModel>(),
                PostedTweets = new List<TweetViewModel>(),
                PostedPhotos = new List<PhotoViewModel>()
            };
        }
        public static UserViewModel ToUserJoinedActivitiesViewModel(this User user)
        {
            return new UserViewModel
            {
                RealName = user.RealName,
                Class = user.Class,
                Status = user.Status,
                UserId = user.Id,
                PhoneNumber = user.UserName,
                HasAvatarImage = user.HasAvatarImage,
                AvatarImageName =
                    user.HasAvatarImage
                        ? $"{HTTPHelper.GetUrlPrefix()}/img/Uploads/Thumbnails/{user.AvatarImageName}"
                        : string.Empty,
                JoinedActivities = user.Activities.Select(activity => new ActivityViewModel
                {
                    Id = activity.Id.ToString(),
                    Name = activity.Name,
                    Classficiation = activity.Classficiation.ToString(),
                    AvatarImage = activity.AvatarImage ?? string.Empty,
                    CreatorId = activity.CreatorId,
                    Description = activity.Description,
                    EndTime = activity.EndTime,
                    StartTime = activity.StartTime,
                    Place = activity.Place,
                    PublishTime = activity.PublishTime,
                    Creator = string.Empty,
                    Participations = activity.Participents.Select(participant => new ParticipationViewModel
                    {
                        Id = participant.Id,
                        Name = participant.RealName,
                        AvatarImage = participant.AvatarImageName,
                        HasAvatarImage = participant.HasAvatarImage
                    }).ToList()
                }),
                PostedTweets = new List<TweetViewModel>(),
                PostedPhotos = new List<PhotoViewModel>(),
                JoinedGroups = new List<GroupVieModels>()
            };
        }

        #endregion
    }
}