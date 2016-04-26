using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Helpers;
using Twitter.App.Common;
using Twitter.App.Models.ViewModels;
using Twitter.Models;

namespace Twitter.App.BusinessLogic
{
    public static class ManagerHelper
    {
        public static IReadOnlyList<TSource> GetPagedResult<TSource, TKey>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            int pageNo,
            int pageSize,
            SortDirection direction = SortDirection.Ascending)
        {
            Guard.ArgumentNotNull(source, nameof(source));
            Guard.ArgumentNotNull(keySelector, nameof(keySelector));
            Guard.Argument(() => pageNo > 0, nameof(pageNo), "Page number must be larger than zero");
            Guard.Argument(() => pageSize > 0, nameof(pageSize), "Page size must be larger than zero");

            int skip = (pageNo - 1) * pageSize;
            return
                (direction == SortDirection.Descending
                    ? source.OrderByDescending(keySelector)
                    : source.OrderBy(keySelector))
                    .Skip(skip)
                    .Take(pageSize)
                    .ToList();
        }

        public static readonly Expression<Func<Tweet, TweetViewModel>> AsTweetViewModel =
            t => new TweetViewModel
            {
                Id = t.Id,
                Author = t.Author.UserName,
                AuthorStatus = t.Author.Status,
                IsEvent = t.IsEvent,
                Text = t.Text,
                UsersFavouriteCount = t.UsersFavourite.Count,
                RepliesCount = t.Reply.Count,
                RetweetsCount = t.Retweets.Count,
                DatePosted = t.DatePosted,
                GroupId = t.GroupId,
                ReplyList = t.Reply.Select(reply => new ReplyViewModel
                {
                    Text = reply.Content,
                    Id = reply.Id,
                    PublishTime = reply.PublishTime,
                    Author = reply.AuthorName
                }).ToList()
            };
    }
}