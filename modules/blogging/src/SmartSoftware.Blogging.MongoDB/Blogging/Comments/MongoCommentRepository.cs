﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using SmartSoftware.Domain.Repositories.MongoDB;
using SmartSoftware.MongoDB;
using SmartSoftware.Blogging.MongoDB;

namespace SmartSoftware.Blogging.Comments
{
    public class MongoCommentRepository : MongoDbRepository<IBloggingMongoDbContext, Comment, Guid>, ICommentRepository
    {
        public MongoCommentRepository(IMongoDbContextProvider<IBloggingMongoDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public virtual async Task<List<Comment>> GetListOfPostAsync(Guid postId, CancellationToken cancellationToken = default)
        {
            return await (await GetMongoQueryableAsync(cancellationToken))
                .Where(a => a.PostId == postId)
                .OrderBy(a => a.CreationTime)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public virtual async Task<int> GetCommentCountOfPostAsync(Guid postId, CancellationToken cancellationToken = default)
        {
            return await (await GetMongoQueryableAsync(cancellationToken))
                .CountAsync(a => a.PostId == postId, GetCancellationToken(cancellationToken));
        }

        public virtual async Task<List<Comment>> GetRepliesOfComment(Guid id, CancellationToken cancellationToken = default)
        {
            return await (await GetMongoQueryableAsync(cancellationToken))
                .Where(a => a.RepliedCommentId == id).ToListAsync(GetCancellationToken(cancellationToken));
        }

        public virtual async Task DeleteOfPost(Guid id, CancellationToken cancellationToken = default)
        {
            var recordsToDelete = (await GetMongoQueryableAsync(cancellationToken)).Where(pt => pt.PostId == id);

            foreach (var record in recordsToDelete)
            {
                await DeleteAsync(record, cancellationToken: GetCancellationToken(cancellationToken));
            }
        }
    }
}
