using System.Data.Entity.Infrastructure;

namespace Twitter.Data.UnitOfWork
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;

    using Twitter.Data.Repositories;

    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;

    using Twitter.Data;
    using Twitter.Data.UnitOfWork;
    using Twitter.Models;

    public class TwitterData : ITwitterData
    {
        private readonly DbContext dbContext;

        private readonly IDictionary<Type, object> repositories;

        private IUserStore<User> userStore;

        public TwitterData()
            : this(new TwitterDbContext())
        {
        }

        public TwitterData(DbContext dbContext)
        {
            this.dbContext = dbContext;
            this.repositories = new Dictionary<Type, object>();
        }

        public IRepository<Tweet> Tweets => this.GetRepository<Tweet>();

        public IRepository<Message> Messages => this.GetRepository<Message>();

        public IRepository<Report> Reports => this.GetRepository<Report>();

        public IRepository<Notification> Notifications => this.GetRepository<Notification>();

        public IRepository<User> Users => this.GetRepository<User>();

        public IRepository<IdentityRole> Roles => this.GetRepository<IdentityRole>();

        public IRepository<Reply> Reply => this.GetRepository<Reply>();

        public IUserStore<User> UserStore => this.userStore ?? (this.userStore = new UserStore<User>(this.dbContext));

        public void SaveChanges()
        {
            this.dbContext.SaveChanges();
        }

        public DbEntityEntry Entry(object entity)
        {
            return this.dbContext.Entry(entity);
        }

        private IRepository<T> GetRepository<T>() where T : class
        {
            if (!this.repositories.ContainsKey(typeof(T)))
            {
                var type = typeof(GenericEfRepository<T>);
                this.repositories.Add(typeof(T), Activator.CreateInstance(type, this.dbContext));
            }

            return (IRepository<T>)this.repositories[typeof(T)];
        }
    }
}