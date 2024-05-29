﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using SmartSoftware;
using SmartSoftware.Domain.Repositories.MongoDB;
using SmartSoftware.MongoDB;
using SmartSoftware.CmsKit.Pages;

namespace SmartSoftware.CmsKit.MongoDB.Pages;

public class MongoPageRepository : MongoDbRepository<ICmsKitMongoDbContext, Page, Guid>, IPageRepository
{
    public MongoPageRepository(IMongoDbContextProvider<ICmsKitMongoDbContext> dbContextProvider) : base(
        dbContextProvider)
    {
    }

    public virtual async Task<int> GetCountAsync(string filter = null,
        CancellationToken cancellationToken = default)
    {
        var cancellation = GetCancellationToken(cancellationToken);

        return await (await GetMongoQueryableAsync(cancellation))
            .WhereIf<Page, IMongoQueryable<Page>>(
                !filter.IsNullOrWhiteSpace(),
                u =>
                    u.Title.ToLower().Contains(filter.ToLower()) || u.Slug.Contains(filter)
            ).CountAsync(cancellation);
    }

    public virtual async Task<List<Page>> GetListAsync(
        string filter = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        string sorting = null,
        CancellationToken cancellationToken = default)
    {
        var cancellation = GetCancellationToken(cancellationToken);

        return await (await GetMongoQueryableAsync(cancellation))
            .WhereIf<Page, IMongoQueryable<Page>>(
                !filter.IsNullOrWhiteSpace(),
                u => u.Title.ToLower().Contains(filter) || u.Slug.Contains(filter))
            .OrderBy(sorting.IsNullOrEmpty() ? nameof(Page.Title) : sorting)
            .As<IMongoQueryable<Page>>()
            .PageBy<Page, IMongoQueryable<Page>>(skipCount, maxResultCount)
            .ToListAsync(cancellation);
    }

    public virtual Task<Page> GetBySlugAsync([NotNull] string slug, CancellationToken cancellationToken = default)
    {
        Check.NotNullOrEmpty(slug, nameof(slug));
        return GetAsync(x => x.Slug == slug, cancellationToken: GetCancellationToken(cancellationToken));
    }

    public virtual Task<Page> FindBySlugAsync([NotNull] string slug, CancellationToken cancellationToken = default)
    {
        Check.NotNullOrEmpty(slug, nameof(slug));
        return FindAsync(x => x.Slug == slug, cancellationToken: GetCancellationToken(cancellationToken));
    }

    public virtual async Task<bool> ExistsAsync([NotNull] string slug, CancellationToken cancellationToken = default)
    {
        Check.NotNullOrEmpty(slug, nameof(slug));
        return await (await GetMongoQueryableAsync(cancellationToken)).AnyAsync(x => x.Slug == slug,
            GetCancellationToken(cancellationToken));
    }

    public virtual Task<List<Page>> GetListOfHomePagesAsync(CancellationToken cancellationToken = default)
    {
        return GetListAsync(x => x.IsHomePage, cancellationToken: GetCancellationToken(cancellationToken));
    }

    public async Task<string?> FindTitleAsync(Guid pageId, CancellationToken cancellationToken = default)
    {
        return await (await GetMongoQueryableAsync(cancellationToken)).Where(x => x.Id == pageId).Select(x => x.Title)
            .FirstOrDefaultAsync(cancellationToken);
    }
}