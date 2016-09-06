using Twitter.Models.Trace;

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

        public IDbSet<DevLog> DevLogs { get; set; }

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

            modelBuilder.Entity<Reply>()
                .HasRequired(r => r.Author)
                .WithMany(u => u.Replies)
                .HasForeignKey(r => r.AuthorId)
                .WillCascadeOnDelete(false);

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

            modelBuilder.Entity<Photo>()
                .HasRequired(t => t.Author)
                .WithMany(u => u.Photos)
                .HasForeignKey(t => t.AuthorId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CourseReview>()
                .HasRequired(t => t.Author)
                .WithMany(u => u.CourseReviews)
                .HasForeignKey(t => t.AuthorId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<UserLogTrace>();

            modelBuilder.Entity<HighAccLocationByIpResult>();

            modelBuilder.Entity<Group>()
                .HasMany(g => g.Users)
                .WithMany(u => u.Groups)
                .Map(m =>
                {
                    m.ToTable("UsersInGroups");
                    m.MapLeftKey("GroupId");
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