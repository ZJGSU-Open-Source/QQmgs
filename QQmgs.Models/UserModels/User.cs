using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Twitter.Models.ActivityModels;
using Twitter.Models.CourseReviewModels;
using Twitter.Models.DataAnnotations;
using Twitter.Models.GroupModels;
using Twitter.Models.PhotoModels;
using Twitter.Models.TraceModels;

namespace Twitter.Models.UserModels
{
    public class User : IdentityUser
    {
        public User()
        {
            this.UserProfilePhotos = new HashSet<UserProfleImage>();
            this.Tweets = new HashSet<Tweet>();
            this.Followers = new HashSet<User>();
            this.FollowingUsers = new HashSet<User>();
            this.Photos = new HashSet<Image>();
            this.CourseReviews = new HashSet<CourseReview>();
            this.UserLoginTraces = new HashSet<UserLogTrace>();
            this.Groups = new HashSet<Group>();
            this.JoinedActivities = new HashSet<Activity>();
            this.CreatedActivities = new HashSet<Activity>();
        }

        public DateTime RegisteredTime { get; set; }

        // Not required filed
        [CustomRequiredEmail]
        public override string Email { get; set; }

        [MaxLength(30)]
        public string Class { get; set; }
    
        [MaxLength(50)]
        public string Status { get; set; }

        public bool HasAvatarImage { get; set; }

        public string RealName { get; set; }

        public string AvatarImageName { get; set; }

        public override string PhoneNumber { get; set; }

        public virtual ICollection<UserProfleImage> UserProfilePhotos { get; set; }

        public virtual ICollection<Reply> Replies { get; set; } 

        public virtual ICollection<Tweet> Tweets { get; set; }

        public virtual ICollection<Group> Groups { get; set; }

        public virtual ICollection<Image> Photos { get; set; } 

        public virtual ICollection<Activity> CreatedActivities { get; set; }

        public virtual ICollection<Activity> JoinedActivities { get; set; }

        public virtual ICollection<CourseReview> CourseReviews { get; set; } 

        public virtual ICollection<UserLogTrace> UserLoginTraces { get; set; }

        public virtual ICollection<User> FollowingUsers { get; set; }

        public virtual ICollection<User> Followers { get; set; }

        public virtual ICollection<Tweet> FavouriteTweets { get; set; }

        public virtual ICollection<Image> FavouritePhotos { get; set; }

        // MVC Web App
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);

            // Add custom user claims here
            return userIdentity;
        }

        // Web APIs
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);

            // Add custom user claims here
            return userIdentity;
        }
    }
}