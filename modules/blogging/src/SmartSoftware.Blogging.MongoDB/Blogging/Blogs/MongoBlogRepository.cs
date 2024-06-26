﻿using System;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver.Linq;
using SmartSoftware.Domain.Repositories.MongoDB;
using SmartSoftware.MongoDB;
using SmartSoftware.Blogging.MongoDB;

namespace SmartSoftware.Blogging.Blogs
{
    public class MongoBlogRepository : MongoDbRepository<IBloggingMongoDbContext, Blog, Guid>, IBlogRepository
    {
        public MongoBlogRepository(IMongoDbContextProvider<IBloggingMongoDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public virtual async Task<Blog> FindByShortNameAsync(string shortName, CancellationToken cancellationToken = default)
        {
            return await (await GetMongoQueryableAsync(cancellationToken)).FirstOrDefaultAsync(p => p.ShortName == shortName, GetCancellationToken(cancellationToken));
        }
    }
}
