using Twitter.Models.ActivityModels;
using Twitter.Models.CourseReviewModels;
using Twitter.Models.GroupModels;
using Twitter.Models.PhotoModels;
using Twitter.Models.TraceModels;
using Twitter.Models.UserModels;

namespace Twitter.Data.UnitOfWork
{
    using Twitter.Data.Repositories;

    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;

    using Twitter.Models;

    public interface IQQmgsData
    {
        IRepository<Tweet> Tweets { get; }

        IRepository<Message> Messages { get; }

        IRepository<Report> Reports { get; }

        IRepository<Notification> Notifications { get; }

        IRepository<User> Users { get; }

        IUserStore<User> UserStore { get; }

        IRepository<IdentityRole> Roles { get; }

        IRepository<Reply> Reply { get; }

        IRepository<DevLog> DevLog { get; }

        IRepository<Group> Group { get; }

        IRepository<Image> Photo { get; }

        IRepository<CourseReview> CourseReview { get; }

        IRepository<UserLogTrace> UserLogTrace { get; }

        IRepository<HighAccLocationByIpResult> HighAccLocationByIpResult { get; }

        IRepository<Activity> Activity { get; }

        void SaveChanges();
    }
}