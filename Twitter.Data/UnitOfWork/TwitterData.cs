namespace Twitter.Data.UnitOfWork
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;

    using Twitter.Data.Repositories;

    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;

    using Twitter.Data;
    using Twitter.Models;

    public class TwitterData : ITwitterData
    {
        private readonly DbContext _dbContext;

        private readonly IDictionary<Type, object> _repositories;

        private IUserStore<User> _userStore;

        public TwitterData()
            : this(new TwitterDbContext())
        {
        }

        public TwitterData(DbContext dbContext)
        {
            this._dbContext = dbContext;
            this._repositories = new Dictionary<Type, object>();
        }

        public IRepository<Tweet> Tweets => this.GetRepository<Tweet>();

        public IRepository<Message> Messages => this.GetRepository<Message>();

        public IRepository<Report> Reports => this.GetRepository<Report>();

        public IRepository<Notification> Notifications => this.GetRepository<Notification>();

        public IRepository<User> Users => this.GetRepository<User>();

        public IRepository<IdentityRole> Roles => this.GetRepository<IdentityRole>();

        public IRepository<Reply> Reply => this.GetRepository<Reply>();

        public IRepository<DevLog> DevLog => this.GetRepository<DevLog>();

        public IRepository<Group> Group => this.GetRepository<Group>();

        public IRepository<Photo> Photo => this.GetRepository<Photo>();

        public IRepository<CourseReview> CourseReview => this.GetRepository<CourseReview>();

        public IUserStore<User> UserStore => this._userStore ?? (this._userStore = new UserStore<User>(this._dbContext));

        public void SaveChanges()
        {
            this._dbContext.SaveChanges();
        }

        public DbEntityEntry Entry(object entity)
        {
            return this._dbContext.Entry(entity);
        }

        private IRepository<T> GetRepository<T>() where T : class
        {
            if (!this._repositories.ContainsKey(typeof(T)))
            {
                var type = typeof(GenericEfRepository<T>);
                this._repositories.Add(typeof(T), Activator.CreateInstance(type, this._dbContext));
            }

            return (IRepository<T>)this._repositories[typeof(T)];
        }
    }
}