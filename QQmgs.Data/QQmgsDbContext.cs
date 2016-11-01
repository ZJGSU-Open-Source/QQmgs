using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Twitter.Models.ActivityModels;
using Twitter.Models.CourseReviewModels;
using Twitter.Models.GroupModels;
using Twitter.Models.PhotoModels;
using Twitter.Models.TraceModels;
using Twitter.Models.UserModels;

namespace Twitter.Data
{
    using System.Data.Entity;

    using Microsoft.AspNet.Identity.EntityFramework;

    using Twitter.Data.Migrations;
    using Twitter.Models;

    public class QQmgsDbContext : IdentityDbContext<User>
    {
        public QQmgsDbContext()
            : base("name=TwitterDbContext")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<QQmgsDbContext, Configuration>());
        }

        public IDbSet<Tweet> Tweets { get; set; }

        public IDbSet<Message> Messages { get; set; }

        public IDbSet<Report> Reports { get; set; }

        public IDbSet<Notification> Notifications { get; set; }

        public static QQmgsDbContext Create()
        {
            return new QQmgsDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Message>()
                .HasRequired(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Message>()
                .HasRequired(m => m.Recipient)
                .WithMany()
                .HasForeignKey(m => m.RecipientId)
                .WillCascadeOnDelete(false);

            // Reply of Tweet
            modelBuilder.Entity<Reply>()
                .HasRequired(r => r.Author)
                .WithMany(u => u.Replies)
                .HasForeignKey(r => r.AuthorId)
                .WillCascadeOnDelete(false);

            // UserAvatar of User
            modelBuilder.Entity<UserProfleImage>()
                .HasRequired(ua => ua.Author)
                .WithMany(u => u.UserAvatars)
                .HasForeignKey(ua => ua.AuthorId)
                .WillCascadeOnDelete(false);

            // ActivityPhoto of Activity
            modelBuilder.Entity<ActivityPhoto>()
                .HasRequired(ap => ap.Activity)
                .WithMany(a => a.ActivityPhotos)
                .HasForeignKey(ap => ap.ActivityId)
                .WillCascadeOnDelete(false);

            // GroupPhoto of Group
            modelBuilder.Entity<GroupPhoto>()
                .HasRequired(gp => gp.Group)
                .WithMany(g => g.GroupPhotos)
                .HasForeignKey(gp => gp.GroupId)
                .WillCascadeOnDelete(false);

            // Group plugin
            modelBuilder.Entity<Group>()
                .HasOptional(g => g.GroupPlugin)
                .WithRequired(plugin => plugin.Group);

            modelBuilder.Entity<GroupPlugin>()
                .HasKey(plugin => plugin.GroupId);

            modelBuilder.Entity<Tweet>()
                .HasRequired(t => t.Group)
                .WithMany(u => u.Tweets)
                .HasForeignKey(t => t.GroupId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Tweet>()
                .HasRequired(t => t.Author)
                .WithMany(u => u.Tweets)
                .HasForeignKey(t => t.AuthorId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Image>()
                .HasRequired(t => t.Author)
                .WithMany(u => u.Photos)
                .HasForeignKey(t => t.AuthorId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CourseReview>()
                .HasRequired(t => t.Author)
                .WithMany(u => u.CourseReviews)
                .HasForeignKey(t => t.AuthorId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Activity>()
                .HasRequired(t => t.Creator)
                .WithMany(u => u.CreatedActivities)
                .HasForeignKey(t => t.CreatorId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<UserLogTrace>();

            modelBuilder.Entity<HighAccLocationByIpResult>();

            // more to more mapping relation
            modelBuilder.Entity<Group>()
                .HasMany(g => g.Users)
                .WithMany(u => u.Groups)
                .Map(m =>
                {
                    m.ToTable("UsersInGroups");
                    m.MapLeftKey("GroupId");
                    m.MapRightKey("UserId");
                });

            modelBuilder.Entity<Activity>()
                .HasMany(a => a.Participents)
                .WithMany(u => u.JoinedActivities)
                .Map(m =>
                {
                    m.ToTable("UsersInActivities");
                    m.MapLeftKey("ActivityId");
                    m.MapRightKey("UserId");
                });

            modelBuilder.Entity<Tweet>()
                .HasMany(t => t.Replies)
                .WithMany()
                .Map(t => t.MapLeftKey("TweetId").MapRightKey("ReplyTweetId").ToTable("TweetsReplies"));

            modelBuilder.Entity<Tweet>()
                .HasMany(t => t.Retweets)
                .WithMany()
                .Map(t => t.MapLeftKey("TweetId").MapRightKey("RetweetId").ToTable("TweetsRetweets"));

            modelBuilder.Entity<User>()
               .HasMany(u => u.FavouriteTweets)
               .WithMany(t => t.UsersFavourite)
               .Map(m =>
               {
                   m.MapLeftKey("UserId");
                   m.MapRightKey("TweetId");
                   m.ToTable("UsersFavouriteTweets");
               });

            modelBuilder.Entity<User>()
                .HasMany(u => u.FavouritePhotos)
                .WithMany(p => p.UsersFavourite)
                .Map(m =>
                {
                    m.MapLeftKey("UserId");
                    m.MapRightKey("PhotoId");
                    m.ToTable("UsersFavouritePhotos");
                });

            modelBuilder.Entity<User>()
                .HasMany(u => u.FollowingUsers)
                .WithMany(u => u.Followers)
                .Map(u => u.MapLeftKey("UserId").MapRightKey("FollowingUserId").ToTable("UsersFollowers"));

            base.OnModelCreating(modelBuilder);
        }
    }
}