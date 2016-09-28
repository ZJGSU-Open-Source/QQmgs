namespace Twitter.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;

    public class User : IdentityUser
    {
        public User()
        {
            this.Tweets = new HashSet<Tweet>();
            this.Followers = new HashSet<User>();
            this.FollowingUsers = new HashSet<User>();
            this.Photos = new HashSet<Photo>();
            this.CourseReviews = new HashSet<CourseReview>();
            this.UserLoginTraces = new HashSet<UserLogTrace>();
            this.Groups = new HashSet<Group>();
            this.Activities = new HashSet<Activity>();
        }

        public DateTime RegisteredTime { get; set; }

        [MaxLength(30)]
        public string Class { get; set; }
    
        [MaxLength(50)]
        public string Status { get; set; }

        public bool HasAvatarImage { get; set; }

        public string RealName { get; set; }

        public string AvatarImageName { get; set; }

        public override string PhoneNumber { get; set; }

        public virtual ICollection<Reply> Replies { get; set; } 

        public virtual ICollection<Tweet> Tweets { get; set; }

        public virtual ICollection<Group> Groups { get; set; }

        public virtual ICollection<Photo> Photos { get; set; } 

        public virtual ICollection<Activity> Activities { get; set; }

        public virtual ICollection<CourseReview> CourseReviews { get; set; } 

        public virtual ICollection<UserLogTrace> UserLoginTraces { get; set; }

        public virtual ICollection<User> FollowingUsers { get; set; }

        public virtual ICollection<User> Followers { get; set; }

        public virtual ICollection<Tweet> FavouriteTweets { get; set; }

        public virtual ICollection<Photo> FavouritePhotos { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);

            // Add custom user claims here
            return userIdentity;
        }
    }
}